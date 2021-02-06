namespace Orimath.Core
open System.Collections.Generic

type IPaper =
    abstract member Layers : IReadOnlyList<ILayer>


module Paper =
    /// この紙の範囲内に収まるように、指定された直線をカットします。
    let clipBy (paper: IPaper) line =
        let mutable result = []
        paper.Layers
        |> Seq.collect(Layer.clip line)
        |> Seq.sortBy(fun l -> if line.YFactor = 0.0 then l.Point1.Y else l.Point1.X)
        |> Seq.iter(fun l ->
            match result with
            | [] -> result <- [l]
            | head :: tail ->
                if LineSegment.containsPoint l.Point1 head &&
                   not (LineSegment.containsPoint l.Point2 head) then
                    match LineSegment.FromPoints(head.Point1, l.Point2) with
                    | Some(newLine) -> result <- newLine :: tail
                    | None -> ()
                else result <- l :: head :: tail)
        result

    /// この紙の範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    let clipBoundBy paper line =
        match clipBy paper line with
        | [] -> None
        | segments -> Some((List.last segments).Point1, segments.Head.Point2)
