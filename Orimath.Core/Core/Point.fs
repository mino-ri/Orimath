namespace Orimath.Core
open Orimath
open Orimath.ApproximatelyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Point = Point of x: float * y: float with
    member this.X = match this with Point(x, _) -> x
    member this.Y = match this with Point(_, y) -> y

    override this.ToString() = System.String.Format("({0:0.#####}, {1:0.#####})", this.X, this.Y)

    interface IApproximatelyEquatable<Point> with
        member this.ApproximatelyEquals(Point(ox, oy), margin) =
            let (Point(x, y)) = this
            aprxEqualf margin x ox && aprxEqualf margin y oy

    static member ( ~+ ) (a: Point) = a
    static member ( ~- ) (Point(x, y)) = Point(!-x, !-y)
    static member ( + ) (Point(ax, ay), Point(bx, by)) = Point(ax + bx, ay + by)
    static member ( - ) (Point(ax, ay), Point(bx, by)) = Point(ax - bx, ay - by)
    static member ( * ) (Point(ax, ay), scalar: float) = Point(ax * scalar, ay * scalar)
    static member ( * ) (scalar: float, Point(ax, ay)) = Point(ax * scalar, ay * scalar)
    static member ( / ) (Point(ax, ay), scalar: float) = Point(ax / scalar, ay / scalar)
    static member ( / ) (scalar: float, Point(ax, ay)) = Point(ax / scalar, ay / scalar)
