namespace Orimath.Core
open System

type INearlyEquatable<'T> =
    abstract member NearlyEquals : other: 'T * margin: float -> bool

module NearlyEquatable =
    [<CompiledName("DefaultMargin")>]
    let defaultMargin = 1.0 / 1024.0

    [<CompiledName("NearlyEquals")>]
    let nearlyEqualsf margin (x: float) y =
        not (Double.IsFinite(x)) && not (Double.IsFinite(y)) ||
        x - margin <= y && y <= x + margin

    [<CompiledName("NearlyEquals")>]
    let nearlyEquals margin (x: 'T when 'T :> INearlyEquatable<'T>) y = x.NearlyEquals(y, margin)

    [<CompiledName("NearlyEquals")>]
    let inline (=~~) x y = nearlyEqualsf defaultMargin x y

    [<CompiledName("NearlyLessThan")>]
    let inline (<=~) x y = x - defaultMargin <= y

    [<CompiledName("NearlyGreaterThan")>]
    let inline (>=~) x y = x >= y - defaultMargin

    [<CompiledName("NearlyEquals")>]
    let inline (=~) (x: 'T when 'T :> INearlyEquatable<'T>) y = x.NearlyEquals(y, defaultMargin)

    [<CompiledName("NotNearlyEquals")>]
    let inline (<>~) (x: 'T when 'T :> INearlyEquatable<'T>) y = not (x.NearlyEquals(y, defaultMargin))

    // floatの「-0」表現を回避する演算子
    [<CompiledName("Negate")>]
    let (!-) x = if x = 0.0 then 0.0 else -x

    [<CompiledName("UnaryPlus")>]
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
