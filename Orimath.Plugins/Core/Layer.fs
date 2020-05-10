namespace Orimath.Core
open System.Collections.Generic
open System.Runtime.CompilerServices
open NearlyEquatable

type ILayer =
    abstract member Edges : IReadOnlyList<Edge>
    abstract member Lines : IReadOnlyList<LineSegment>
    abstract member Points : IReadOnlyList<Point>

and Edge private (line: LineSegment, layer: ILayer option) =
    member __.Line = line
    member __.Layer = layer

    static member Create(line: LineSegment, layer: ILayer option) =
        match layer with
        | Some(ly) ->
            if ly.Edges |> Seq.forall(fun (e: Edge) -> e.Line <>~ line) then
                invalidArg "layer" "接続先のレイヤーに共有する辺がありません。"
        | None -> ()
        Edge(line, layer)

[<Extension>]
type LayerExtensions =
    static member ContainsCore(edges, point: Point) =
        let rec recSelf acm (edges: Edge list) =
            match edges with
            | head :: tail ->
                let p1 = head.Line.Point1
                let p2 = head.Line.Point2
                if head.Line.Contains(point) then
                    true
                else
                    if (p1.Y <= point.Y && point.Y < p2.Y || p2.Y <= point.Y && point.Y < p1.Y) &&
                        point.X < head.Line.Line.GetX(point.Y) then
                        recSelf (acm + 1) tail
                    else
                        recSelf acm tail
            | [] -> acm % 2 = 1
        recSelf 0 edges

    /// このレイヤーの領域に指定した点が含まれているか判断します。
    [<Extension>]
    static member Contains(layer: ILayer, point) = LayerExtensions.ContainsCore(asList layer.Edges, point)

    /// このレイヤーの領域に指定した線分が完全に含まれているか判断します。
    [<Extension>]
    static member Contains(layer: ILayer, line: LineSegment) = layer.Contains(line.Point1) && layer.Contains(line.Point2)

    /// このレイヤーの領域に全ての点が含まれているか判断します。
    [<Extension>]
    static member ContainsAll(layer: ILayer, points: seq<Point>) = points |> Seq.forall(layer.Contains)

    /// このレイヤーの領域に全ての線分が完全に含まれているか判断します。
    [<Extension>]
    static member ContainsAll(layer: ILayer, lines: seq<LineSegment>) = lines |> Seq.forall(layer.Contains)

    /// このレイヤーに、指定した点と同じ点が存在するか判断します。
    [<Extension>]
    static member HasPoint(layer: ILayer, point) = layer.Points |> Seq.exists((=~) point)

    /// このレイヤーに、指定した線分と同じ線分が存在するか判断します。
    [<Extension>]
    static member HasLine(layer: ILayer, line: LineSegment) =
        layer.Edges |> Seq.exists(fun e -> e.Line.Line =~ line.Line) ||
        layer.Lines |> Seq.exists(fun l -> l.Contains(line))

    /// このレイヤーに、指定した直線と同じ線分が存在するか判断します。
    [<Extension>]
    static member HasLine(layer: ILayer, line: Line) =
        layer.Clip(line)
        |> Seq.forall(layer.HasLine : LineSegment -> bool)

    /// このレイヤーの範囲内に収まるように、指定された直線をカットします。
    [<Extension>]
    static member Clip(layer: ILayer, line: Line) =
        let points = ResizeArray()
        for edge in layer.Edges do
            match edge.Line.Line.GetCrossPoint(line) with
            | Some(p) when edge.Line.Contains(p) && not (points |> Seq.exists((=~) p))
                -> points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy(fun p -> if line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter(fun (p1, p2) -> layer.Contains((p1 + p2) / 2.0))
        |> Seq.choose(LineSegment.FromPoints)
        
    /// このレイヤーの範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    [<Extension>]
    static member ClipBound(layer: ILayer, line: Line) =
        let segments = layer.Clip(line) |> Seq.toArray
        if segments.Length = 0 then
            None
        else
            Some(segments.[0].Point1, segments.[segments.Length - 1].Point2)

    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    [<Extension>]
    static member Clip(layer: ILayer, line: LineSegment) =
        let points = ResizeArray()
        points.Add(line.Point1)
        points.Add(line.Point2)
        for edge in layer.Edges do
            match edge.Line.GetCrossPoint(line) with
            | Some(p) when not (points |> Seq.exists((=~) p)) -> points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy(fun p -> if line.Line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter(fun (p1, p2) -> layer.Contains((p1 + p2) / 2.0))
        |> Seq.choose(LineSegment.FromPoints)
        
    /// このレイヤーの範囲内に収まるように指定された線分をカットし、その両端の位置を返します。
    [<Extension>]
    static member ClipBound(layer: ILayer, line: LineSegment) =
        let segments = layer.Clip(line) |> Seq.toArray
        if segments.Length = 0 then
            None
        else
            Some(segments.[0].Point1, segments.[segments.Length - 1].Point2)
