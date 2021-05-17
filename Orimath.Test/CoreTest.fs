module Orimath.Test.CoreTest
open Xunit
open RightArrow
open Orimath.Core

[<Fact>]
let ``正常 紙に点が含まれる`` () =
    let point = { X = 0.75; Y = 0.4999999999999998 }
    let layer =
        Layer.fromPolygon
            [ { X = 0.0; Y = 0.0 }
              { X = 0.0; Y = 1.0 }
              { X = 0.5; Y = 1.0 }
              { X = 1.0; Y = 0.5 }
              { X = 0.5; Y = 0.0 } ]
            LayerType.BackSide
    test Layer.containsPoint (point, layer) ==> it ^= true

[<Fact>]
let ``正常 紙に点が含まれない`` () =
    let point = { X = 0.25; Y = 0.25 }
    let layer =
        Layer.fromPolygon
            [ { X = 0.625; Y = 0.2500000000000001 }
              { X = 0.625; Y = 0.125 }
              { X = 0.5; Y = 0.25 } ]
            LayerType.BackSide
    test Edge.containsPointWithoutOnEdges (point, layer.Edges) ==> it ^= false

