namespace Orimath
open Orimath.ApproximatelyEquatable

type NotEqual<'T when 'T :> IApproximatelyEquatable<'T>> = private NotEqual of 'T * 'T


[<AutoOpen>]
module NotEqualOperator =
    let (|NotEqual|) (NotEqual(a, b) as diff) = struct (a, b)

    [<return: Struct>]
    let (|AsNotEqual|_|) (a, b) = if a =~ b then ValueNone else ValueSome(NotEqual(a, b))


module NotEqual =
    let create (a: 'T) (b: 'T) =
        if a =~ b then Error(Error.mustBeDifferent)
        else Ok(NotEqual(a, b))
