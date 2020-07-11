namespace Orimath.Core
open System
open System.Collections.Generic
open Orimath.Plugins

type PaperModel internal () as this =
    let mutable changeBlockDeclared = false
    let mutable changeBlockDisabled = false
    let undoOprStack = Stack<PaperOpr>()
    let redoOprStack = Stack<PaperOpr>()
    let selectedLayers = ReactiveProperty.createArray<ILayerModel> this
    let selectedEdges = ReactiveProperty.createArray<Edge> this
    let selectedPoints = ReactiveProperty.createArray<Point> this
    let selectedLines = ReactiveProperty.createArray<LineSegment> this
    let layerModels = ReactiveCollection<ILayerModel>(this)
    let layerChanged = CollectionChangedEvent<ILayerModel>()
    let canUndoChanged = Event<EventHandler, EventArgs>()

    do layerModels.Changed.Add(function
        | CollectionChange.Add(index, layers) ->
            this.PushUndoOpr(LayerAddition(index, asList layers))
            layerChanged.Trigger(this, CollectionChange.Add(index, layers))
        | CollectionChange.Remove(index, layers) ->
            this.PushUndoOpr(LayerRemoving(index, asList layers))
            layerChanged.Trigger(this, CollectionChange.Remove(index, layers))
        | CollectionChange.Replace(index, oldLayer, newLayer) ->
            this.PushUndoOpr(LayerReplace(index, oldLayer, newLayer))
            layerChanged.Trigger(this, CollectionChange.Replace(index, oldLayer, newLayer))
        | CollectionChange.Reset(odlLayers, newLayers) ->
            this.PushUndoOpr(Clear(asList odlLayers, asList newLayers))
            layerChanged.Trigger(this, CollectionChange.Reset(odlLayers, newLayers))
        )

    member __.Layers = layerModels :> IReadOnlyList<ILayerModel>
    member __.CanUndo = undoOprStack.Count > 0
    member __.CanRedo = redoOprStack.Count > 0
    member __.SelectedLayers with get() = selectedLayers.Value and set v = selectedLayers.Value <- v
    member __.SelectedEdges with get() = selectedEdges.Value and set v = selectedEdges.Value <- v
    member __.SelectedPoints with get() = selectedPoints.Value and set v = selectedPoints.Value <- v
    member __.SelectedLines with get() = selectedLines.Value and set v = selectedLines.Value <- v
    member __.ChangeBlockDeclared = changeBlockDeclared

    member __.SelectedLayersChanged = selectedLayers.ValueChanged
    member __.SelectedEdgesChanged = selectedEdges.ValueChanged
    member __.SelectedPointsChanged = selectedPoints.ValueChanged
    member __.SelectedLinesChanged = selectedLines.ValueChanged
    member __.LayerChanged = layerChanged.Publish
    member __.CanUndoChanged = canUndoChanged.Publish

    member internal __.PushUndoOpr(opr: PaperOpr) = if not changeBlockDisabled then undoOprStack.Push(opr)

    member __.GetSnapShot() =
        layerModels
        |> Seq.map(fun lm -> lm.GetSnapShot())
        |> Paper.Create

    member __.ResetSelection() =
        selectedLayers.Value <- Array.Empty()
        selectedEdges.Value <- Array.Empty()
        selectedPoints.Value <- Array.Empty()
        selectedLines.Value <- Array.Empty()

    member this.Undo() =
        if this.CanUndo && not this.ChangeBlockDeclared then
            this.ResetSelection()
            use __ = this.BeginUndo()
            redoOprStack.Push(BeginChangeBlock)
            let rec recSelf() =
                let opr = undoOprStack.Pop()
                if opr = BeginChangeBlock then
                    ()
                else
                    redoOprStack.Push(opr)
                    match opr with
                    | BeginChangeBlock -> failwith "想定しない動作です。"
                    | Clear(oldLayers, _) -> this.ClearRaw(oldLayers)
                    | LayerAddition(_, layers) -> this.RemoveLayers(layers.Length)
                    | LayerRemoving(_, layers) -> this.AddLayersRaw(layers)
                    | LayerReplace(index, oldLayer, _) -> this.ReplaceLayerRaw(index, oldLayer)
                    | LineAddition(layerIndex, _, lines) -> layerModels.[layerIndex].RemoveLines(lines.Length)
                    | LineRemoving(layerIndex, _, lines) -> layerModels.[layerIndex].AddLinesRaw(lines)
                    | PointAddition(layerIndex, _, points) -> layerModels.[layerIndex].RemovePoints(points.Length)
                    | PointRemoving(layerIndex, _, points) -> layerModels.[layerIndex].AddPoints(points)
                    recSelf()
            recSelf()

    member this.Redo() =
        if this.CanRedo && not this.ChangeBlockDeclared then
            this.ResetSelection()
            use __ = this.BeginUndo()
            undoOprStack.Push(BeginChangeBlock)
            let rec recSelf() =
                let opr = redoOprStack.Pop()
                if opr = BeginChangeBlock then
                    ()
                else
                    undoOprStack.Push(opr)
                    match opr with
                    | BeginChangeBlock -> failwith "想定しない動作です。"
                    | Clear(_, newLayers) -> this.ClearRaw(newLayers)
                    | LayerAddition(_, layers) -> this.AddLayersRaw(layers)
                    | LayerRemoving(_, layers) -> this.RemoveLayers(layers.Length)
                    | LayerReplace(index, _, newLayer) -> this.ReplaceLayerRaw(index, newLayer)
                    | LineAddition(layerIndex, _, lines) -> layerModels.[layerIndex].AddLinesRaw(lines)
                    | LineRemoving(layerIndex, _, lines) -> layerModels.[layerIndex].RemoveLines(lines.Length)
                    | PointAddition(layerIndex, _, points) -> layerModels.[layerIndex].AddPoints(points)
                    | PointRemoving(layerIndex, _, points) -> layerModels.[layerIndex].RemovePoints(points.Length)
                    recSelf()
            recSelf()

    member this.BeginChange() =
        if changeBlockDeclared then invalidOp "既に変更ブロックが定義されています。"
        redoOprStack.Clear()
        changeBlockDeclared <- true
        undoOprStack.Push(BeginChangeBlock)
        { new IDisposable with
            member __.Dispose() =
                if undoOprStack.Peek() = BeginChangeBlock then
                    ignore (undoOprStack.Pop())
                changeBlockDeclared <- false
                canUndoChanged.Trigger(this, EventArgs.Empty)
        }

    member private __.BeginUndo() =
        if changeBlockDeclared then invalidOp "変更ブロックが定義されているため、Undoを開始できません。"
        changeBlockDeclared <- true
        changeBlockDisabled <- true
        { new IDisposable with
            member __.Dispose() =
                changeBlockDeclared <- false
                changeBlockDisabled <- false
                canUndoChanged.Trigger(this, EventArgs.Empty)
        }

    member private this.ClearRaw(layers: ILayerModel list) =
        use __ = this.TryBeginChange()
        layerModels.Reset(layers)

    member this.Clear(paper: IPaper) =
        this.ResetSelection()
        use __ = this.TryBeginChange()
        let layers =
            paper.Layers
            |> Seq.indexed
            |> Seq.map(fun (index, ly) -> LayerModel(this, index, Layer.AsLayer(ly)) :> ILayerModel)
            |> Seq.toList
        this.ClearRaw(layers)

    member this.Clear() =
        this.Clear(Paper.FromSize(1.0, 1.0))
        this.ClearUndoStack()

    member __.ClearUndoStack() =
        undoOprStack.Clear()
        redoOprStack.Clear()

    member private this.AddLayersRaw(layers) =
        if layers <> [] then
            use __ = this.TryBeginChange()
            layerModels.Add(layers)

    member this.AddLayers(layers: seq<ILayer>) =
        let layers =
            layers
            |> Seq.indexed
            |> Seq.map(fun (index, ly) -> LayerModel(this, layerModels.Count + index, Layer.AsLayer(ly)) :> ILayerModel)
            |> Seq.toList
        this.AddLayersRaw(layers)

    member this.RemoveLayers(count: int) =
        if count > 0 then
            use __ = this.TryBeginChange()
            layerModels.Remove(count)

    member private this.ReplaceLayerRaw(index: int, newLayer) =
        use __ = this.TryBeginChange()
        layerModels.Replace(index, newLayer)

    member this.ReplaceLayer(index: int, newLayer: ILayer) =
        this.ReplaceLayerRaw(index, LayerModel(this, index, Layer.AsLayer(newLayer)))

    interface IPaper with
        member this.Layers = this.Layers :> IReadOnlyCollection<ILayerModel> :?> IReadOnlyList<ILayer>

    interface IInternalPaperModel with
        member this.PushUndoOpr(opr) = this.PushUndoOpr(opr)
        member this.Layers = this.Layers
        member this.CanUndo = this.CanUndo
        member this.CanRedo = this.CanRedo
        member this.ChangeBlockDeclared = this.ChangeBlockDeclared
        member this.SelectedLayers with get() = this.SelectedLayers and set(v) = this.SelectedLayers <- v
        member this.SelectedEdges with get() = this.SelectedEdges and set(v) = this.SelectedEdges <- v
        member this.SelectedPoints with get() = this.SelectedPoints and set(v) = this.SelectedPoints <- v
        member this.SelectedLines with get() = this.SelectedLines and set(v) = this.SelectedLines <- v

        [<CLIEvent>] member this.SelectedLayersChanged = this.SelectedLayersChanged
        [<CLIEvent>] member this.SelectedEdgesChanged = this.SelectedEdgesChanged 
        [<CLIEvent>] member this.SelectedPointsChanged = this.SelectedPointsChanged
        [<CLIEvent>] member this.SelectedLinesChanged = this.SelectedLinesChanged 
        [<CLIEvent>] member this.LayerChanged = this.LayerChanged
        [<CLIEvent>] member this.CanUndoChanged = this.CanUndoChanged

        member this.GetSnapShot() = upcast this.GetSnapShot()
        member this.Undo() = this.Undo()
        member this.Redo() = this.Redo()
        member this.BeginChange() = this.BeginChange()
        member this.Clear() = this.Clear()
        member this.Clear(paper) = this.Clear(paper)
        member this.ClearUndoStack() = this.ClearUndoStack()
        member this.AddLayers(layers) = this.AddLayers(layers)
        member this.RemoveLayers(count) = this.RemoveLayers(count)
        member this.ReplaceLayer(index: int, newLayer: ILayer) = this.ReplaceLayer(index, newLayer)
