namespace Orimath.Core
open NearlyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Point = { X: float; Y: float } with
    member this.Norm = sqrt (this.X * this.X + this.Y * this.Y)

    member this.Deconstruct(x: outref<float>, y: outref<float>) =
        x <- this.X
        y <- this.Y

    override this.ToString() = System.String.Format("({0:0.#####}, {1:0.#####})", this.X, this.Y)

    interface INearlyEquatable<Point> with
        member this.NearlyEquals(other, margin) =
            nearlyEqualsf margin this.X other.X && nearlyEqualsf margin this.Y other.Y

    static member ( ~+ ) (a: Point) = a
    static member ( ~- ) (a: Point) = { X = !-a.X; Y = !-a.Y }
    static member ( + ) (a: Point, b: Point) = { X = a.X + b.X; Y = a.Y + b.Y }
    static member ( - ) (a: Point, b: Point) = { X = a.X - b.X; Y = a.Y - b.Y }
    static member ( * ) (a: Point, scalar: float) = { X = a.X * scalar; Y = a.Y * scalar }
    static member ( * ) (scalar: float, a: Point) = { X = a.X * scalar; Y = a.Y * scalar }
    static member ( / ) (a: Point, scalar: float) = { X = a.X / scalar; Y = a.Y / scalar }
    static member ( / ) (scalar: float, a: Point) = { X = a.X / scalar; Y = a.Y / scalar }
