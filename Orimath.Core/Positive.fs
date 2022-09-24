namespace Orimath

[<Struct>]
type Positive<'T when 'T : struct> = private Positive of 'T with
    member this.Value = match this with Positive(value) -> value

    static member inline op_Explicit(positive : Positive<_>) = int8 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = int16 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = int32 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = int64 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = uint8 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = uint16 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = uint32 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = uint64 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = float32 positive.Value
    static member inline op_Explicit(positive : Positive<_>) = float positive.Value
    static member inline op_Explicit(positive : Positive<_>) = decimal positive.Value


[<AutoOpen>]
module PositiveOperator =
    let (|Positive|) (value: Positive<'T>) = value.Value

    [<return: Struct>]
    let (|AsPositive|_|) (value: 'T) =
        if value > Unchecked.defaultof<'T>
        then ValueSome(Positive(value))
        else ValueNone


module Positive =
    let create (value: 'T) =
        if value > Unchecked.defaultof<'T> then
            Ok(Positive(value))
        else
            Error(Error.negativeValue)

    let int (value: int) = create value
    
    let uint (value: uint) = create value

    let float32 (value: float32) = create value

    let float (value: float) = create value
