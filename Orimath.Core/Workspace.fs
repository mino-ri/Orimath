namespace Orimath.Core
open System
open System.Collections.Generic
open Orimath.Plugins

type internal WorkspaceEvent<'TValue> = Event<WorkspaceEventHandler<'TValue>, WorkspaceEventArgs<'TValue>>

type internal PaperOpr =
    | BeginChangeBlock
    | Clear of before: Paper * after: Paper
    | LayerAddition of layers: Layer list
    | LayerRemoving of layers: Layer list
    | LayerReplace of layerIndex: int * before: Layer * after: Layer
    | LineAddition of layerIndex: int * lines: LineSegment list
    | LineRemoving of layerIndex: int * lines: LineSegment list
    | PointAddition of layerIndex: int * points: Point list
    | PointRemoving of layerIndex: int * points: Point list

type Workspace() =
    let mutable paper = Paper.FromSize(1.0, 1.0)
    let mutable tools = Array.empty<ITool>
    let mutable effects = Array.empty<IEffect>
    let mutable changeBlockDeclared = false
    let undoOprStack = Stack<PaperOpr>()
    let redoOprStack = Stack<PaperOpr>()
    let undoStack = Stack<Paper>()
    let redoStack = Stack<Paper>()
    let currentTool = ReactiveProperty.createEq(Unchecked.defaultof<ITool>)
    let selectedLayers = ReactiveProperty.createArray<Layer>
    let selectedEdges = ReactiveProperty.createArray<Edge>
    let selectedPoints = ReactiveProperty.createArray<Point>
    let selectedLines = ReactiveProperty.createArray<LineSegment>
    let paperCleared = WorkspaceEvent<Paper>()
    let layerAdded   = WorkspaceEvent<IReadOnlyCollection<Layer>>()
    let layerRemoved = WorkspaceEvent<IReadOnlyCollection<Layer>>()
    let layerReplaced = WorkspaceEvent<Layer>()
    let lineAdded    = WorkspaceEvent<IReadOnlyCollection<LineSegment>>()
    let lineRemoved  = WorkspaceEvent<IReadOnlyCollection<LineSegment>>()
    let pointAdded   = WorkspaceEvent<IReadOnlyCollection<Point>>()
    let pointRemoved = WorkspaceEvent<IReadOnlyCollection<Point>>()

    member private this.Trigger(event: WorkspaceEvent<'a>, layerIndex, value) =
        event.Trigger(this, WorkspaceEventArgs(layerIndex, value))

    member __.Paper = paper
    member __.UndoStack = undoStack :> IReadOnlyCollection<Paper>
    member __.RedoStack = redoStack :> IReadOnlyCollection<Paper>
    member __.CanUndo = undoStack.Count > 0
    member __.CanRedo = redoStack.Count > 0
    member __.Tools = tools :> IReadOnlyCollection<ITool>
    member __.Effects = effects :> IReadOnlyCollection<IEffect>
    member __.CurrentTool with get() = currentTool.Value and set v = currentTool.Value <- v
    member __.SelectedLayers with get() = selectedLayers.Value and set v = selectedLayers.Value <- v
    member __.SelectedEdges with get() = selectedEdges.Value and set v = selectedEdges.Value <- v
    member __.SelectedPoints with get() = selectedPoints.Value and set v = selectedPoints.Value <- v
    member __.SelectedLines with get() = selectedLines.Value and set v = selectedLines.Value <- v
    member __.ChangeBlockDeclared = changeBlockDeclared

    [<CLIEvent>]
    member __.CurrentToolChanged = currentTool.ValueChanged
    [<CLIEvent>]
    member __.SelectedLayersChanged = selectedLayers.ValueChanged
    [<CLIEvent>]
    member __.SelectedEdgesChanged = selectedEdges.ValueChanged
    [<CLIEvent>]
    member __.SelectedPointsChanged = selectedPoints.ValueChanged
    [<CLIEvent>]
    member __.SelectedLinesChanged = selectedLines.ValueChanged
    [<CLIEvent>]
    member __.PaperCleared = paperCleared.Publish
    [<CLIEvent>]
    member __.LayerAdded = layerAdded.Publish
    [<CLIEvent>]
    member __.LayerRemoved = layerRemoved.Publish
    [<CLIEvent>]
    member __.LayerReplaced = layerReplaced.Publish
    [<CLIEvent>]
    member __.LineAdded = lineAdded.Publish
    [<CLIEvent>]
    member __.LineRemoved = lineRemoved.Publish
    [<CLIEvent>]
    member __.PointAdded = pointAdded.Publish
    [<CLIEvent>]
    member __.PointRemoved = pointRemoved.Publish

    member this.Undo() =
        if this.CanUndo && not this.ChangeBlockDeclared then
            redoStack.Push(paper)
            redoOprStack.Push(BeginChangeBlock)
            paper <- undoStack.Pop()
            let rec recSelf() =
                let opr = undoOprStack.Pop()
                if opr = BeginChangeBlock then
                    ()
                else
                    redoOprStack.Push(opr)
                    match opr with
                    | BeginChangeBlock -> failwith "想定しない動作"
                    | Clear(before, _) -> this.Trigger(paperCleared, -1, before)
                    | LayerAddition(layers) -> this.Trigger(layerRemoved, -1, upcast layers)
                    | LayerRemoving(layers) -> this.Trigger(layerAdded, -1, upcast layers)
                    | LayerReplace (layerIndex, before, _) -> this.Trigger(layerReplaced, layerIndex, before)
                    | LineAddition(layerIndex, lines) -> this.Trigger(lineRemoved, layerIndex, upcast lines)
                    | LineRemoving(layerIndex, lines) -> this.Trigger(lineAdded, layerIndex, upcast lines)
                    | PointAddition(layerIndex, points) -> this.Trigger(pointRemoved, layerIndex, upcast points)
                    | PointRemoving(layerIndex, points) -> this.Trigger(pointAdded, layerIndex, upcast points)
                    recSelf()
            recSelf()
    member this.Redo() =
        if this.CanRedo && not this.ChangeBlockDeclared then
            undoStack.Push(paper)
            undoOprStack.Push(BeginChangeBlock)
            paper <- redoStack.Pop()
            let rec recSelf() =
                let opr = redoOprStack.Pop()
                if opr = BeginChangeBlock then
                    ()
                else
                    undoOprStack.Push(opr)
                    match opr with
                    | BeginChangeBlock -> failwith "想定しない動作"
                    | Clear(_, after) -> this.Trigger(paperCleared, -1, after)
                    | LayerAddition(layers) -> this.Trigger(layerAdded, -1, upcast layers)
                    | LayerRemoving(layers) -> this.Trigger(layerRemoved, -1, upcast layers)
                    | LayerReplace (layerIndex, _, after) -> this.Trigger(layerReplaced, layerIndex, after)
                    | LineAddition(layerIndex, lines) -> this.Trigger(lineAdded, layerIndex, upcast lines)
                    | LineRemoving(layerIndex, lines) -> this.Trigger(lineRemoved, layerIndex, upcast lines)
                    | PointAddition(layerIndex, points) -> this.Trigger(pointAdded, layerIndex, upcast points)
                    | PointRemoving(layerIndex, points) -> this.Trigger(pointRemoved, layerIndex, upcast points)
                    recSelf()
            recSelf()
    member __.BeginChangeCore() =
        redoStack.Clear()
        redoOprStack.Clear()
        changeBlockDeclared <- true
        undoStack.Push(paper)
        undoOprStack.Push(BeginChangeBlock)
    member this.BeginChange() =
        if changeBlockDeclared then invalidOp "既に変更ブロックが定義されています。"
        this.BeginChangeCore()
        { new IDisposable with member __.Dispose() = changeBlockDeclared <- false }
    member this.TryBeginChange() =
        if changeBlockDeclared then
            { new IDisposable with member __.Dispose() = () }
        else
            this.BeginChangeCore()
            { new IDisposable with member __.Dispose() = changeBlockDeclared <- false }

    member this.Clear() = this.Clear(Paper.FromSize(1.0, 1.0))

    member this.Clear(newPaper: Paper) =
        use __ = this.TryBeginChange()
        undoOprStack.Push(Clear(paper, newPaper))
        paper <- newPaper
        this.Trigger(paperCleared, -1, newPaper)

    member this.AddLayers(layers: seq<Layer>) =
        let layers = asList layers
        if layers <> [] then
            use __ = this.TryBeginChange()
            undoOprStack.Push(LayerAddition(layers))
            paper <- Paper.Create(layers @ paper.Layers)
            this.Trigger(layerAdded, -1, upcast layers)

    member this.RemoveLayers(count: int) =
        if count > 0 then
            use __ = this.TryBeginChange()
            let target, rest = List.splitAt count paper.Layers
            undoOprStack.Push(LayerRemoving(target))
            paper <- Paper.Create(rest)
            this.Trigger(layerRemoved, -1, upcast target)

    member __.TryFindLayer(target) = List.tryFindIndex((=) target) paper.Layers

    member inline private this.TryFindLayer(target, action) =
        match this.TryFindLayer(target) with
        | None -> ()
        | Some(index) -> action index

    member private __.ReplaceLayerCore(index, after) =
        let a, b = List.splitAt index paper.Layers
        paper <- Paper.Create(a @ after :: b.Tail)

    member this.ReplaceLayer(before: Layer, after: Layer) =
        this.TryFindLayer(before, fun index ->
            use __ = this.TryBeginChange()
            undoOprStack.Push(LayerReplace(index, before, after))
            this.ReplaceLayerCore(index, after)
            this.Trigger(layerReplaced, index, after))

    member private this.AddLineCore(layer: Layer, lines: seq<LineSegment>, addCross: bool) =
        this.TryFindLayer(layer, fun index ->
            let lines = lines |> Seq.filter(layer.HasLine >> not) |> Seq.toList
            if lines <> [] then
                let points = if addCross then Layer.crossAll layer lines else []
                use __ = this.TryBeginChange()
                undoOprStack.Push(LineAddition(index, lines))
                if points <> [] then undoOprStack.Push(PointAddition(index, points))
                this.ReplaceLayerCore(index, Layer.add layer lines points)
                this.Trigger(lineAdded, index, upcast lines)
                if points <> [] then this.Trigger(pointAdded, index, upcast points))

    member this.AddLinesRaw(layer: Layer, lines: seq<Line>) = this.AddLineCore(layer, lines |> Seq.collect(layer.Clip), false)

    member this.AddLinesRaw(layer: Layer, lines: seq<LineSegment>) = this.AddLineCore(layer, lines |> Seq.collect(layer.Clip), false)

    member this.AddLines(layer: Layer, lines: seq<Line>) = this.AddLineCore(layer, lines |> Seq.collect(layer.Clip), true)

    member this.AddLines(layer: Layer, lines: seq<LineSegment>) = this.AddLineCore(layer, lines |> Seq.collect(layer.Clip), true)
    
    member this.RemoveLines(layer: Layer, count: int) =
        if count > 0 then
            this.TryFindLayer(layer, fun index ->
                use __ = this.TryBeginChange()
                let target, rest = List.splitAt count layer.Lines
                undoOprStack.Push(LineRemoving(index, target))
                this.ReplaceLayerCore(index, Layer.Create(layer.Edges, rest, layer.Points))
                this.Trigger(lineRemoved, index, upcast target))

    member this.AddPoints(layer: Layer, points: seq<Point>) =
        this.TryFindLayer(layer, fun index ->
            let points = points |> Seq.filter(fun p -> layer.Contains(p) && not (layer.HasPoint(p))) |> Seq.toList
            if points <> [] then
                use __ = this.TryBeginChange()
                undoOprStack.Push(PointAddition(index, points))
                this.ReplaceLayerCore(index, Layer.addPoints layer points)
                this.Trigger(pointAdded, index, upcast points)
            )

    member this.RemovePoints(layer: Layer, count: int) =
        if count > 0 then
            this.TryFindLayer(layer, fun index ->
                use __ = this.TryBeginChange()
                let target, rest = List.splitAt count layer.Points
                undoOprStack.Push(PointRemoving(index, target))
                this.ReplaceLayerCore(index, Layer.Create(layer.Edges, layer.Lines, rest))
                this.Trigger(pointRemoved, index, upcast target))

    interface IWorkspace with
        member this.Paper = this.Paper
        member this.UndoStack = this.UndoStack
        member this.RedoStack = this.RedoStack
        member this.CanUndo = this.CanUndo
        member this.CanRedo = this.CanRedo
        member this.Tools = this.Tools
        member this.Effects = this.Effects
        member this.CurrentTool with get() = this.CurrentTool and set v = this.CurrentTool <- v
        member this.SelectedLayers with get() = this.SelectedLayers and set v = this.SelectedLayers <- v
        member this.SelectedEdges with get() = this.SelectedEdges and set v = this.SelectedEdges <- v
        member this.SelectedPoints with get() = this.SelectedPoints and set v = this.SelectedPoints <- v
        member this.SelectedLines with get() = this.SelectedLines and set v = this.SelectedLines <- v
        member this.ChangeBlockDeclared = this.ChangeBlockDeclared

        [<CLIEvent>] member this.CurrentToolChanged = this.CurrentToolChanged
        [<CLIEvent>] member this.SelectedLayersChanged = this.SelectedLayersChanged
        [<CLIEvent>] member this.SelectedEdgesChanged = this.SelectedEdgesChanged
        [<CLIEvent>] member this.SelectedPointsChanged = this.SelectedPointsChanged
        [<CLIEvent>] member this.SelectedLinesChanged = this.SelectedLinesChanged
        [<CLIEvent>] member this.PaperCleared = this.PaperCleared
        [<CLIEvent>] member this.LayerAdded = this.LayerAdded
        [<CLIEvent>] member this.LayerRemoved = this.LayerRemoved
        [<CLIEvent>] member this.LayerReplaced = this.LayerReplaced
        [<CLIEvent>] member this.LineAdded = this.LineAdded
        [<CLIEvent>] member this.LineRemoved = this.LineRemoved
        [<CLIEvent>] member this.PointAdded = this.PointAdded
        [<CLIEvent>] member this.PointRemoved = this.PointRemoved

        member this.Undo() = this.Undo()
        member this.Redo() = this.Redo()
        member this.BeginChange() = this.BeginChange()
        member this.Clear() = this.Clear()
        member this.Clear(paper) = this.Clear(paper)
        member this.AddLayers(layers) = this.AddLayers(layers)
        member this.RemoveLayers(count) = this.RemoveLayers(count)
        member this.ReplaceLayer(before, after) = this.ReplaceLayer(before, after)
        member this.AddLines(layer, lines: seq<Line>) = this.AddLines(layer, lines)
        member this.AddLines(layer, lines: seq<LineSegment>) = this.AddLines(layer, lines)
        member this.AddLinesRaw(layer, lines: seq<Line>) = this.AddLinesRaw(layer, lines)
        member this.AddLinesRaw(layer, lines: seq<LineSegment>) = this.AddLinesRaw(layer, lines)
        member this.RemoveLines(layer, count) = this.RemoveLines(layer, count)
        member this.AddPoints(layer, points) = this.AddPoints(layer, points)
        member this.RemovePoints(layer, count) = this.RemovePoints(layer, count)
