namespace Orimath.Test
open System
open Orimath
open Orimath.Core
open Testexp
open Orimath.ApproximatelyEquatable


type FactAttribute = Xunit.FactAttribute


module Assert =
    let aprxEqual (expected: 'T) (actual: 'T) (context: TestContext<'Result>) =
        Assert.should ((=~) expected) (Assert.formatError2 "Should be EXPECTED =~ ACTUAL." expected) actual context
    
    let aprxEqualf (expected: float) (actual: float) (context: TestContext<'Result>) =
        Assert.should ((=~~) expected) (Assert.formatError2 "Should be EXPECTED =~~ ACTUAL." expected) actual context
    
    let notAprxEqual (expected: 'T) (actual: 'T) (context: TestContext<'Result>) =
        Assert.should ((<>~) expected) (Assert.formatError2 "Should be EXPECTED <>~ ACTUAL." expected) actual context
    
    let notAprxEqualf (expected: float) (actual: float) (context: TestContext<'Result>) =
        Assert.should ((<>~~) expected) (Assert.formatError2 "Should be EXPECTED <>~~ ACTUAL." expected) actual context
    

module ArgGen =
    let vchoose (chooser: 'T -> 'U voption) (source: IArgumentGenerator<'T>) : IArgumentGenerator<'U> =
        argGen {
            let mutable result = ValueNone
            while result.IsNone do
                let! value = source
                result <- chooser value
            return result.Value
        }

    let rchoose (chooser: 'T -> Result<'U, 'Error>) (source: IArgumentGenerator<'T>) : IArgumentGenerator<'U> =
        argGen {
            let mutable result = Error(Unchecked.defaultof<_>)
            while Result.isError result do
                let! value = source
                result <- chooser value
            return Result.forceOk result
        }

    let radians = ArgGen.floatRange 0.0 Math.PI
  
    let notEqual (source: IArgumentGenerator<'T>) =
        argGen {
            let! x = source
            and! y = source
            return NotEqual.create x y
        }
        |> rchoose id

    let pointIn min max = argGen {
        let! x = ArgGen.floatRange min max
        and! y = ArgGen.floatRange min max
        return Point(x, y)
    }
    
    let lineIn interceptMin interceptMax = argGen {
        let! radians = radians
        and! intercept = ArgGen.floatRange interceptMin interceptMax
        return Line.create (NonZeroPoint.fromRadians radians) intercept
    }

    let nonZeroPointIn min max = pointIn min max |> rchoose NonZeroPoint.ofPoint

    let notEqualPointsIn min max = pointIn min max |> notEqual
    
    let notEqualFloatIn min max =
        let generator = ArgGen.floatRange min max
        argGen {
            let! x = generator
            and! y = generator
            return if x =~~ y then ValueSome(struct (x, y)) else ValueNone
        }
        |> vchoose id

    let segmentIn min max = notEqualPointsIn min max |> ArgGen.map LineSegment.fromPoints

    let point = pointIn -1.0 1.0

    let line = lineIn -1.0 1.0

    let nonZeroPoint = nonZeroPointIn -1.0 1.0

    let notEqualPoints = notEqualPointsIn -1.0 1.0

    let segment = segmentIn -1.0 1.0
