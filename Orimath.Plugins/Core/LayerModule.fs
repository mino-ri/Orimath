[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Runtime.CompilerServices.Extension>]
module Orimath.Core.Layer
open System.Runtime.CompilerServices
open NearlyEquatable

[<CompiledName("ContainsCore")>]
let private containsCore edges point =
    let rec recSelf acm (edges: Edge list) =
        match edges with
        | head :: tail ->
            let p1 = head.Line.Point1
            let p2 = head.Line.Point2
            if LineSegment.contains head.Line point then
                true
            else
                if (p1.Y <= point.Y && point.Y < p2.Y || p2.Y <= point.Y && point.Y < p1.Y) &&
                    point.X < Line.getX head.Line.Line point.Y then
                    recSelf (acm + 1) tail
                else
                    recSelf acm tail
        | [] -> acm % 2 = 0
    recSelf 0 edges

[<CompiledName("AsList")>]
let private asList (s: seq<'a>) =
    match s with
    | :? list<'a> as lst -> lst
    | _ -> List.ofSeq s

let private ( @@ ) (s : seq<'a>) (lst: 'a list) =
    let mutable r = lst
    for item in s do r <- item :: r
    r

[<CompiledName("Contains"); Extension>]
let containsPoint (layer: Layer) point = containsCore layer.Edges point

[<CompiledName("Contains"); Extension>]
let containsPoints (layer: Layer) (points: seq<Point>) = Seq.forall (containsPoint layer) points

[<CompiledName("Contains"); Extension>]
let containsLine (layer: Layer) (line: LineSegment) = containsPoint layer line.Point1 && containsPoint layer line.Point2

[<CompiledName("Contains"); Extension>]
let containsLines (layer: Layer) (lines: seq<LineSegment>) = Seq.forall (containsLine layer) lines

[<CompiledName("HasPoint"); Extension>]
let hasPoint (layer: Layer) point = layer.Points |> List.exists(fun p -> p =~ point)

[<CompiledName("HasLine"); Extension>]
let hasLine (layer: Layer) line =
    layer.Edges |> List.exists(fun e -> e.Line.Line =~ line) ||
    layer.Lines |> List.exists(fun l ->
        if l.Line <>~ line then
            false
        else
            let p1, p2 =
                layer.Edges
                |> List.fold(fun (p1, p2) e -> 
                        p1 || LineSegment.contains e.Line l.Point1,
                        p2 || LineSegment.contains e.Line l.Point2) (false, false)
            p1 && p2)

[<CompiledName("HasLineSegment"); Extension>]
let hasLineSeg (layer: Layer) (line: LineSegment) =
    layer.Edges |> List.exists(fun e -> e.Line.Line =~ line.Line) ||
    layer.Lines |> List.exists(fun l -> LineSegment.containsSeg l line)

// ======== ↓coreに移動する↓ ========
[<CompiledName("Clip"); Extension>]
let clip (layer: Layer) (line: Line) =
    let points = ResizeArray()
    for edge in layer.Edges do
        match Line.cross edge.Line.Line line with
        | Some(p) when LineSegment.contains edge.Line p &&
                       not (points |> Seq.exists((=~) p))
            -> points.Add(p)
        | _ -> ()
    points
    |> Seq.sortBy(fun p -> if line.B = 0.0 then p.Y else p.X)
    |> Seq.pairwise
    |> Seq.filter(fun (p1, p2) -> containsPoint layer ((p1 + p2) / 2.0))
    |> Seq.choose(LineSegment.FromPoints)

[<CompiledName("Clip"); Extension>]
let clipSeg (layer: Layer) (line: LineSegment) =
    let points = ResizeArray()
    points.Add(line.Point1)
    points.Add(line.Point2)
    for edge in layer.Edges do
        match LineSegment.cross edge.Line line with
        | Some(p) when not (points |> Seq.exists((=~) p)) -> points.Add(p)
        | _ -> ()
    points
    |> Seq.sortBy(fun p -> if line.Line.B = 0.0 then p.Y else p.X)
    |> Seq.pairwise
    |> Seq.filter(fun (p1, p2) -> containsPoint layer ((p1 + p2) / 2.0))
    |> Seq.choose(LineSegment.FromPoints)

[<CompiledName("AppendCrosses")>]
let private appendCross (layer: Layer) (line: LineSegment) (points: Point list) =
    let mutable points = points
    for edge in layer.Edges do
        match LineSegment.cross edge.Line line with
        | Some(p) when not (points |> List.exists((=~) p)) && not (hasPoint layer p) ->
            points <- p :: points
        | _ -> ()
    points

[<CompiledName("GetCrossesFSharpList"); Extension>]
let cross (layer: Layer) (line: LineSegment) = appendCross layer line []

[<CompiledName("GetCrosses"); Extension>]
let crossArray layer line = cross layer line |> List.toArray

[<CompiledName("AddCore"); Extension>]
let add layer lines points =
    if not (containsLines layer lines) then invalidArg "lines" "レイヤー内に含まれていない線分があります。"
    if not (containsPoints layer points) then invalidArg "points" "レイヤー内に含まれていない点があります。"
    Layer(layer.Edges, lines @@ layer.Lines, points @@ layer.Points)

[<CompiledName("AddPoints"); Extension>]
let addPoints layer points = add layer [] points

[<CompiledName("AddLinesRaw"); Extension>]
let addLinesRaw layer lines = add layer lines []

[<CompiledName("AddLine"); Extension>]
let addLine layer (line: LineSegment) = add layer [line] (cross layer line)
// ======== ↑coreに移動する↑ ========

[<CompiledName("CreateEdge")>]
let internal createEdge (line: LineSegment) (layer: Layer option) =
    match layer with
    | Some(ly) ->
        if ly.Edges |> List.forall(fun e -> e.Line <>~ line) then
            invalidArg "layer" "接続先のレイヤーに共有する辺がありません。"
    | None -> ()

[<CompiledName("Create")>]
let create (edges: seq<Edge>) (lines: seq<LineSegment>) (points: seq<Point>) =
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
    if not (lines |> List.forall(fun l -> containsCore edges l.Point1 && containsCore edges l.Point2))
    then invalidArg "lines" "レイヤー内に含まれていない線分があります。"
    if not (points |> List.forall(containsCore edges))
    then invalidArg "points" "レイヤー内に含まれていない点があります。"
    Layer(edges, lines, points)

[<CompiledName("FromSize")>]
let fromSize (width: float) (height: float) =
    if width <= 0.0 then invalidArg "width" "紙の幅または高さを0以下にすることはできません。"
    if height <= 0.0 then invalidArg "height" "紙の幅または高さを0以下にすることはできません。"
    let p0 = { X = 0.0; Y = 0.0 }
    let p1 = { X = width; Y = 0.0 }
    let p2 = { X = width; Y = height }
    let p3 = { X = 0.0; Y = height }
    let edges = [
            Edge(LineSegment.FromPoints(p0, p1).Value, None)
            Edge(LineSegment.FromPoints(p1, p2).Value, None)
            Edge(LineSegment.FromPoints(p2, p3).Value, None)
            Edge(LineSegment.FromPoints(p3, p0).Value, None)
        ]
    let points = [ p0; p1; p2; p3 ]
    Layer(edges, [], points)
