namespace Orimath.Core
open System.Collections.Generic
open Orimath.Plugins

type internal PaperOpr =
    | BeginChangeBlock
    | Clear of before: LayerModel list * after: LayerModel list
    | LayerAddition of index: int * layers: LayerModel list
    | LayerRemoving of index: int * layers: LayerModel list
    | LayerReplace of layerIndex: int * before: LayerModel * after: LayerModel
    | LineAddition of layerIndex: int * index: int * lines: LineSegment list
    | LineRemoving of layerIndex: int * index: int * lines: LineSegment list
    | PointAddition of layerIndex: int * index: int * points: Point list
    | PointRemoving of layerIndex: int * index: int * points: Point list

and internal IInternalPaperModel =
    inherit IPaperModel
    abstract member PushUndoOpr : opr: PaperOpr -> unit

and LayerModel internal (parent: IInternalPaperModel, layerIndex: int, init: Layer) as this =
    let layerLines = ReactiveCollection<LineSegment>(this, init.Lines)
    let layerPoints = ReactiveCollection<Point>(this, init.Points)
    
    do layerLines.Changed.Add(function
        | CollectionChange.Add(index, lines) -> parent.PushUndoOpr(LineAddition(layerIndex, index, asList lines))
        | CollectionChange.Remove(index, lines) -> parent.PushUndoOpr(LineRemoving(layerIndex, index, asList lines))
        | _ -> ())
    do layerPoints.Changed.Add(function
        | CollectionChange.Add(index, points) -> parent.PushUndoOpr(PointAddition(layerIndex, index, asList points))
        | CollectionChange.Remove(index, points) -> parent.PushUndoOpr(PointRemoving(layerIndex, index, asList points))
        | _ -> ())

    member __.Edges = init.Edges
    member __.Lines = layerLines :> IReadOnlyList<_>
    member __.Points = layerPoints :> IReadOnlyList<_>
    member internal __.LayerIndex = layerIndex

    member __.GetSnapShot() = Layer.Create(init.Edges, layerLines, layerPoints)

    member this.AddLineCore(lines: seq<LineSegment>, addCross: bool) =
        let lines = lines |> Seq.filter(this.HasLine >> not) |> Seq.toList
        if lines <> [] then
            let points = if addCross then Layer.crossAll this lines else []
            use __ = parent.TryBeginChange()
            layerLines.Add(lines)
            layerPoints.Add(points)

    member this.AddLinesRaw(lines: seq<Line>) = this.AddLineCore(lines |> Seq.collect(this.Clip), false)

    member this.AddLinesRaw(lines: seq<LineSegment>) = this.AddLineCore(lines |> Seq.collect(this.Clip), false)

    member this.AddLines(lines: seq<Line>) = this.AddLineCore(lines |> Seq.collect(this.Clip), true)

    member this.AddLines(lines: seq<LineSegment>) = this.AddLineCore(lines |> Seq.collect(this.Clip), true)

    member this.RemoveLines(count: int) =
        if count > 0 then
            use __ = parent.TryBeginChange()
            layerLines.Remove(count)

    member this.AddPoints(points: seq<Point>) =
        let points = points |> Seq.filter(fun p -> this.Contains(p) && not (this.HasPoint(p))) |> Seq.toList
        if points <> [] then
            use __ = parent.TryBeginChange()
            layerPoints.Add(points)

    member this.RemovePoints(count: int) =
        if count > 0 then
            use __ = parent.TryBeginChange()
            layerPoints.Remove(count)

    member __.LineChanged = layerLines.Changed
    member __.PointChanged = layerPoints.Changed

    interface ILayer with
        member this.Edges = upcast this.Edges
        member this.Lines = this.Lines
        member this.Points = this.Points

    interface ILayerModel with
        [<CLIEvent>] member this.LineChanged = this.LineChanged
        [<CLIEvent>] member this.PointChanged = this.PointChanged
        member this.GetSnapShot() = upcast (this.GetSnapShot())
        member this.AddLines(lines: seq<Line>) = this.AddLines(lines)
        member this.AddLines(lines: seq<LineSegment>) = this.AddLines(lines)
        member this.AddLinesRaw(lines: seq<Line>) = this.AddLinesRaw(lines)
        member this.AddLinesRaw(lines: seq<LineSegment>) = this.AddLinesRaw(lines)
        member this.RemoveLines(count: int) = this.RemoveLines(count)
        member this.AddPoints(points) = this.AddPoints(points)
        member this.RemovePoints(count) = this.RemovePoints(count)
