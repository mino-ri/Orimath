namespace Orimath.ViewModels
open System
open System.ComponentModel

type NotifyPropertyChanged() =
    let propertyChanged = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()

    [<CLIEvent>]
    member __.PropertyChanged = propertyChanged.Publish

    member internal this.OnPropertyChanged(propertyName: string) =
        propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName))

    interface INotifyPropertyChanged with
        member this.add_PropertyChanged(handler) = this.PropertyChanged.AddHandler(handler)
        member this.remove_PropertyChanged(handler) = this.PropertyChanged.RemoveHandler(handler)
