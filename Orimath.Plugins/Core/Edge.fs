namespace Orimath.Core
open NearlyEquatable
open Internal

type Edge = { Segment: LineSegment; Inner: bool } with
    member this.Line = this.Segment.Line
    member this.Point1 = this.Segment.Point1
    member this.Point2 = this.Segment.Point2
    member this.Length = this.Segment.Length
    override this.ToString() = this.Segment.ToString()

module Edge =
    /// 現在の線分を、指定した直線で反転させます。
    let reflectBy line (edge: Edge) =
        let p1 = Point.reflectBy line edge.Segment.Point1
        let p2 = Point.reflectBy line edge.Segment.Point2
        { edge with Segment = LineSegment(Line.FromPoints(p1, p2).Value, p1, p2) }

    let private containsPointCore point edges withOnEdges =
        let rec recSelf acm (edges: Edge list) =
            match edges with
            | [] ->
                match acm with
                | head :: tail -> tail |> List.forall ((=) head)
                | [] -> true
            | head :: _ when LineSegment.containsPoint point head.Segment ->
                withOnEdges
            | head :: tail ->
                let v1 = point - head.Point1
                let v2 = head.Point2 - head.Point1
                let dot = v1.X * v2.Y - v2.X * v1.Y
                let nextAcm = if dot =~~ 0.0 then acm else ((dot < 0.0) :: acm)
                recSelf nextAcm tail
        recSelf [] edges

    let containsPoint point edges = containsPointCore point edges true

    let containsPointWithoutOnEdges point edges = containsPointCore point edges false

    /// このレイヤーの領域に指定した線分が完全に含まれているか判断します。
    let containsSeg (seg: LineSegment) edges =
        containsPoint seg.Point1 edges && containsPoint seg.Point2 edges
    
    /// このレイヤーの範囲内に収まるように、指定された直線をカットします。
    let clip line (edges: Edge list) =
        let points = ResizeArray()
        for edge in edges do
            match Line.cross edge.Segment.Line line with
            | Some(p) when LineSegment.containsPoint p edge.Segment &&
                           not (Seq.exists ((=~) p) points) ->
                points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy (fun p -> if line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter (fun (p1, p2) -> containsPoint ((p1 + p2) / 2.0) edges)
        |> Seq.choose LineSegment.FromPoints

    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipSeg (line: LineSegment) (edges: Edge list) =
        let points = ResizeArray()
        points.Add(line.Point1)
        points.Add(line.Point2)
        for edge in edges do
            match LineSegment.cross edge.Segment line with
            | Some(p) when not (points |> Seq.exists ((=~) p)) -> points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy (fun p -> if line.Line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter (fun (p1, p2) -> containsPoint ((p1 + p2) / 2.0) edges)
        |> Seq.choose LineSegment.FromPoints

    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipCrease (crease: Crease) edges =
        clipSeg crease.Segment edges |> Crease.ofSegs crease.Type

    /// 2つのレイヤーに重なっている領域があるか判定します。
    let areOverlap edges1 edges2 =
        edges1 |> Seq.exists (fun e1 -> containsPointWithoutOnEdges e1.Segment.Point1 edges2) ||
        edges2 |> Seq.exists (fun e2 -> containsPointWithoutOnEdges e2.Segment.Point1 edges1) ||
        edges1 |> Seq.forall (fun e1 -> containsPoint e1.Segment.Point1 edges2) ||
        edges2 |> Seq.forall (fun e2 -> containsPoint e2.Segment.Point1 edges1) ||
        exists {
            let! e1 = edges1
            let! e2 = edges2
            let! point = LineSegment.cross e1.Segment e2.Segment
            return point <>~ e1.Segment.Point1 && point <>~ e1.Segment.Point2 &&
                   point <>~ e2.Segment.Point1 && point <>~ e2.Segment.Point2
        }
