[<AutoOpen>]
module Orimath.Operators
open System.Collections.Generic
open ApplicativeProperty

let inline flip f x y = f y x

let inline swapWhen cond a b = if cond then b, a else a, b

let inline checkNonNull argName arg =
    if isNull arg then nullArg argName

let revList s = ([], s) ||> Seq.fold (fun t h -> h :: t)

let createArrayProp<'T when 'T : equality>() =
    let comparer =
        { new IEqualityComparer<'T[]> with
            member _.Equals(a, b) = a.Length = b.Length && Array.forall2 (=) a b
            member _.GetHashCode(_) = 0
        }
    ValueProp<'T[]>(array.Empty(), comparer, System.Threading.SynchronizationContext.Current)


type ExistsBuilder internal () =
    member inline _.Bind(m, f) = Option.exists f m
    member inline _.Bind(m, f) = ValueOption.exists f m
    member inline _.Bind(m, f) = List.exists f m
    member inline _.Bind(m, f) = Array.exists f m
    member inline _.Bind(m, f) = Seq.exists f m
    member inline _.Zero() = false
    member inline _.Return(x: bool) = x

let exists = ExistsBuilder()

type ResultBuilder() =
    member inline _.Bind(m, [<InlineIfLambda>] f: 'T -> Result<'U, 'Error>) =
        match m with
        | Ok(value) -> f value
        | Error(error) -> Error(error)
    member inline _.BindReturn(m, [<InlineIfLambda>] f: 'T -> 'U) =
        match m with
        | Ok(value) -> Ok(f value)
        | Error(error) -> Error(error)
    member inline _.Zero() = Ok()
    member inline _.Return(value: 'T) = Ok(value)
    member inline _.ReturnFrom(value: Result<'T, 'Error>) = value
    member inline _.Delay([<InlineIfLambda>] f: unit -> Result<'T, 'Error>) = f
    member inline _.Run([<InlineIfLambda>] f: unit -> Result<'T, 'Error>) = f ()
    member inline _.TryWith([<InlineIfLambda>] body: unit -> Result<'T, 'Error>, [<InlineIfLambda>] catch: 'Error -> Result<'T, 'Error>) =
        match body () with
        | Ok(ok) -> Ok(ok)
        | Error(error) -> catch error

let result = ResultBuilder()
