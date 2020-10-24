namespace Orimath.Core
open System.Collections.Generic
open System.Runtime.CompilerServices
open NearlyEquatable

type IPaper =
    abstract member Layers : IReadOnlyList<ILayer>

[<Extension>]
type PaperExtensions =
    /// この紙の範囲内に収まるように、指定された直線をカットします。
    [<Extension>]
    static member Clip(paper: IPaper, line: Line) =
        let mutable result = []
        paper.Layers
        |> Seq.collect(fun l -> l.Clip(line))
        |> Seq.sortBy(fun l -> if line.YFactor = 0.0 then l.Point1.Y else l.Point1.X)
        |> Seq.iter(fun l ->
            match result with
            | [] -> result <- [l]
            | head :: tail ->
                if head.Contains(l.Point1) && not (head.Contains(l.Point2)) then
                    match LineSegment.FromPoints(head.Point1, l.Point2) with
                    | Some(newLine) -> result <- newLine :: tail
                    | None -> ()
                else result <- l :: head :: tail)
        result

    /// この紙の範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    [<Extension>]
    static member ClipBound(paper: IPaper, line: Line) =
        let segments = paper.Clip(line)
        if segments.IsEmpty then
            None
        else
            Some((List.last segments).Point1, segments.Head.Point2)
