[<AutoOpen>]
module internal Orimath.Core.InternalModule
open Orimath.Plugins

type CollectionChangedEvent<'Item> = Event<CollectionChangedEventHandler<'Item>, CollectionChange<'Item>>

let asList (s: seq<'a>) =
    match s with
    | :? list<'a> as lst -> lst
    | _ -> List.ofSeq s

let revList s = ([], s) ||> Seq.fold(fun t h -> h :: t)

let triggerAdd sender (event: CollectionChangedEvent<'Item>) index items =
    event.Trigger(box sender, CollectionChange.Add(index, items))

let triggerRemove sender (event: CollectionChangedEvent<'Item>) index items =
    event.Trigger(box sender, CollectionChange.Remove(index, items))

let triggerReplace sender (event: CollectionChangedEvent<'Item>) index oldItem newItem =
    event.Trigger(box sender, CollectionChange.Replace(index, oldItem, newItem))

let triggerReset sender (event: CollectionChangedEvent<'Item>) oldItems newItems =
    event.Trigger(box sender, CollectionChange.Reset(oldItems, newItems))
