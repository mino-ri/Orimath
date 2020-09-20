namespace Orimath.Core
open System.Collections.Generic

type Paper private (layers: Layer list) =
    member _.Layers = layers

    interface IPaper with
        member _.Layers = layers :> IReadOnlyList<Layer> :?> IReadOnlyList<ILayer>

    static member Create(layers: seq<ILayer>) =
        let layers = layers |> Seq.map Layer.AsLayer |> Seq.toList
        if layers.Length < 1
        then invalidArg "layers" "少なくとも1つのレイヤーが必要です。"
        else Paper(layers)

    static member FromSize(width: float, height: float) = Paper([ Layer.FromSize(width, height) ])

    static member AsPaper(source: IPaper) =
        match source with
        | :? Paper as p -> p
        | _ -> Paper.Create(source.Layers)
