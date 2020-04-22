namespace Orimath.Plugins
open NearlyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Line = private { A: float; B: float; C: float } with
    member this.XFactor = this.A
    member this.YFactor = this.B
    member this.Intercept = this.C
    member this.Slope = !-(this.A / this.B)

    override this.ToString() = System.String.Format("[{0:0.#####}, {1:0.#####}, {2:0.#####}]", this.A, this.B, this.C)

    interface INearlyEquatable<Line> with
        member this.NearlyEquals(other, margin) =
            nearlyEquals margin this.A other.A &&
            nearlyEquals margin this.B other.B &&
            nearlyEquals margin this.C other.C

    static member Create(xFactor, yFactor, intercept) =
        let d = sqrt (xFactor * xFactor + yFactor * yFactor)
        let a = xFactor / d
        let b = yFactor / d
        let c = intercept / d
        match a, b with
        | 0.0, 0.0 -> failwith "直線の傾きを定義できません。"
        | 0.0, v | v, _ when v < 0.0 -> {
                A = !-a
                B = !-b
                C = !-c
            }
        | _ -> {
                A = !+a
                B = !+b
                C = !+c
            }
