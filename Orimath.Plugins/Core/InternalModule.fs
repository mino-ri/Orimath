[<AutoOpen>]
module internal Orimath.Core.Internal

let asList(s: seq<'a>) =
    match s with
    | :? ('a list) as lst -> lst
    | _ -> List.ofSeq s

let inline flip f x y = f y x
