namespace Orimath.Core
open Orimath

type Paper = Paper of layers: NonEmptyList<Layer> with
    member this.LayersAsNonEmpty = match this with Paper(layers) -> layers
    member this.Layers = match this with Paper(NonEmptyList(layers)) -> layers


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Paper =
    let create firstLayer restLayers = Paper(NonEmptyList.create firstLayer restLayers)
    
    let tryCreate layers =
        result {
            let! lst = NonEmptyList.tryCreate layers
            return Paper(lst)
        }
    
    /// この紙の範囲内に収まるように、指定された直線をカットします。
    let clipBy (paper: Paper) line =
        paper.Layers
        |> Seq.collect (Layer.clip line)
        |> Seq.sortBy (fun l -> if line.YFactor = 0.0 then l.Point1.Y else l.Point1.X)
        |> Seq.fold (fun result l ->
            match result with
            | [] -> [l]
            | head :: tail ->
                if LineSegment.containsPoint l.Point1 head && not (LineSegment.containsPoint l.Point2 head) then
                    match LineSegment.tryFromPoints head.Point1 l.Point2 with
                    | Ok(newLine) -> newLine :: tail
                    | Error _ -> result
                else
                    l :: result) []
    
    /// この紙の範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    let clipBoundBy paper line =
        match clipBy paper line |> LineSegment.merge |> Seq.toList with
        | [] -> None
        | segments -> Some(segments.Head.Point1, (List.last segments).Point2)
    