namespace Orimath.ViewModels
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.ComponentModel

type NotifyPropertyChanged() =
    let propertyChanged = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()

    [<CLIEvent>]
    member __.PropertyChanged = propertyChanged.Publish

    member internal this.OnPropertyChanged([<CallerMemberName; Optional; DefaultParameterValue("")>] propertyName: string) =
        propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName))

    member internal this.SetValue(storage: byref<'T>, value: 'T, [<CallerMemberName; Optional; DefaultParameterValue("")>] propertyName: string) =
        if storage <> value then
            storage <- value
            this.OnPropertyChanged(propertyName)
            true
        else
            false

    member internal this.SetValueIgnore(storage: byref<'T>, value: 'T, [<CallerMemberName; Optional; DefaultParameterValue("")>] propertyName: string) =
        ignore <| this.SetValue(&storage, value, propertyName)

    interface INotifyPropertyChanged with
        member this.add_PropertyChanged(handler) = this.PropertyChanged.AddHandler(handler)
        member this.remove_PropertyChanged(handler) = this.PropertyChanged.RemoveHandler(handler)
