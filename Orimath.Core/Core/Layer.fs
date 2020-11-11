namespace Orimath.Core
open Orimath.Core.NearlyEquatable

type Layer private (edges: Edge list, lines: LineSegment list, points: Point list, layerType: LayerType, originalEdges: Edge list, matrix: Matrix) =
    member _.Edges = edges
    member _.Lines = lines
    member _.Points = points
    member _.LayerType = layerType
    member _.OriginalEdges = originalEdges
    member _.Matrix = matrix

    interface ILayer with
        member _.Edges = upcast edges
        member _.Lines = upcast lines
        member _.Points = upcast points        
        member _.LayerType = layerType
        member _.OriginalEdges = upcast originalEdges
        member _.Matrix = matrix

    /// 指定した要素をもつレイヤーを生成します。
    static member Create(edges: seq<Edge>, lines: seq<LineSegment>, points: seq<Point>, layerType, originalEdges: seq<Edge>, matrix: Matrix) =
        let edges = asList edges
        let lines = asList lines
        let points = asList points
        let originalEdges = asList originalEdges
        let rec isValidEdge (head: Edge) (edges: Edge list) =
            match edges with
            | [ e ] -> e.Line.Point2 =~ head.Line.Point1
            // 末尾最適化用に一応 if で分岐
            | e1 :: ((e2 :: _) as tail) -> if e1.Line.Point2 <>~ e2.Line.Point1 then false else isValidEdge head tail
            | [] -> failwith "想定しない動作"
        if edges.Length < 3 then invalidArg (nameof(edges)) "多角形の辺は3以上でなければなりません。"
        if not (isValidEdge edges.Head edges) then invalidArg (nameof(edges)) "多角形の辺は閉じている必要があります。"
        if originalEdges.Length < 3 then invalidArg (nameof(originalEdges)) "多角形の辺は3以上でなければなりません。"
        if not (isValidEdge originalEdges.Head originalEdges) then invalidArg (nameof(originalEdges)) "多角形の辺は閉じている必要があります。"
        // if not (lines |> List.forall(fun l -> LayerExtensions.ContainsCore(edges, l.Point1) && LayerExtensions.ContainsCore(edges, l.Point2)))
        // then invalidArg (nameof(lines)) "レイヤー内に含まれていない線分があります。"
        // if not (points |> List.forall(fun p -> LayerExtensions.ContainsCore(edges, p)))
        // then invalidArg (nameof(points)) "レイヤー内に含まれていない点があります。"
        Layer(edges, lines, points, layerType, originalEdges, matrix)

    /// 指定した高さと幅を持つ長方形のレイヤーを生成します。
    static member FromSize(width, height, layerType) =
        if width <= 0.0 then invalidArg (nameof(width)) "紙の幅または高さを0以下にすることはできません。"
        if height <= 0.0 then invalidArg (nameof(height)) "紙の幅または高さを0以下にすることはできません。"
        let p0 = { X = 0.0; Y = 0.0 }
        let p1 = { X = width; Y = 0.0 }
        let p2 = { X = width; Y = height }
        let p3 = { X = 0.0; Y = height }
        Layer.FromPolygon([ p0; p1; p2; p3 ], layerType)

    /// 指定した頂点を持つ多角形のレイヤーを生成します。
    static member FromPolygon(vertexes: seq<Point>, layerType) =
        let vertexes = asList vertexes
        if vertexes.Length < 3 then invalidArg (nameof(vertexes)) "多角形の頂点は3以上でなければなりません。"
        let rec createEdges (head: Point) (points: Point list) (acm: Edge list) =
            match points with
            | [ p ] -> Edge(LineSegment.FromPoints(head, p).Value, false) :: acm
            | p1 :: ((p2 :: _) as tail) -> createEdges head tail (Edge(LineSegment.FromPoints(p2, p1).Value, false) :: acm)
            | [] -> acm
        let edges = createEdges vertexes.Head vertexes []
        Layer(edges, [], vertexes, layerType, edges, Matrix.Identity)

    static member AsLayer(source: ILayer) =
        match source with
        | :? Layer as ly -> ly
        | _ -> Layer(asList source.Edges, asList source.Lines, asList source.Points, source.LayerType, asList source.OriginalEdges, source.Matrix)
