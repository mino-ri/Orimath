[<AutoOpen>]
module internal Orimath.Core.Internal
open System
open System.Collections.Generic
open ApplicativeProperty

let asList (s: seq<'a>) =
    match s with
    | :? list<'a> as lst -> lst
    | _ -> List.ofSeq s

let revList s = ([], s) ||> Seq.fold(fun t h -> h :: t)

let createArrayProp<'T when 'T : equality>() =
    ValueProp<'T[]>(Array.Empty(), { new IEqualityComparer<'T[]> with
        member _.Equals(a, b) = a.Length = b.Length && Array.forall2 (=) a b
        member _.GetHashCode(_) = 0 // not used
    }, System.Threading.SynchronizationContext.Current)

let inline flip f x y = f y x
