namespace Orimath.Basics.View.ViewModels
open System.Windows.Media.Imaging
open Orimath.Controls
open Orimath.Plugins


type InstructionListDialogViewModel(messenger: IMessenger, size: int, images: BitmapSource list) =
    inherit NotifyPropertyChanged()
    member val Header = messenger.LocalizeWord("{basic/Effect.InstructionList}Show instructions...")
    member val OkText = messenger.LocalizeWord("{Ok}OK")
    member _.ImageSize = size
    member _.Images = images
    member _.CloseCommand = messenger.CloseDialogCommand
