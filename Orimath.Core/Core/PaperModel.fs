namespace Orimath.Core
open System
open System.Collections.Generic
open Orimath.Plugins
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type PaperModel internal () as this =
    let mutable changeBlockDeclared = false
    let mutable changeBlockDisabled = false
    let undoOprStack = Stack<PaperOpr>()
    let redoOprStack = Stack<PaperOpr>()
    let selectedLayers = createArrayProp<ILayerModel>()
    let selectedEdges = createArrayProp<Edge>()
    let selectedPoints = createArrayProp<Point>()
    let selectedLines = createArrayProp<LineSegment>()
    let layerModels = ReactiveCollection<ILayerModel>()
    let canUndo = ValueProp<bool>(false)
    let canRedo = ValueProp<bool>(false)

    do layerModels.Add(function
        | CollectionChange.Add(index, layers) ->
            this.PushUndoOpr(LayerAddition(index, asList layers))
        | CollectionChange.Remove(index, layers) ->
            this.PushUndoOpr(LayerRemoving(index, asList layers))
        | CollectionChange.Replace(index, oldLayer, newLayer) ->
            this.PushUndoOpr(LayerReplace(index, oldLayer, newLayer))
        | CollectionChange.Reset(odlLayers, newLayers) ->
            this.PushUndoOpr(Clear(asList odlLayers, asList newLayers))
        )

    member _.Layers = layerModels :> IReadOnlyList<ILayerModel>
    member val CanUndo = Prop.asGet canUndo
    member val CanRedo = Prop.asGet canRedo
    member _.SelectedLayers = selectedLayers
    member _.SelectedEdges = selectedEdges
    member _.SelectedPoints = selectedPoints
    member _.SelectedLines = selectedLines
    member _.ChangeBlockDeclared = changeBlockDeclared

    member internal _.PushUndoOpr(opr: PaperOpr) = if not changeBlockDisabled then undoOprStack.Push(opr)

    member _.GetSnapShot() =
        layerModels
        |> Seq.map(fun lm -> lm.GetSnapShot())
        |> Paper.Create

    member _.ResetSelection() =
        selectedLayers .<- Array.Empty()
        selectedEdges .<- Array.Empty()
        selectedPoints .<- Array.Empty()
        selectedLines .<- Array.Empty()

    member private _.UpdateCanUndo() =
        canUndo .<- (undoOprStack.Count > 0)
        canRedo .<- (redoOprStack.Count > 0)

    member this.Undo() =
        if this.CanUndo.Value && not this.ChangeBlockDeclared then
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
        if this.CanRedo.Value && not this.ChangeBlockDeclared then
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
            member _.Dispose() =
                if undoOprStack.Peek() = BeginChangeBlock then
                    ignore (undoOprStack.Pop())
                changeBlockDeclared <- false
                this.UpdateCanUndo()
        }

    member private this.BeginUndo() =
        if changeBlockDeclared then invalidOp "変更ブロックが定義されているため、Undoを開始できません。"
        changeBlockDeclared <- true
        changeBlockDisabled <- true
        { new IDisposable with
            member _.Dispose() =
                changeBlockDeclared <- false
                changeBlockDisabled <- false
                this.UpdateCanUndo()
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

    member _.ClearUndoStack() =
        undoOprStack.Clear()
        redoOprStack.Clear()

    member private this.AddLayersRaw(layers) =
        if layers <> [] then
            use __ = this.TryBeginChange()
            layerModels.AddRange(layers)

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
            layerModels.RemoveTail(count)

    member private this.ReplaceLayerRaw(index: int, newLayer) =
        use __ = this.TryBeginChange()
        layerModels.Replace(index, newLayer)

    member this.ReplaceLayer(index: int, newLayer: ILayer) =
        this.ReplaceLayerRaw(index, LayerModel(this, index, Layer.AsLayer(newLayer)))

    interface IPaper with
        member _.Layers = layerModels :> IReadOnlyList<ILayerModel> :?> IReadOnlyList<ILayer>

    interface IInternalPaperModel with
        member this.PushUndoOpr(opr) = this.PushUndoOpr(opr)
        member _.Layers = upcast layerModels
        member this.CanUndo = this.CanUndo
        member this.CanRedo = this.CanRedo
        member this.ChangeBlockDeclared = this.ChangeBlockDeclared
        member this.SelectedLayers = upcast this.SelectedLayers
        member this.SelectedEdges = upcast this.SelectedEdges
        member this.SelectedPoints = upcast this.SelectedPoints
        member this.SelectedLines = upcast this.SelectedLines

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
