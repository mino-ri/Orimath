namespace Orimath.Plugins
open System.Collections.Generic

type CollectionChangeAction =
    | Add = 0
    | Remove = 1
    | Replace = 2
    | Reset = 3

[<RequireQualifiedAccess>]
type CollectionChange<'Item> =
    | Add of index: int * items: IReadOnlyList<'Item>
    | Remove of index: int * items: IReadOnlyList<'Item>
    | Replace of index: int * oldItem: 'Item * newItem: 'Item
    | Reset of oldItems: IReadOnlyList<'Item> * newItems: IReadOnlyList<'Item>
    with
    member this.Action =
        match this with
        | Add _ -> CollectionChangeAction.Add
        | Remove _ -> CollectionChangeAction.Remove
        | Replace _ -> CollectionChangeAction.Replace
        | Reset _ -> CollectionChangeAction.Reset

    member this.Index =
        match this with
        | Add(index, _)
        | Remove(index, _)
        | Replace(index, _, _) -> index
        | Reset(_) -> 0

    member this.OldItems =
        match this with
        | Reset(items, _) -> items
        | Add(_, _) -> upcast []
        | Remove(_, items) -> items
        | Replace(_, oldItem, _) -> upcast [ oldItem ]

    member this.NewItems =
        match this with
        | Reset(_, items) -> items
        | Add(_, items) -> items
        | Remove(_, _) -> upcast []
        | Replace(_, _, item) -> upcast [ item ]

type CollectionChangedEventHandler<'Item> = delegate of sender: obj * args: CollectionChange<'Item> -> unit

module CollectionChange =
    [<CompiledName("Add")>]
    let add index (items: IReadOnlyList<'Item>) = CollectionChange.Add(index, items)

    [<CompiledName("Remove")>]
    let remove index (items: IReadOnlyList<'Item>) = CollectionChange.Remove(index, items)

    [<CompiledName("Replace")>]
    let replace index (oldItem: 'Item) newItem = CollectionChange.Replace(index, oldItem, newItem)

    [<CompiledName("Reset")>]
    let reset (oldItems: IReadOnlyList<'Item>) newItems = CollectionChange.Reset(oldItems, newItems)

type ICollectionChangedEvent<'Item> = IEvent<CollectionChangedEventHandler<'Item>, CollectionChange<'Item>>
