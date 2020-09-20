namespace Orimath.Core
open System.Collections
open System.Collections.Generic

type private ReactiveCollection<'T>(sender: obj, inner: ResizeArray<'T>) =
    let changed = CollectionChangedEvent<'T>()

    member _.Item with get(index) = inner.[index]

    member _.Count = inner.Count

    member _.Add(items: 'T list) =
        if items.Length > 0 then
            let index = inner.Count
            inner.AddRange(items)
            triggerAdd sender changed index items

    member _.Remove(count: int) =
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

    member _.Replace(index: int, item: 'T) =
        let oldItem = inner.[index]
        inner.[index] <- item
        triggerReplace sender changed index oldItem item

    member _.Reset(items: 'T list) =
        let oldItems = inner |> Seq.toList
        inner.Clear()
        inner.AddRange(items)
        triggerReset sender changed oldItems items

    member _.Changed = changed.Publish

    new(sender) = ReactiveCollection(sender, ResizeArray())

    new(sender, capacity: int) = ReactiveCollection(sender, ResizeArray(capacity))

    new(sender, items: seq<_>) = ReactiveCollection(sender, ResizeArray(items))

    interface IEnumerable with
        member _.GetEnumerator() = upcast inner.GetEnumerator()

    interface IReadOnlyList<'T> with
        member _.GetEnumerator() = upcast inner.GetEnumerator()
        member _.Count = inner.Count
        member _.Item with get(index) = inner.[index]
