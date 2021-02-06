namespace Orimath.Controls
open System.Collections.Generic
open System.Collections.Specialized
open System.Collections.ObjectModel
open System.ComponentModel

type ResettableObservableCollection<'T>(collection: seq<'T>) =
    inherit ObservableCollection<'T>(collection)

    member this.Reset(newItems) =
        this.Items.Clear()
        (this.Items :?> List<'T>).AddRange(newItems)
        this.OnCollectionChanged(NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))
        this.OnPropertyChanged(PropertyChangedEventArgs(nameof this.Count))

    new() = ResettableObservableCollection(array.Empty())
