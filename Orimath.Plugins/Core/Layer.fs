namespace Orimath.Core
open System.Collections.Generic
open System.Runtime.CompilerServices
open NearlyEquatable

type LayerType =
    | BackSide = 0
    | FrontSide = 1


[<Extension>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module LayerType =
    /// LayerType を裏表逆転させます。
    let turnOver layerType =
        match layerType with
        | LayerType.FrontSide -> LayerType.BackSide
        | LayerType.BackSide -> LayerType.FrontSide
        | _ -> layerType


type ILayer =
    abstract member Edges : IReadOnlyList<Edge>
    abstract member Creases : IReadOnlyList<Crease>
    abstract member Points : IReadOnlyList<Point>
    abstract member LayerType : LayerType
    abstract member OriginalEdges : IReadOnlyList<Edge>
    abstract member Matrix : Matrix


type Layer =
    internal
    | Layer of edges: Edge list
             * creases: Crease list
             * points: Point list
             * layerType: LayerType
             * originalEdges: Edge list
             * matrix: Matrix
    member this.Edges = match this with Layer(edge, _, _, _, _, _) -> edge
    member this.Creases = match this with Layer(_, crease, _, _, _, _) -> crease
    member this.Points = match this with Layer(_, _, points, _, _, _) -> points
    member this.LayerType = match this with Layer(_, _, _, layerType, _, _) -> layerType
    member this.OriginalEdges = match this with Layer(_, _, _, _, originalEdges, _) -> originalEdges
    member this.Matrix = match this with Layer(_, _, _, _, _, matrix) -> matrix
    interface ILayer with
        member this.Edges = upcast this.Edges
        member this.Creases = upcast this.Creases
        member this.Points = upcast this.Points
        member this.LayerType = this.LayerType
        member this.OriginalEdges = upcast this.OriginalEdges
        member this.Matrix = this.Matrix


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Layer =
    /// 指定した要素をもつレイヤーを生成します。
    let create edges creases points layerType originalEdges matrix =
        let edges = asList edges
        let creases = asList creases
        let points = asList points
        let originalEdges = asList originalEdges
        let rec isValidEdge (head: Edge) (edges: Edge list) =
            match edges with
            | [ e ] -> e.Segment.Point2 =~ head.Segment.Point1
            // 末尾最適化用に一応 if で分岐
            | e1 :: ((e2 :: _) as tail) ->
                if e1.Segment.Point2 <>~ e2.Segment.Point1 then false else isValidEdge head tail
            | [] -> failwith "想定しない動作"
        if edges.Length < 3 then
            invalidArg (nameof edges) "多角形の辺は3以上でなければなりません。"
        if not (isValidEdge edges.Head edges) then
            invalidArg (nameof edges) "多角形の辺は閉じている必要があります。"
        if originalEdges.Length < 3 then
            invalidArg (nameof originalEdges) "多角形の辺は3以上でなければなりません。"
        if not (isValidEdge originalEdges.Head originalEdges) then
            invalidArg (nameof originalEdges) "多角形の辺は閉じている必要があります。"
        if not (creases |> List.forall (fun (c: Crease) ->
            Edge.containsPoint c.Point1 edges && Edge.containsPoint c.Point2 edges)) then
            invalidArg (nameof creases) "レイヤー内に含まれていない線分があります。"
        if not (points |> List.forall (flip Edge.containsPoint edges)) then
            invalidArg (nameof points) "レイヤー内に含まれていない点があります。"
        Layer(edges, creases, points, layerType, originalEdges, matrix)

    /// 指定した頂点を持つ多角形のレイヤーを生成します。
    let fromPolygon (vertexes: seq<Point>) layerType =
        let vertexes = asList vertexes
        if vertexes.Length < 3 then
            invalidArg (nameof vertexes) "多角形の頂点は3以上でなければなりません。"
        let rec createEdges (head: Point) (points: Point list) (acm: Edge list) =
            match points with
            | [ p ] -> { Segment = LineSegment.FromPoints(head, p).Value; Inner = false } :: acm
            | p1 :: ((p2 :: _) as tail) ->
                createEdges head tail
                    ({ Segment = LineSegment.FromPoints(p2, p1).Value; Inner = false } :: acm)
            | [] -> acm
        let edges = createEdges vertexes.Head vertexes []
        Layer(edges, [], vertexes, layerType, edges, Matrix.Identity)

    /// 指定した高さと幅を持つ長方形のレイヤーを生成します。
    let fromSize width height layerType =
        if width <= 0.0 then
            invalidArg (nameof width) "紙の幅または高さを0以下にすることはできません。"
        if height <= 0.0 then
            invalidArg (nameof height) "紙の幅または高さを0以下にすることはできません。"
        let p0 = { X = 0.5 - width / 2.0; Y = 0.5 - height / 2.0 }
        let p1 = { X = 0.5 + width / 2.0; Y = 0.5 - height / 2.0 }
        let p2 = { X = 0.5 + width / 2.0; Y = 0.5 + height / 2.0 }
        let p3 = { X = 0.5 - width / 2.0; Y = 0.5 + height / 2.0 }
        fromPolygon [ p0; p1; p2; p3 ] layerType

    let snapShot (layer: ILayer) =
        match layer with
        | :? Layer as ly -> ly
        | _ ->
            Layer(asList layer.Edges, asList layer.Creases, asList layer.Points,
                layer.LayerType, asList layer.OriginalEdges, layer.Matrix)

    /// このレイヤーの領域に指定した点が含まれているか判断します。
    let containsPoint point (layer: ILayer) = Edge.containsPoint point (asList layer.Edges)

    /// このレイヤーの領域に指定した線分が完全に含まれているか判断します。
    let containsSeg (line: LineSegment) layer =
        containsPoint line.Point1 layer && containsPoint line.Point2 layer

    /// このレイヤーの領域に全ての点が含まれているか判断します。
    let containsAllPoint points layer = Seq.forall (flip containsPoint layer) points

    /// このレイヤーの領域に全ての線分が完全に含まれているか判断します。
    let containsAllSeg segs layer = Seq.forall (flip containsSeg layer) segs

    /// このレイヤーに、指定した点と同じ点が存在するか判断します。
    let hasPoint point (layer: ILayer) = layer.Points |> Seq.exists ((=~) point)

    /// このレイヤーに、指定した線分と同じ線分が存在するか判断します。
    let hasSeg (seg: LineSegment) (layer: ILayer) =
        layer.Edges |> Seq.exists (fun e -> e.Line =~ seg.Line) ||
        layer.Creases |> Seq.exists (fun c -> LineSegment.containsSeg seg c.Segment)

    /// このレイヤーの範囲内に収まるように、指定された直線をカットします。
    let clip line (layer: ILayer) = Edge.clip line (asList layer.Edges)

    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipSeg line (layer: ILayer) = Edge.clipSeg line (asList layer.Edges)

    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipCrease crease (layer: ILayer) = Edge.clipCrease crease (asList layer.Edges)

    /// このレイヤーに、指定した直線と同じ線分が存在するか判断します。
    let hasLine line layer = clip line layer |> Seq.forall (flip hasSeg layer)
        
    let private clipBoundCore (segments: LineSegment[]) =
        if segments.Length = 0
        then None
        else Some(segments[0].Point1, (Array.last segments).Point2)

    /// このレイヤーの範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    let clipBound line layer = clip line layer |> LineSegment.merge |> Seq.toArray |> clipBoundCore
        
    /// このレイヤーの範囲内に収まるように指定された線分をカットし、その両端の位置を返します。
    let clipBoundSeg seg layer = clipSeg seg layer |> LineSegment.merge |> Seq.toArray |> clipBoundCore
 
    let private tryAddPoint points addingPoint layer =
        match addingPoint with
        | Some(p) when List.forall ((<>~) p) points && not (hasPoint p layer) ->
            p :: points
        | _ -> points

    let private appendCross seg points (layer: ILayer) =
        let mutable points = points
        for edge in layer.Edges do
            points <- tryAddPoint points (LineSegment.cross edge.Segment seg) layer
        for crease in layer.Creases do
            points <- tryAddPoint points (LineSegment.cross crease.Segment seg) layer
        points
            
    /// このレイヤー内の全ての折線と、指定した線分との交点を取得します。
    let crosses seg layer = appendCross seg [seg.Point1; seg.Point2] layer
            
    /// このレイヤー内の全ての折線と、指定した全ての線分との交点を取得します。
    let crossesAll segs layer =
        let rec recSelf (lines: LineSegment list) points =
            let mutable points = points
            match lines with
            | line :: tail ->
                if List.forall ((<>~) line.Point1) points && not (hasPoint line.Point1 layer) then
                    points <- line.Point1 :: points
                if List.forall ((<>~) line.Point2) points && not (hasPoint line.Point2 layer) then
                    points <- line.Point2 :: points
                points <- appendCross line points layer
                for tailLine in tail do
                    points <- tryAddPoint points (LineSegment.cross tailLine line) layer
                recSelf tail points
            | _ -> points
        recSelf (asList segs) []

    /// このレイヤー内の全ての折線と、指定した全ての線分との交点を取得します。
    let crossesAllCrease (creases: seq<Crease>) layer =
        crossesAll (creases |> Seq.map (fun c -> c.Segment)) layer

    /// このレイヤーの OriginalEdges を辺として持ち、点・折線を持たないレイヤーを取得します。
    let original (layer: ILayer) =
        Layer(asList layer.OriginalEdges, [], [], layer.LayerType, asList layer.OriginalEdges, Matrix.Identity)

    /// 指定されたレイヤーを開いた領域を結合した新しいレイヤーを生成します。
    let merge (layers: seq<#ILayer>) =
        let edges =
            seq {
                for ly in layers do
                for e in ly.OriginalEdges do
                if not e.Inner then e.Segment
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
        fromPolygon points LayerType.BackSide
