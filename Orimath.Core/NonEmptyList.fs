namespace Orimath

[<Struct>]
type NonEmptyList<'T> = private NonEmptyList of 'T list with
    member this.List = match this with NonEmptyList(lst) -> lst


[<AutoOpen>]
module NoEmptyListOperator =
    let (|NonEmptyList|) (lst: NonEmptyList<'T>) = lst.List

    [<return: Struct>]
    let (|AsNonEmptyList|_|) (lst: 'T list) =
        match lst with
        | _ :: _ -> ValueSome(NonEmptyList(lst))
        | _ -> ValueNone


module NonEmptyList =
    let create head tail : NonEmptyList<'T> = NonEmptyList(head :: tail)
        
    let tryCreate (lst: 'T list) =
        match lst with
        | _ :: _ -> Ok(NonEmptyList(lst))
        | _ -> Error(Error.emptyList)
