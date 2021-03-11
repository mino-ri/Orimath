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

    let containsPoint point edges =
        let rec recSelf acm (edges: Edge list) =
            match edges with
            | head :: tail ->
                let p1 = head.Point1
                let p2 = head.Point2
                if LineSegment.containsPoint point head.Segment then
                    true
                else
                    if (p1.Y <= point.Y && point.Y < p2.Y || p2.Y <= point.Y && point.Y < p1.Y) &&
                        point.X < Line.getX point.Y head.Segment.Line
                    then recSelf (acm + 1) tail
                    else recSelf acm tail
            | [] -> acm % 2 = 1
        recSelf 0 edges

    let containsPoint2 point (edges: LineSegment list) =
        let rec recSelf acm (edges: LineSegment list) =
            match edges with
            | head :: tail ->
                let p1 = head.Point1
                let p2 = head.Point2
                if LineSegment.containsPoint point head then
                    true
                else
                    if (p1.Y <= point.Y && point.Y < p2.Y || p2.Y <= point.Y && point.Y < p1.Y) &&
                        point.X < Line.getX point.Y head.Line
                    then recSelf (acm + 1) tail
                    else recSelf acm tail
            | [] -> acm % 2 = 1
        recSelf 0 edges

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
        edges1 |> Seq.forall (fun e1 -> containsPoint e1.Segment.Point1 edges2) ||
        edges2 |> Seq.forall (fun e2 -> containsPoint e2.Segment.Point1 edges1) ||
        exists {
            let! e1 = edges1
            let! e2 = edges2
            let! point = LineSegment.cross e1.Segment e2.Segment
            return point <>~ e1.Segment.Point1 && point <>~ e1.Segment.Point2 &&
                   point <>~ e2.Segment.Point1 && point <>~ e2.Segment.Point2
        }
