namespace Orimath.Core

type Layer private (edges: Edge list, lines: LineSegment list, points: Point list) =
    member _.Edges = edges
    member _.Lines = lines
    member _.Points = points

    interface ILayer with
        member _.Edges = upcast edges
        member _.Lines = upcast lines
        member _.Points = upcast points        

    /// 指定した要素をもつレイヤーを生成します。
    static member Create(edges: seq<Edge>, lines: seq<LineSegment>, points: seq<Point>) =
        let edges = asList edges
        let lines = asList lines
        let points = asList points
        if edges.Length < 3 then invalidArg "edges" "多角形の辺は3以上でなければなりません。"
        let rec isValidEdge (head: Edge) (edges: Edge list) =
            match edges with
            | [ e ] -> e.Line.Point2 = head.Line.Point1
            // 末尾最適化用に一応 if で分岐
            | e1 :: ((e2 :: _) as tail) -> if e1.Line.Point2 <> e2.Line.Point1 then false else isValidEdge head tail
            | [] -> failwith "想定しない動作"
        if not (isValidEdge edges.Head edges) then invalidArg "edges" "多角形の辺は閉じている必要があります。"
        if not (lines |> List.forall(fun l -> LayerExtensions.ContainsCore(edges, l.Point1) && LayerExtensions.ContainsCore(edges, l.Point2)))
        then invalidArg "lines" "レイヤー内に含まれていない線分があります。"
        if not (points |> List.forall(fun p -> LayerExtensions.ContainsCore(edges, p)))
        then invalidArg "points" "レイヤー内に含まれていない点があります。"
        Layer(edges, lines, points)

    /// 指定した高さと幅を持つ長方形のレイヤーを生成します。
    static member FromSize(width: float, height: float) =
        if width <= 0.0 then invalidArg "width" "紙の幅または高さを0以下にすることはできません。"
        if height <= 0.0 then invalidArg "height" "紙の幅または高さを0以下にすることはできません。"
        let p0 = { X = 0.0; Y = 0.0 }
        let p1 = { X = width; Y = 0.0 }
        let p2 = { X = width; Y = height }
        let p3 = { X = 0.0; Y = height }
        Layer.FromPolygon([ p0; p1; p2; p3 ])

    /// 指定した頂点を持つ多角形のレイヤーを生成します。
    static member FromPolygon(vertexes: seq<Point>) =
        let vertexes = asList vertexes
        if vertexes.Length < 3 then invalidArg "vertexes" "多角形の頂点は3以上でなければなりません。"
        let rec createEdges (head: Point) (points: Point list) (acm: Edge list) =
            match points with
            | [ p ] -> Edge.Create(LineSegment.FromPoints(head, p).Value, None) :: acm
            | p1 :: ((p2 :: _) as tail) -> createEdges head tail (Edge.Create(LineSegment.FromPoints(p2, p1).Value, None) :: acm)
            | [] -> acm
        Layer(createEdges vertexes.Head vertexes [], [], vertexes)

    static member AsLayer(source: ILayer) =
        match source with
        | :? Layer as ly -> ly
        | _ -> Layer(asList source.Edges, asList source.Lines, asList source.Points)
