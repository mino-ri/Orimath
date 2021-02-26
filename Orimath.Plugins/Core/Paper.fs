namespace Orimath.Core
open System.Collections.Generic

type IPaper =
    abstract member Layers : IReadOnlyList<ILayer>


type Paper = internal Paper of layers: Layer list
    with
    member this.Layers = match this with Paper(layers) -> layers
    interface IPaper with
        member this.Layers =
            match this with
            | Paper(layer) -> layer :> IReadOnlyList<Layer> :?> IReadOnlyList<ILayer>


module Paper =
    let create (layers: seq<ILayer>) =
        let layers = [ for l in layers -> Layer.snapShot l ]
        if layers.Length < 1
        then invalidArg (nameof(layers)) "少なくとも1つのレイヤーが必要です。"
        else Paper(layers)

    let fromSize width height =
        Paper [ Layer.fromSize width height LayerType.BackSide ]

    let snapShot (source: IPaper) =
        match source with
        | :? Paper as paper -> paper
        | _ -> create source.Layers

    /// この紙の範囲内に収まるように、指定された直線をカットします。
    let clipBy (paper: IPaper) line =
        let mutable result = []
        paper.Layers
        |> Seq.collect (Layer.clip line)
        |> Seq.sortBy (fun l -> if line.YFactor = 0.0 then l.Point1.Y else l.Point1.X)
        |> Seq.iter (fun l ->
            match result with
            | [] -> result <- [l]
            | head :: tail ->
                if LineSegment.containsPoint l.Point1 head &&
                   not (LineSegment.containsPoint l.Point2 head) then
                    match LineSegment.FromPoints(head.Point1, l.Point2) with
                    | Some(newLine) -> result <- newLine :: tail
                    | None -> ()
                else
                    result <- l :: head :: tail)
        result

    /// この紙の範囲内に収まるように指定された直線をカットし、その両端の位置を返します。
    let clipBoundBy paper line =
        match clipBy paper line with
        | [] -> None
        | segments -> Some((List.last segments).Point1, segments.Head.Point2)
