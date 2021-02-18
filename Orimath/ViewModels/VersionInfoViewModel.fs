namespace Orimath.ViewModels
open Orimath
open Orimath.Controls
open Orimath.Plugins
open System.Reflection

type VersionInfoViewModel(messenger: IMessenger) =
    inherit NotifyPropertyChanged()
    member val Title = Language.GetWord("{SystemCommand.VersionInfo}Version info")
    member val OkText = Language.GetWord("{Ok}OK")

    member val Version =
        Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        |> Null.bind (fun att -> att.InformationalVersion)
        |> Null.defaultValue ""

    member _.CloseCommand = messenger.CloseDialogCommand
