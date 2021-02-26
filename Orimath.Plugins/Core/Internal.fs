[<AutoOpen>]
module internal Orimath.Core.Internal

let asList (s: seq<'a>) =
    match s with
    | :? ('a list) as lst -> lst
    | _ -> List.ofSeq s

let inline flip f x y = f y x

type ExistsBuilder() =
    member inline _.Bind(m, f) = Option.exists f m
    member inline _.Bind(m, f) = List.exists f m
    member inline _.Bind(m, f) = Array.exists f m
    member inline _.Bind(m, f) = Seq.exists f m
    member inline _.Zero() = false
    member inline _.Return(x: bool) = x

let exists = ExistsBuilder()

let swapWhen cond a b = if cond then b, a else a, b
