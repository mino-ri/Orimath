namespace Orimath.Core
open NearlyEquatable

type Edge private (line: LineSegment, layer: Layer option) =
    member __.Line = line
    member __.Layer = layer

    static member Create(line: LineSegment, layer: Layer option) =
        match layer with
        | Some(ly) ->
            if ly.Edges |> List.forall(fun (e: Edge) -> e.Line <>~ line) then
                invalidArg "layer" "接続先のレイヤーに共有する辺がありません。"
        | None -> ()
        Edge(line, layer)

and Layer private (edges: Edge list, lines: LineSegment list, points: Point list) =
    member __.Edges = edges
    member __.Lines = lines
    member __.Points = points

    static member private ContainsCore(edges, point: Point) =
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
            | [] -> acm % 2 = 0
        recSelf 0 edges

    /// このレイヤーの領域に指定した点が含まれているか判断します。
    member this.Contains(point) = Layer.ContainsCore(this.Edges, point)

    /// このレイヤーの領域に指定した線分が完全に含まれているか判断します。
    member this.Contains(line: LineSegment) = this.Contains(line.Point1) && this.Contains(line.Point2)

    /// このレイヤーの領域に全ての点が含まれているか判断します。
    member this.ContainsAll(points: seq<Point>) = points |> Seq.forall(this.Contains)

    /// このレイヤーの領域に全ての線分が完全に含まれているか判断します。
    member this.ContainsAll(lines: seq<LineSegment>) = lines |> Seq.forall(this.Contains)

    /// このレイヤーに、指定した点と同じ点が存在するか判断します。
    member this.HasPoint(point) = this.Points |> List.exists((=~) point)

    /// このレイヤーに、指定した線分と同じ線分が存在するか判断します。
    member layer.HasLine(line: LineSegment) =
        layer.Edges |> List.exists(fun e -> e.Line.Line =~ line.Line) ||
        layer.Lines |> List.exists(fun l -> l.Contains(line))

    /// このレイヤーに、指定した直線と同じ線分が存在するか判断します。
    member this.HasLine(line: Line) =
        this.Clip(line)
        |> Seq.forall(this.HasLine : LineSegment -> bool)

    /// このレイヤーの範囲内に収まるように、指定された直線をカットします。
    member this.Clip(line: Line) =
        let points = ResizeArray()
        for edge in this.Edges do
            match edge.Line.Line.GetCrossPoint(line) with
            | Some(p) when edge.Line.Contains(p) &&
                            not (points |> Seq.exists((=~) p))
                -> points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy(fun p -> if line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter(fun (p1, p2) -> this.Contains((p1 + p2) / 2.0))
        |> Seq.choose(LineSegment.FromPoints)
        
    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    member this.Clip(line: LineSegment) =
        let points = ResizeArray()
        points.Add(line.Point1)
        points.Add(line.Point2)
        for edge in this.Edges do
            match edge.Line.GetCrossPoint(line) with
            | Some(p) when not (points |> Seq.exists((=~) p)) -> points.Add(p)
            | _ -> ()
        points
        |> Seq.sortBy(fun p -> if line.Line.YFactor = 0.0 then p.Y else p.X)
        |> Seq.pairwise
        |> Seq.filter(fun (p1, p2) -> this.Contains((p1 + p2) / 2.0))
        |> Seq.choose(LineSegment.FromPoints)

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
        if not (lines |> List.forall(fun l -> Layer.ContainsCore(edges, l.Point1) && Layer.ContainsCore(edges, l.Point2)))
        then invalidArg "lines" "レイヤー内に含まれていない線分があります。"
        if not (points |> List.forall(fun p -> Layer.ContainsCore(edges, p)))
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
