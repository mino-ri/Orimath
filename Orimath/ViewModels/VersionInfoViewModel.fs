namespace Orimath.ViewModels
open Orimath
open Orimath.Controls
open Orimath.Plugins
open System.Reflection

type VersionInfoViewModel(messenger: IMessenger) =
    inherit NotifyPropertyChanged()
    member val Version =
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        |> Null.bind (fun att -> att.InformationalVersion)
        |> Null.defaultValue ""

    member _.CloseCommand = messenger.CloseDialogCommand
