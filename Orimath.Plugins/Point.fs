namespace Orimath.Plugins
open NearlyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Point = { X: float; Y: float } with
    member this.Norm = sqrt (this.X * this.X + this.Y * this.Y)

    override this.ToString() = System.String.Format("({0:0.#####}, {1:0.#####})", this.X, this.Y)

    interface INearlyEquatable<Point> with
        member this.NearlyEquals(other, margin) =
            nearlyEquals margin this.X other.X && nearlyEquals margin this.Y other.Y
