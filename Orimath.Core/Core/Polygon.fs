namespace Orimath.Core
open Orimath
open Orimath.ApproximatelyEquatable

[<Struct; NoEquality; NoComparison>]
type Polygon = private Polygon of edges: Edge list with
    member this.Edges = match this with Polygon(edges) -> edges


[<AutoOpen>]
module PolygonOperator =
    let (|Polygon|) ((Polygon edges)) = edges


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Polygon =
    let vertexes (polygon: Polygon) = polygon.Edges |> List.map (fun e -> e.Point1)
        
    /// 指定した辺を持つ多角形を生成します。
    let tryFromEdges (edges: Edge list) =
        match edges with
        | [] | [ _ ] | [ _; _ ] -> Error(Error.invalidLayerVertexesLessThan3)
        | head :: tail ->
            let rec isValid (first: Edge) (edges: Edge list) =
                match edges with
                | [] -> true
                | [ edge ] -> edge.Point2 = first.Point1
                | edge1 :: ((edge2 :: _) as tail) ->
                    if edge1.Point2 <> edge2.Point1 then false
                    else
                        isValid first tail
            if isValid head tail then
                Ok(Polygon(edges))
            else
                Error(Error.invalidLayerPolygonMustBeClosed)

    /// 指定した頂点を持つ多角形のレイヤーを生成します。
    let tryFromVertexes (vertexes: seq<Point>) =
        let edges =
            vertexes
            |> Seq.pairwiseOfNotEqual
            |> Seq.map (fun points -> Edge(EdgeType.Outside, LineSegment.fromPoints points))
            |> Seq.toList
        if edges.Length < 3 then
            Error(Error.invalidLayerVertexesLessThan3)
        else
            Ok(Polygon(edges))
    
    /// 指定した高さと幅を持つ長方形のレイヤーを生成します。
    let fromSize (width: Positive<float>) (height: Positive<float>) =
        let width = float width
        let height = float height
        let scale = max width height
        let width = width / scale
        let height = height / scale
        let p0 = Point(0.5 - width / 2.0, 0.5 - height / 2.0)
        let p1 = Point(0.5 + width / 2.0, 0.5 - height / 2.0)
        let p2 = Point(0.5 + width / 2.0, 0.5 + height / 2.0)
        let p3 = Point(0.5 - width / 2.0, 0.5 + height / 2.0)
        tryFromVertexes [ p0; p1; p2; p3 ] |> Result.forceOk
    
    let square =
        let p0 = Point(0.0, 0.0)
        let p1 = Point(0.1, 0.0)
        let p2 = Point(0.1, 1.0)
        let p3 = Point(0.0, 1.0)
        tryFromVertexes [ p0; p1; p2; p3 ] |> Result.forceOk
    
    let private containsPointCore point (polygon: Polygon) withOnEdges =
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
        recSelf [] polygon.Edges
    
    /// このレイヤー領域に点が含まれているか判断します。辺上の点はレイヤーに含まれます。
    let containsPoint point edges = containsPointCore point edges true
    
    /// このレイヤー領域に点が含まれているか判断します。辺上の点はレイヤーに含まれません。
    let containsPointWithoutOnEdges point edges = containsPointCore point edges false
    
    /// このレイヤーの領域に指定した線分が完全に含まれているか判断します。
    let containsSeg (seg: LineSegment) edges =
        containsPoint seg.Point1 edges && containsPoint seg.Point2 edges
    
    /// このレイヤーの範囲内に収まるように、指定された直線をカットします。
    let clip line (polygon: Polygon) =
        let points = ResizeArray()
        for edge in polygon.Edges do
            match Line.cross edge.Segment.Line line with
            | ValueSome(p) when LineSegment.containsPoint p edge.Segment &&
                                not (Seq.exists ((=~) p) points) ->
                points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy (fun p -> if line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter (fun (p1, p2) -> containsPoint ((p1 + p2) / 2.0) polygon)
        |> Seq.rchoose (fun (p1, p2) -> LineSegment.tryFromPoints p1 p2)
    
    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipSeg (line: LineSegment) (polygon: Polygon) =
        let points = ResizeArray()
        points.Add(line.Point1)
        points.Add(line.Point2)
        for edge in polygon.Edges do
            match LineSegment.cross edge.Segment line with
            | ValueSome(p) when not (points |> Seq.exists ((=~) p)) -> points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy (fun p -> if line.Line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter (fun (p1, p2) -> containsPoint ((p1 + p2) / 2.0) polygon)
        |> Seq.rchoose (fun (p1, p2) -> LineSegment.tryFromPoints p1 p2)
    
    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipCrease (crease: Crease) edges =
        clipSeg crease.Segment edges |> Crease.ofSegs crease.Type
    
    /// 2つのレイヤーに重なっている領域があるか判定します。
    let areOverlap (polygon1: Polygon) (polygon2: Polygon) =
        polygon1.Edges |> Seq.exists (fun e1 -> containsPointWithoutOnEdges e1.Segment.Point1 polygon2) ||
        polygon2.Edges |> Seq.exists (fun e2 -> containsPointWithoutOnEdges e2.Segment.Point1 polygon1) ||
        polygon1.Edges |> Seq.forall (fun e1 -> containsPoint e1.Segment.Point1 polygon2) ||
        polygon2.Edges |> Seq.forall (fun e2 -> containsPoint e2.Segment.Point1 polygon1) ||
        exists {
            let! e1 = polygon1.Edges
            let! e2 = polygon2.Edges
            let! point = LineSegment.cross e1.Segment e2.Segment
            return point <>~ e1.Segment.Point1 && point <>~ e1.Segment.Point2 &&
                   point <>~ e2.Segment.Point1 && point <>~ e2.Segment.Point2
        }
    
    /// 指定されたレイヤーを開いた領域を結合した新しいレイヤーを生成します。
    let merge (polygons: seq<Polygon>) =
        let edges =
            seq {
                for p in polygons do
                for e in p.Edges do
                if e.IsOutside then e.Segment
            }
            |> LineSegment.merge
            |> ResizeArray
        let points = ResizeArray()
        let mutable currentPoint = edges[0].Point1
        while edges.Count > 0 do
            let index =
                edges
                |> Seq.findIndex (fun e -> e.Point1 =~ currentPoint || e.Point2 =~ currentPoint)
            let target = edges[index]
            currentPoint <- if target.Point1 =~ currentPoint then target.Point2 else target.Point1
            points.Add(currentPoint)
            edges.RemoveAt(index)
        tryFromVertexes points
    