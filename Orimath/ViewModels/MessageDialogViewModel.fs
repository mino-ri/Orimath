namespace Orimath.ViewModels
open Orimath.Plugins
open Orimath.Controls

type MessageDialogViewModel(messenger: IMessenger, message: string) =
    inherit NotifyPropertyChanged()

    member _.Message = message
    member _.CloseCommand = messenger.CloseDialogCommand
    member _.Header = messenger.LocalizeWord("{Title}Orimath")
    member _.OkText = messenger.LocalizeWord("{Ok}OK")
