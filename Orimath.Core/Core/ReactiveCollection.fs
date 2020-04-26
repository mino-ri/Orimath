namespace Orimath.Core
open System.Collections
open System.Collections.Generic

type private ReactiveCollection<'T>(sender: obj, inner: ResizeArray<'T>) =
    let changed = CollectionChangedEvent<'T>()

    member __.Item with get(index) = inner.[index]

    member __.Count = inner.Count

    member __.Add(items: 'T list) =
        if items.Length > 0 then
            let index = inner.Count
            inner.AddRange(items)
            triggerAdd sender changed index items

    member __.Remove(count: int) =
        let count = min count inner.Count
        if count > 0 then
            let endIndex = inner.Count
            let target =
                inner
                |> Seq.skip (endIndex - count)
                |> Seq.take count
                |> Seq.toList
            inner.RemoveRange(endIndex - count, count)
            triggerRemove sender changed endIndex target

    member __.Replace(index: int, item: 'T) =
        let oldItem = inner.[index]
        inner.[index] <- item
        triggerReplace sender changed index oldItem item

    member __.Reset(items: 'T list) =
        let oldItems = inner |> Seq.toList
        inner.Clear()
        inner.AddRange(items)
        triggerReset sender changed oldItems items

    member __.Changed = changed.Publish

    new(sender) = ReactiveCollection(sender, ResizeArray())

    new(sender, capacity: int) = ReactiveCollection(sender, ResizeArray(capacity))

    new(sender, items: seq<_>) = ReactiveCollection(sender, ResizeArray(items))

    interface IEnumerable with
        member __.GetEnumerator() = upcast inner.GetEnumerator()

    interface IReadOnlyList<'T> with
        member __.GetEnumerator() = upcast inner.GetEnumerator()
        member __.Count = inner.Count
        member __.Item with get(index) = inner.[index]
