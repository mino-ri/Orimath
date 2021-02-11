[<AutoOpen>]
module internal Orimath.Core.Internal
open System.Collections.Generic
open ApplicativeProperty

let asList (s: seq<'a>) =
    match s with
    | :? ('a list) as lst -> lst
    | _ -> List.ofSeq s

let revList s = ([], s) ||> Seq.fold (fun t h -> h :: t)

let createArrayProp<'T when 'T : equality>() =
    let comparer =
        { new IEqualityComparer<'T[]> with
            member _.Equals(a, b) = a.Length = b.Length && Array.forall2 (=) a b
            member _.GetHashCode(_) = 0 }
    ValueProp<'T[]>(
        array.Empty(),
        comparer,
        System.Threading.SynchronizationContext.Current)

let inline flip f x y = f y x
