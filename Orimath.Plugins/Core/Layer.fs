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


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Layer =

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
        else Some(segments.[0].Point1, (Array.last segments).Point2)

    /// このレイヤーの範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    let clipBound line layer = clip line layer |> Seq.toArray |> clipBoundCore
        
    /// このレイヤーの範囲内に収まるように指定された線分をカットし、その両端の位置を返します。
    let clipBoundSeg seg layer = clipSeg seg layer |> Seq.toArray |> clipBoundCore
 
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
        { new ILayer with
            member _.Edges = layer.OriginalEdges
            member _.Creases = upcast []
            member _.Points = upcast []
            member _.LayerType = layer.LayerType
            member _.OriginalEdges = layer.OriginalEdges
            member _.Matrix = Matrix.Identity }
