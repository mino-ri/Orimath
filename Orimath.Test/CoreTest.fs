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
