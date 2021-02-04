namespace Orimath.Core
open Orimath.Plugins
open ApplicativeProperty

type internal PaperOpr =
    | BeginChangeBlock
    | Clear of before: ILayerModel list * after: ILayerModel list
    | LayerAddition of index: int * layers: ILayerModel list
    | LayerRemoving of index: int * layers: ILayerModel list
    | LayerReplace of layerIndex: int * before: ILayerModel * after: ILayerModel
    | LineAddition of layerIndex: int * index: int * lines: LineSegment list
    | LineRemoving of layerIndex: int * index: int * lines: LineSegment list
    | PointAddition of layerIndex: int * index: int * points: Point list
    | PointRemoving of layerIndex: int * index: int * points: Point list

and internal IInternalPaperModel =
    inherit IPaperModel
    abstract member PushUndoOpr : opr: PaperOpr -> unit

and LayerModel internal (parent: IInternalPaperModel, layerIndex: int, init: Layer) =
    let layerLines = ReactiveCollection<LineSegment>(init.Lines)
    let layerPoints = ReactiveCollection<Point>(init.Points)
    
    do layerLines.Add(function
        | CollectionChange.Add(index, lines) -> parent.PushUndoOpr(LineAddition(layerIndex, index, asList lines))
        | CollectionChange.Remove(index, lines) -> parent.PushUndoOpr(LineRemoving(layerIndex, index, asList lines))
        | _ -> ())
    do layerPoints.Add(function
        | CollectionChange.Add(index, points) -> parent.PushUndoOpr(PointAddition(layerIndex, index, asList points))
        | CollectionChange.Remove(index, points) -> parent.PushUndoOpr(PointRemoving(layerIndex, index, asList points))
        | _ -> ())

    member _.Edges = init.Edges
    member _.Lines = layerLines :> IReactiveCollection<_>
    member _.Points = layerPoints :> IReactiveCollection<_>
    member _.LayerType = init.LayerType

    member _.GetSnapShot() = Layer.Create(init.Edges, layerLines, layerPoints, init.LayerType, init.OriginalEdges, init.Matrix)

    member this.AddLineCore(lines: seq<LineSegment>, addCross: bool) =
        let lines = [for l in lines do if not (Layer.hasSeg l this) then yield l]
        if lines <> [] then
            let points = if addCross then Layer.crossesAll lines this else []
            use __ = parent.TryBeginChange()
            layerLines.AddRange(lines)
            layerPoints.AddRange(points)

    member this.AddLinesRaw(lines: seq<Line>) = this.AddLineCore(lines |> Seq.collect (flip Layer.clip this), false)

    member this.AddLinesRaw(lines: seq<LineSegment>) = this.AddLineCore(lines |> Seq.collect (flip Layer.clipSeg this), false)

    member this.AddLines(lines: seq<Line>) = this.AddLineCore(lines |> Seq.collect (flip Layer.clip this), true)

    member this.AddLines(lines: seq<LineSegment>) = this.AddLineCore(lines |> Seq.collect (flip Layer.clipSeg this), true)

    member _.RemoveLines(count: int) =
        if count > 0 then
            use __ = parent.TryBeginChange()
            layerLines.RemoveTail(count)

    member this.AddPoints(points: seq<Point>) =
        let points = [for p in points do if Layer.containsPoint p this && not (Layer.hasPoint p this) then yield p ]
        if points <> [] then
            use __ = parent.TryBeginChange()
            layerPoints.AddRange(points)

    member _.RemovePoints(count: int) =
        if count > 0 then
            use __ = parent.TryBeginChange()
            layerPoints.RemoveTail(count)

    interface ILayer with
        member this.Edges = upcast this.Edges
        member this.Lines = upcast this.Lines
        member this.Points = upcast this.Points
        member _.LayerType = init.LayerType
        member _.OriginalEdges = upcast init.OriginalEdges
        member _.Matrix = init.Matrix

    interface ILayerModel with
        member this.Lines = this.Lines
        member this.Points = this.Points
        member this.GetSnapShot() = upcast (this.GetSnapShot())
        member this.AddLines(lines: seq<Line>) = this.AddLines(lines)
        member this.AddLines(lines: seq<LineSegment>) = this.AddLines(lines)
        member this.AddLinesRaw(lines: seq<Line>) = this.AddLinesRaw(lines)
        member this.AddLinesRaw(lines: seq<LineSegment>) = this.AddLinesRaw(lines)
        member this.RemoveLines(count: int) = this.RemoveLines(count)
        member this.AddPoints(points) = this.AddPoints(points)
        member this.RemovePoints(count) = this.RemovePoints(count)
