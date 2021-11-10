namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty

type AttachedObservableCollection<'Model, 'ViewModel>
    (dispatcher: IDispatcher,
     source: IReactiveCollection<'Model>,
     mapping: 'Model -> 'ViewModel,
     onRemove: 'ViewModel -> unit) as this =
    inherit ResettableObservableCollection<'ViewModel>(source |> Seq.map mapping)
    let disconnector =
        source |> Observable.subscribe2(this.SourceCollectionChanged)
    
    member private this.SourceCollectionChanged(e) =
        dispatcher.UI.Invoke(fun () ->
            match e with
            | CollectionChange.Add(index, items) ->
                if index = this.Count then
                    for item in items do this.Add(mapping item)
                else
                    for i = 0 to items.Count - 1 do
                        this.Insert(index + i, mapping items[i])
            | CollectionChange.Remove(index, items) ->
                let last = index + items.Count
                for index = last - 1 downto index do
                    onRemove(this[index])
                    this.RemoveAt(index)
            | CollectionChange.Replace(index, _, newItem) ->
                onRemove(this[index])
                this[index] <- mapping newItem
            | CollectionChange.Reset(_, newItems) ->
                this |> Seq.iter onRemove
                this.Reset(newItems |> Seq.map mapping))

    member _.Dispose() = disconnector.Dispose()

    interface IDisposable with
        member _.Dispose() = disconnector.Dispose()
