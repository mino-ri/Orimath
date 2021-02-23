namespace Orimath.Core
open System
open System.Collections.Generic
open Orimath.Plugins
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type PaperModel internal () as this =
    let selectedLayers = createArrayProp<ILayerModel>()
    let selectedEdges = createArrayProp<Edge>()
    let selectedPoints = createArrayProp<Point>()
    let selectedCreases = createArrayProp<Crease>()
    let layerModels = ReactiveCollection<ILayerModel>()
    let undoStack = UndoStack()

    do layerModels.Add(function
        | CollectionChange.Add(index, layers) ->
            undoStack.PushUndoOpr(LayerAddition(index, asList layers))
        | CollectionChange.Remove(index, layers) ->
            undoStack.PushUndoOpr(LayerRemoving(index, asList layers))
        | CollectionChange.Replace(index, oldLayer, newLayer) ->
            undoStack.PushUndoOpr(LayerReplace(index, oldLayer, newLayer))
        | CollectionChange.Reset(odlLayers, newLayers) ->
            undoStack.PushUndoOpr(Clear(asList odlLayers, asList newLayers)))
    do undoStack.OnUndo.Add(this.Undo)
    do undoStack.OnRedo.Add(this.Redo)

    member _.Layers = layerModels :> IReadOnlyList<ILayerModel>
    member _.SelectedLayers = selectedLayers
    member _.SelectedEdges = selectedEdges
    member _.SelectedPoints = selectedPoints
    member _.SelectedCreases = selectedCreases

    member _.GetSnapShot() =
        layerModels
        |> Seq.map (fun lm -> lm.GetSnapShot())
        |> Paper.Create
        :> IPaper

    member _.ResetSelection() =
        selectedLayers .<- array.Empty()
        selectedEdges .<- array.Empty()
        selectedPoints .<- array.Empty()
        selectedCreases .<- array.Empty()

    member private this.Undo(oprBlock) =
        this.ResetSelection()
        for i = oprBlock.Operations.Length - 1 downto 0 do
            match oprBlock.Operations.[i] with
            | Clear(oldLayers, _) -> this.ClearRaw(oldLayers)
            | LayerAddition(_, layers) -> this.RemoveLayers(layers.Length)
            | LayerRemoving(_, layers) -> this.AddLayersRaw(layers)
            | LayerReplace(index, oldLayer, _) -> this.ReplaceLayerRaw(index, oldLayer)
            | LineAddition(layerIndex, _, creases) ->
                layerModels.[layerIndex].RemoveCreases(creases.Length)
            | LineRemoving(layerIndex, _, creases) ->
                layerModels.[layerIndex].AddCreasesRaw(creases)
            | PointAddition(layerIndex, _, points) ->
                layerModels.[layerIndex].RemovePoints(points.Length)
            | PointRemoving(layerIndex, _, points) ->
                layerModels.[layerIndex].AddPoints(points)

    member private this.Redo(oprBlock) =
        this.ResetSelection()
        for opr in oprBlock.Operations do
            match opr with
            | Clear(_, newLayers) -> this.ClearRaw(newLayers)
            | LayerAddition(_, layers) -> this.AddLayersRaw(layers)
            | LayerRemoving(_, layers) -> this.RemoveLayers(layers.Length)
            | LayerReplace(index, _, newLayer) -> this.ReplaceLayerRaw(index, newLayer)
            | LineAddition(layerIndex, _, creases) ->
                layerModels.[layerIndex].AddCreasesRaw(creases)
            | LineRemoving(layerIndex, _, creases) ->
                layerModels.[layerIndex].RemoveCreases(creases.Length)
            | PointAddition(layerIndex, _, points) ->
                layerModels.[layerIndex].AddPoints(points)
            | PointRemoving(layerIndex, _, points) ->
                layerModels.[layerIndex].RemovePoints(points.Length)

    member this.BeginChange(tag) =
        undoStack.BeginChange(this.GetSnapShot(), tag)
        
    member this.TryBeginChange(tag: obj) =
        if undoStack.ChangeBlockDeclared || undoStack.ChangeBlockDisabled
        then { new IDisposable with member _.Dispose() = () }
        else this.BeginChange(tag)

    member private this.ClearRaw(layers: ILayerModel list) =
        use __ = this.TryBeginChange(null)
        layerModels.Reset(layers)

    member this.Clear(paper: IPaper) =
        this.ResetSelection()
        use __ = this.TryBeginChange(null)
        let layers =
            paper.Layers
            |> Seq.mapi (fun index ly -> LayerModel(this, index, Layer.AsLayer(ly)) :> ILayerModel)
            |> Seq.toList
        this.ClearRaw(layers)

    member this.Clear() =
        use __ = undoStack.DisableChangeBlock()
        this.Clear(Paper.FromSize(1.0, 1.0))
        undoStack.ClearUndoStack()

    member private this.AddLayersRaw(layers) =
        if layers <> [] then
            use __ = this.TryBeginChange(null)
            layerModels.AddRange(layers)

    member this.AddLayers(layers: seq<ILayer>) =
        let layers =
            layers
            |> Seq.mapi (fun index ly ->
                LayerModel(this, layerModels.Count + index, Layer.AsLayer(ly)) :> ILayerModel)
            |> Seq.toList
        this.AddLayersRaw(layers)

    member this.RemoveLayers(count: int) =
        if count > 0 then
            use __ = this.TryBeginChange(null)
            layerModels.RemoveTail(count)

    member private this.ReplaceLayerRaw(index: int, newLayer) =
        use __ = this.TryBeginChange(null)
        layerModels.Replace(index, newLayer)

    member this.ReplaceLayer(index: int, newLayer: ILayer) =
        this.ReplaceLayerRaw(index, LayerModel(this, index, Layer.AsLayer(newLayer)))

    interface IPaper with
        member _.Layers = layerModels :> IReadOnlyList<ILayerModel> :?> IReadOnlyList<ILayer>

    interface IInternalPaperModel with
        member _.Layers = upcast layerModels
        member _.PushUndoOpr(opr) = undoStack.PushUndoOpr(opr)
        member _.CanUndo = undoStack.CanUndo
        member _.CanRedo = undoStack.CanRedo
        member this.SelectedLayers = upcast this.SelectedLayers
        member this.SelectedEdges = upcast this.SelectedEdges
        member this.SelectedPoints = upcast this.SelectedPoints
        member this.SelectedCreases = upcast this.SelectedCreases

        member this.GetSnapShot() = this.GetSnapShot()
        member _.Undo() = undoStack.Undo()
        member _.Redo() = undoStack.Redo()
        member _.UndoSnapShots = undoStack.UndoTags
        member _.RedoSnapShots = undoStack.RedoTags
        member _.ClearUndoStack() = undoStack.ClearUndoStack()
        member this.BeginChange(tag) = this.BeginChange(tag)
        member this.TryBeginChange(tag) = this.TryBeginChange(tag)
        member this.Clear() = this.Clear()
        member this.Clear(paper) = this.Clear(paper)
        member this.AddLayers(layers) = this.AddLayers(layers)
        member this.RemoveLayers(count) = this.RemoveLayers(count)
        member this.ReplaceLayer(index, newLayer) = this.ReplaceLayer(index, newLayer)
