namespace Orimath.Basics.ViewModels
open System
open Mvvm
open Orimath.Plugins

type internal AttachedObservableCollection<'Model, 'ViewModel>
    (dispatcher: IDispatcher,
     init: seq<'Model>,
     source: ICollectionChangedEvent<'Model>,
     mapper: 'Model -> 'ViewModel,
     onRemove: 'ViewModel -> unit) as this =
    inherit ResettableObservableCollection<'ViewModel>(init |> Seq.map mapper)

    let disconnector = source.Subscribe(function
        | CollectionChange.Add(_, items) ->
            ignore (dispatcher.OnUIAsync(fun () -> for item in items do this.Add(mapper item)))
        | CollectionChange.Remove(_, items) ->
            ignore (dispatcher.OnUIAsync(fun () ->
                let length = this.Count
                for index in (length - 1)..(-1)..(length - items.Count) do this.RemoveAt(index)))
        | CollectionChange.Replace(index, _, item) ->
            ignore (dispatcher.OnUIAsync(fun () ->
                onRemove this.[index]
                this.[index] <- mapper item))
        | CollectionChange.Reset(_, items) ->
            ignore (dispatcher.OnUIAsync(fun () ->
                Seq.iter onRemove this
                this.Reset(items |> Seq.map mapper))))

    member __.Dispose() = disconnector.Dispose()

    interface IDisposable with
        member this.Dispose() = this.Dispose()
