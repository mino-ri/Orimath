namespace Orimath.Core
open System

type INearlyEquatable<'T> =
    abstract member NearlyEquals : other: 'T * margin: float -> bool

module NearlyEquatable =
    let defaultMargin = 1.0 / 1024.0

    let nearlyEqualsf margin (x: float) y =
        not (Double.IsFinite(x)) && not (Double.IsFinite(y)) ||
        x - margin <= y && y <= x + margin

    let nearlyEquals margin (x: 'T when 'T :> INearlyEquatable<'T>) y = x.NearlyEquals(y, margin)

    let inline (=~~) x y = nearlyEqualsf defaultMargin x y

    let inline (<=~) x y = x - defaultMargin <= y

    let inline (>=~) x y = x >= y - defaultMargin

    let inline (=~) (x: 'T when 'T :> INearlyEquatable<'T>) y = x.NearlyEquals(y, defaultMargin)

    let inline (<>~) (x: 'T when 'T :> INearlyEquatable<'T>) y = not (x.NearlyEquals(y, defaultMargin))

    // floatの「-0」表現を回避する演算子
    let (!-) x = if x = 0.0 then 0.0 else -x

    let (!+) x = if x = 0.0 then 0.0 else x

    [<CustomEquality; NoComparison>]
    type Nearly<'T when 'T: equality and 'T :> INearlyEquatable<'T>>(value: 'T) = struct
        member _.Value = value
        override _.Equals(other) = 
            match other with
            | :? Nearly<'T> as v -> value.NearlyEquals(v.Value, defaultMargin)
            | _ -> false
        override _.GetHashCode() = 0
        override _.ToString() = value.ToString()
        interface IEquatable<Nearly<'T>> with
            member _.Equals(other) = value.NearlyEquals(other.Value, defaultMargin)
    end
