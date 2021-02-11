namespace Orimath.Core
open NearlyEquatable
open Internal

type Edge = { Line: LineSegment; Inner: bool } with

    override this.ToString() = this.Line.ToString()

module Edge =
    let containsPoint point edges =
        let rec recSelf acm (edges: Edge list) =
            match edges with
            | head :: tail ->
                let p1 = head.Line.Point1
                let p2 = head.Line.Point2
                if LineSegment.containsPoint point head.Line then
                    true
                else
                    if (p1.Y <= point.Y && point.Y < p2.Y || p2.Y <= point.Y && point.Y < p1.Y) &&
                        point.X < Line.getX point.Y head.Line.Line
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
            match Line.cross edge.Line.Line line with
            | Some(p) when LineSegment.containsPoint p edge.Line &&
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
            match LineSegment.cross edge.Line line with
            | Some(p) when not (points |> Seq.exists ((=~) p)) -> points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy (fun p -> if line.Line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter (fun (p1, p2) -> containsPoint ((p1 + p2) / 2.0) edges)
        |> Seq.choose LineSegment.FromPoints

    /// 2つのレイヤーに重なっている領域があるか判定します。
    let areOverlap edges1 edges2 =
        edges1 |> Seq.forall (fun e1 -> containsPoint e1.Line.Point1 edges2) ||
        edges2 |> Seq.forall (fun e2 -> containsPoint e2.Line.Point1 edges1) ||
        exists {
            let! e1 = edges1
            let! e2 = edges2
            let! point = LineSegment.cross e1.Line e2.Line
            return point <>~ e1.Line.Point1 && point <>~ e1.Line.Point2 &&
                   point <>~ e2.Line.Point1 && point <>~ e2.Line.Point2
        }
