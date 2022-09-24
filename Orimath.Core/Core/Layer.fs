namespace Orimath.Core
open Orimath
open Orimath.ApproximatelyEquatable


type [<Struct>] LayerType = BackSide | FrontSide


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module LayerType =
    /// LayerType を裏表逆転させます。
    let turnOver layerType =
        match layerType with
        | LayerType.FrontSide -> LayerType.BackSide
        | LayerType.BackSide -> LayerType.FrontSide


[<ReferenceEquality; NoComparison>]
type Layer =
    {
        Type: LayerType
        Polygon: Polygon
        OriginalPolygon: Polygon
        Creases: Crease list
        Points: Point list
        Matrix: Matrix
    }


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Layer =
    /// 指定した要素をもつレイヤーを生成します。
    let create polygon creases points layerType originalPolygon matrix : Result<Layer, Error> =
        if not (creases |> List.forall (fun (c: Crease) ->
            Polygon.containsPoint c.Point1 polygon && Polygon.containsPoint c.Point2 polygon)) then
            Error(Error.invalidLayerSegmentOutOfLayer)
        elif not (points |> List.forall (flip Polygon.containsPoint polygon)) then
            Error(Error.invalidLayerPointOutOfLayer)
        else
            Ok {
                Type = layerType
                Polygon = polygon
                OriginalPolygon = originalPolygon
                Creases = creases
                Points = points
                Matrix = matrix
            }
    
    /// 指定した多角形を持つレイヤーを生成します。
    let plain layerType (polygon: Polygon) =
        {
            Polygon = polygon
            OriginalPolygon = polygon
            Matrix = Matrix.identity
            Creases = []
            Points = Polygon.vertexes polygon
            Type = layerType
        }
    
    /// 指定した頂点を 多角形のレイヤーを生成します。
    let tryFromVertexes vertexes layerType =
        result {
            let! polygon = Polygon.tryFromVertexes vertexes
            return plain layerType polygon
        }
    
    /// 指定した高さと幅を持つ長方形のレイヤーを生成します。
    let fromSize width height layerType =
        plain layerType (Polygon.fromSize width height)
    
    /// このレイヤーの領域に指定した点が含まれているか判断します。
    let containsPoint point (layer: Layer) = Polygon.containsPoint point layer.Polygon
    
    /// このレイヤーの領域に指定した線分が完全に含まれているか判断します。
    let containsSeg (line: LineSegment) layer =
        containsPoint line.Point1 layer && containsPoint line.Point2 layer
    
    /// このレイヤーの領域に全ての点が含まれているか判断します。
    let containsAllPoint points layer = Seq.forall (flip containsPoint layer) points
    
    /// このレイヤーの領域に全ての線分が完全に含まれているか判断します。
    let containsAllSeg segs layer = Seq.forall (flip containsSeg layer) segs
    
    /// このレイヤーに、指定した点と同じ点が存在するか判断します。
    let hasPoint point (layer: Layer) = layer.Points |> Seq.exists ((=~) point)
    
    /// このレイヤーに、指定した線分と同じ線分が存在するか判断します。
    let hasSeg (seg: LineSegment) (layer: Layer) =
        layer.Polygon.Edges |> Seq.exists (fun e -> e.Line =~ seg.Line) ||
        layer.Creases |> Seq.exists (fun c -> LineSegment.containsSeg seg c.Segment)
    
    /// このレイヤーの範囲内に収まるように、指定された直線をカットします。
    let clip line (layer: Layer) = Polygon.clip line layer.Polygon
    
    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipSeg line (layer: Layer) = Polygon.clipSeg line layer.Polygon
    
    /// このレイヤーの範囲内に収まるように、指定された線分をカットします。
    let clipCrease crease (layer: Layer) = Polygon.clipCrease crease layer.Polygon
    
    /// このレイヤーに、指定した直線と同じ線分が存在するか判断します。
    let hasLine line layer = clip line layer |> Seq.forall (flip hasSeg layer)
        
    let private clipBoundCore (segments: seq<LineSegment>) : struct (Point * Point) voption =
        match Seq.tryHeadLast segments with
        | ValueNone -> ValueNone
        | ValueSome(head, last) -> ValueSome(head.Point1, last.Point2)
    
    /// このレイヤーの範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    let clipBound line layer = clip line layer |> LineSegment.merge |> clipBoundCore
        
    /// このレイヤーの範囲内に収まるように指定された線分をカットし、その両端の位置を返します。
    let clipBoundSeg seg layer = clipSeg seg layer |> LineSegment.merge |> clipBoundCore
     
    let private tryAddPoint points addingPoint layer =
        match addingPoint with
        | ValueSome(p) when List.forall ((<>~) p) points && not (hasPoint p layer) ->
            p :: points
        | _ -> points
    
    let private appendCross seg points (layer: Layer) =
        let points =
            (points, layer.Polygon.Edges)
            ||> Seq.fold (fun points edge -> tryAddPoint points (LineSegment.cross edge.Segment seg) layer)
        (points, layer.Creases)
        ||> Seq.fold (fun points crease -> tryAddPoint points (LineSegment.cross crease.Segment seg) layer)
            
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
        recSelf (Seq.asList segs) []
    
    /// このレイヤー内の全ての折線と、指定した全ての線分との交点を取得します。
    let crossesAllCrease (creases: seq<Crease>) layer =
        crossesAll (creases |> Seq.map (fun c -> c.Segment)) layer
    
    /// このレイヤーの OriginalEdges を辺として持ち、点・折線を持たないレイヤーを取得します。
    let original (layer: Layer) =
        {
            Polygon = layer.OriginalPolygon
            OriginalPolygon = layer.OriginalPolygon
            Points = []
            Type = layer.Type
            Creases = []
            Matrix = Matrix.identity
        }
    
    /// 指定されたレイヤーを開いた領域を結合した新しいレイヤーを生成します。
    let merge (layers: seq<Layer>) =
        result {
            let! polygon = layers |> Seq.map (fun ly -> ly.OriginalPolygon) |> Polygon.merge
            return plain LayerType.BackSide polygon
        }
    