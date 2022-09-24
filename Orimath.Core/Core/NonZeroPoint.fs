namespace Orimath.Core
open Orimath
open Orimath.ApproximatelyEquatable

[<Struct; NoComparison; StructuralEquality>]
type NonZeroPoint = private NonZeroPoint of x: float * y: float with
    member this.X = match this with NonZeroPoint(x, _) -> x
    member this.Y = match this with NonZeroPoint(_, y) -> y

    override this.ToString() = System.String.Format("({0:0.#####}, {1:0.#####})", this.X, this.Y)

    interface IApproximatelyEquatable<NonZeroPoint> with
        member this.ApproximatelyEquals(NonZeroPoint(ox, oy), margin) =
            let (NonZeroPoint(x, y)) = this
            aprxEqualf margin x ox && aprxEqualf margin y oy

    static member ( ~+ ) (a: NonZeroPoint) = a
    static member ( ~- ) (NonZeroPoint(x, y)) = NonZeroPoint(!-x, !-y)


[<AutoOpen>]
module NonZeroPointOperator =
    let (|NonZeroPoint|) (NonZeroPoint(x, y): NonZeroPoint) = struct (x, y)


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module NonZeroPoint =
    let create x y =
        if x =~~ 0 || y =~~ 0 then Error(Error.zeroValue)
        else Ok(NonZeroPoint(x, y))

    let fromRadians radians = NonZeroPoint(cos radians, -sin radians)

    let toPoint (NonZeroPoint(x, y)) = Point(x, y)

    let ofPoint (Point(x, y)) = create x y

    /// 点と点の距離を求めます。
    let dist (NonZeroPoint(x1, y1)) (NonZeroPoint(x2, y2)) = sqrt ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2))
    
    /// 点のノルムを求めます。
    let norm (NonZeroPoint(x, y)) = sqrt (x * x + y * y)

    /// 2つの異なる点の差を求めます。    
    let subtract (NotEqual(Point(x1, y1), Point(x2, y2))) = NonZeroPoint(!+(x2 - x1), !+(y2 - y1))

    /// 原点を中心として右に90°回転します。
    let rotateRight (NonZeroPoint(x, y)) = NonZeroPoint(y, !-x)

    /// 原点を中心として左に90°回転します。
    let rotateLeft (NonZeroPoint(x, y)) = NonZeroPoint(!-y, x)
    
    /// 点のX座標とY座標を入れ替えます。
    let swap (NonZeroPoint(x, y)) = (NonZeroPoint(y, x))
