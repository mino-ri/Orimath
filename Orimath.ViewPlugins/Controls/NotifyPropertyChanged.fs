namespace Orimath.Controls
open System.Collections.Generic
open System.ComponentModel
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

[<AllowNullLiteral>]
type NotifyPropertyChanged() =
    let ev = Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()

    [<CLIEvent>]
    member _.PropertyChanged = ev.Publish

    member this.OnPropertyChanged([<Optional; DefaultParameterValue(""); CallerMemberName>] propertyName: string) =
        ev.Trigger(this, PropertyChangedEventArgs(propertyName))

    member this.SetValue<'T>(storage: byref<'T>, value: 'T, [<Optional; DefaultParameterValue(""); CallerMemberName>] propertyName: string) =
        if EqualityComparer<'T>.Default.Equals(storage, value) then
            false
        else
            storage <- value
            this.OnPropertyChanged(propertyName)
            true

    interface INotifyPropertyChanged with
        [<CLIEvent>]
        member _.PropertyChanged = ev.Publish
