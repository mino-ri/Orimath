module Orimath.Test.Point
open Orimath.Core
open Testexp


[<Fact>]
let ``dist.異なる2点間の距離`` () =
    testing {
        let! point1 = ArgGen.pointIn 0.0 1.0
        let! point2 = ArgGen.pointIn -2.0 -1.0
        test Point.dist (point1, point2) ==> Assert.greaterThan 0.0
    }

[<Fact>]
let ``dist.同一の点同士の距離は0`` () =
    testing {
        let! point = ArgGen.point
        test Point.dist (point, point) ==> Assert.aprxEqualf 0.0
    }

[<Fact>]
let ``reflect.反射による距離の性質`` () =
    testing {
        let! line = ArgGen.line
        let! point = ArgGen.point
        let! result = test Point.reflectBy (line, point)
        Assert.equal (Line.distSign point line) -(Line.distSign result line)
        Assert.aprxEqualf (Line.dist point line) (Line.dist result line)
    }

[<Fact>]
let ``reflect.2回反射すると元に戻る`` () =
    let doubleReflect line point = point |> Point.reflectBy line |> Point.reflectBy line
    testing {
        let! line = ArgGen.line
        let! point = ArgGen.point
        test doubleReflect (line, point) ==> Assert.aprxEqual point
    }
