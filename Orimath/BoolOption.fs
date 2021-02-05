namespace Orimath

[<RequireQualifiedAccess>]
module internal BoolOption =
    let some (value: 'T) = true, value

    let none<'T> = false, Unchecked.defaultof<'T>

    let isSome (hasValue: bool, value) = hasValue

    let isNone (hasValue: bool, value) = not hasValue

    let map mapping (hasValue, value) = if hasValue then true, mapping value else none

    let bind mapping (hasValue, value) = if hasValue then mapping value else none

    let filter cond (hasValue, value) = if hasValue && cond value then hasValue, value else none

    let toOption (hasValue, value) = if hasValue then Some(value) else None
