namespace Orimath.Core

type Paper private (layers: Layer list) =
    member __.Layers = layers

    static member Create(layers: seq<Layer>) =
        let layers = asList layers
        if layers.Length < 1
        then invalidArg "layers" "少なくとも1つのレイヤーが必要です。"
        else Paper(layers)

    static member FromSize(width: float, height: float) = Paper([ Layer.FromSize(width, height) ])
