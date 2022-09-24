namespace Orimath
open System

type IApproximatelyEquatable<'T> =
    abstract member ApproximatelyEquals : other: 'T * margin: float -> bool

module ApproximatelyEquatable =
    // 1 / 1024
    [<Literal>]
    let defaultMargin = 0.0009765625

    let aprxEqualf margin (x: float) y =
        not (Double.IsFinite(x)) && not (Double.IsFinite(y)) ||
        x - margin <= y && y <= x + margin

    let aprxEqual margin (x: 'T when 'T :> IApproximatelyEquatable<'T>) y = x.ApproximatelyEquals(y, margin)

    let inline (=~~) x y = aprxEqualf defaultMargin x y

    let inline (<>~~) x y = not (aprxEqualf defaultMargin x y)

    let inline (<=~) x y = x - defaultMargin <= y

    let inline (>=~) x y = x >= y - defaultMargin

    let inline (=~) (x: 'T when 'T :> IApproximatelyEquatable<'T>) y = x.ApproximatelyEquals(y, defaultMargin)

    let inline (<>~) (x: 'T when 'T :> IApproximatelyEquatable<'T>) y = not (x =~ y)

    // floatの「-0」表現を回避する演算子
    let (!-) x = if x = 0.0 then 0.0 else -x

    let (!+) x = if x = 0.0 then 0.0 else x

    [<CustomEquality; NoComparison; Struct>]
    type Nearly<'T when 'T: equality and 'T :> IApproximatelyEquatable<'T>> =
        | Nearly of value: 'T
        member this.Value = match this with Nearly(value) -> value
        override this.Equals(other) =
            let value = this.Value
            match other with
            | :? Nearly<'T> as v -> value.ApproximatelyEquals(v.Value, defaultMargin)
            | _ -> false
        override _.GetHashCode() = 0
        override this.ToString() = this.Value.ToString()
        interface IEquatable<Nearly<'T>> with
            member this.Equals(other) = this.Value.ApproximatelyEquals(other.Value, defaultMargin)
