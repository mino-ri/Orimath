namespace Orimath.Core
open NearlyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Line = private { A: float; B: float; C: float } with
    member this.XFactor = this.A
    member this.YFactor = this.B
    member this.Intercept = this.C
    member this.Slope = !-(this.A / this.B)

    member this.Deconstruct(xFactor: outref<float>, yFactor: outref<float>, intercept: outref<float>) =
        xFactor <- this.A
        yFactor <- this.B
        intercept <- this.C

    override this.ToString() = System.String.Format("[{0:0.#####}, {1:0.#####}, {2:0.#####}]", this.A, this.B, this.C)

    interface INearlyEquatable<Line> with
        member this.NearlyEquals(other, margin) =
            nearlyEqualsf margin this.A other.A &&
            nearlyEqualsf margin this.B other.B &&
            nearlyEqualsf margin this.C other.C

    static member Create(xFactor, yFactor, intercept) =
        let d = sqrt (xFactor * xFactor + yFactor * yFactor)
        let a = xFactor / d
        let b = yFactor / d
        let c = intercept / d
        let aZero = a =~~ 0.0
        let bZero = b =~~ 0.0
        match aZero, bZero with
        | true, true -> failwith "直線の傾きを定義できません。"
        | false, true -> { A = 1.0; B = 0.0; C = if a < 0.0 then !-c else !+c }
        | true, false -> { A = 0.0; B = 1.0; C = if b < 0.0 then !-c else !+c }
        | false, false ->
            if a < 0.0
            then { A = !-a; B = !-b; C = !-c }
            else { A = !+a; B = !+b; C = !+c }

    [<CompiledName("FromPointsOption")>]
    static member FromPoints(p1, p2) =
        if p1 = p2 then None
        else
            Some(Line.Create(p1.X - p2.X, p1.Y - p2.Y, (p2.X * p2.X + p2.Y * p2.Y - p1.X * p1.X - p1.Y * p1.Y) / 2.0))

    [<CompiledName("FromPoints")>]
    static member FromPointsNullable(p1, p2) =
        Option.toNullable(Line.FromPoints(p1, p2))
