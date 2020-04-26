namespace Orimath.Basics.ViewModels
open System
open Mvvm
open Orimath.Plugins
open Orimath.Plugins.ThreadController

type internal AttachedObservableCollection<'Model, 'ViewModel>
    (invoker: IUIThreadInvoker,
     init: seq<'Model>,
     source: ICollectionChangedEvent<'Model>,
     mapper: 'Model -> 'ViewModel,
     onRemove: 'ViewModel -> unit) as this =
    inherit ResettableObservableCollection<'ViewModel>(init |> Seq.map mapper)

    let disconnector = source.Subscribe(function
        | CollectionChange.Add(_, items) ->
            onUI invoker <| fun () -> for item in items do this.Add(mapper item)
        | CollectionChange.Remove(_, items) ->
            onUI invoker <| fun () ->
                let length = this.Count
                for index in (length - 1)..(-1)..(length - items.Count) do this.RemoveAt(index)
        | CollectionChange.Replace(index, _, item) ->
            onUI invoker <| fun () ->
                onRemove this.[index]
                this.[index] <- mapper item
        | CollectionChange.Reset(_, items) ->
            onUI invoker <| fun () ->
                Seq.iter onRemove this
                this.Reset(items |> Seq.map mapper))

    member __.Dispose() = disconnector.Dispose()

    interface IDisposable with
        member this.Dispose() = this.Dispose()
