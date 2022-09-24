module Orimath.Test.NonZeroPoint

open System
open Orimath.Core
open Testexp

[<Fact>]
let ``fromRadians.ゼロにならない`` () =
    testing {
        let! radians = ArgGen.floatRange 0.0 Math.PI
        test NonZeroPoint.fromRadians radians ==> Assert.notEqual Unchecked.defaultof<NonZeroPoint>
    }

[<Fact>]
let ``dist.異なる2点間の距離`` () =
    testing {
        let! point1 = ArgGen.nonZeroPointIn 0.0 1.0
        let! point2 = ArgGen.nonZeroPointIn -2.0 -1.0
        test NonZeroPoint.dist (point1, point2) ==> Assert.greaterThan 0.0
    }

[<Fact>]
let ``dist.同一の点同士の距離は0`` () =
    testing {
        let! point = ArgGen.nonZeroPoint
        test NonZeroPoint.dist (point, point) ==> Assert.aprxEqualf 0.0
    }

[<Fact>]
let ``norm.常に正の値`` () =
    testing {
        let! point = ArgGen.nonZeroPoint
        test NonZeroPoint.norm point ==> Assert.greaterThan 0.0
    }

[<Fact>]
let ``subtract.ゼロにならない`` () =
    testing {
        let! points = ArgGen.notEqualPoints
        test NonZeroPoint.subtract points ==> Assert.notAprxEqual Unchecked.defaultof<NonZeroPoint>
    }
