module Orimath.Test.Line
open System
open Orimath
open Orimath.Core
open Orimath.ApproximatelyEquatable
open Testexp


[<Fact>]
let ``tryCreate.createと結果が一致する`` () =
    testing {
        let! factors = ArgGen.nonZeroPoint
        let! intercept = ArgGen.floatRange -1.0 1.0
        let! result = test Line.tryCreate (factors.X, factors.Y, intercept)
        Assert.ok result
        Assert.aprxEqual (Line.create factors intercept) (Result.forceOk result)
    }

[<Fact>]
let ``signedDist.符号の性質`` () =
    testing {
        let! point = ArgGen.point
        let! line = ArgGen.line
        test Line.signedDist (point, line) ==>
            Assert.aprxEqualf (Line.dist point line * float (Line.distSign point line))
    }

[<Fact>]
let ``cross.交点の有無と2線が平行であるかが対応する`` () =
    testing {
        let! line1 = ArgGen.lineIn 0.0 1.0
        let! line2 = ArgGen.lineIn 0.0 1.0
        let! result = test Line.cross (line1, line2)
        if Line.slope line1 =~~ Line.slope line2 then
            Assert.vnone result
        else
            Assert.vsome result
    }
