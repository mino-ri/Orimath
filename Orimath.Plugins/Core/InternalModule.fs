[<AutoOpen>]
module internal Orimath.Core.InternalModule

let asList(s: seq<'a>) =
    match s with
    | :? list<'a> as lst -> lst
    | _ -> List.ofSeq s

let inline flip f x y = f y x