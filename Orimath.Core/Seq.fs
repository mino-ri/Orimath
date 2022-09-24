module Orimath.Seq
open System.Collections.Generic

let asArray (source: seq<'T>) =
    match source with
    | :? ('T[]) as ar -> ar
    | _ -> Array.ofSeq source

let asList (source: seq<'T>) =
    match source with
    | :? ('T list) as lst -> lst
    | _ -> List.ofSeq source

let groupFold
    (projection: 'T -> 'Key)
    (createSeed: 'Key -> 'State)
    (folder: 'Key -> 'State -> 'T -> 'State)
    (source: seq<'T>)
    =
    let dic = Dictionary<'Key, 'State>()
    for item in source do
        let key = projection item
        let state =
            let ok, state = dic.TryGetValue(key)
            if ok then state else createSeed key
        dic[key] <- folder key state item
    dic

let vchoose (chooser: 'T -> 'U voption) (source: seq<'T>) =
    seq {
        for item in source do
            match chooser item with
            | ValueSome(value) -> yield value
            | ValueNone -> ()
    }

let rchoose (chooser: 'T -> Result<'U, 'Error>) (source: seq<'T>) =
    seq {
        for item in source do
            match chooser item with
            | Ok(value) -> yield value
            | Error _ -> ()
    }

let tryHeadLast (source: seq<'T>) =
    use e = source.GetEnumerator()
    if not (e.MoveNext()) then ValueNone
    else
        let head = e.Current
        let mutable last = head
        while e.MoveNext() do
            last <- e.Current
        ValueSome(struct (head, last))

let pairwiseOfNotEqual (source: seq<'T>) =
    seq {
        use enumerator = source.GetEnumerator()
        if enumerator.MoveNext() then
            let first = enumerator.Current
            let mutable latest = first
            while enumerator.MoveNext() do
                let current = enumerator.Current
                match latest, current with
                | AsNotEqual(notEqual) ->
                    yield notEqual
                    latest <- current
                | _ -> ()
            match latest, first with
            | AsNotEqual(notEqual) ->
                yield notEqual
            | _ -> ()
    }
