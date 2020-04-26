namespace Orimath.Plugins
open System
open System.Windows.Input

type ViewPane =
    | Main = 0
    | Top = 1
    | Side = 2
    | Dialog = 3

type IMessenger =
    abstract member AddViewModel : viewModel: obj -> unit
    abstract member RemoveViewModel : viewModelType: Type -> unit
    abstract member RemoveViewModel : viewModel: obj -> unit
    abstract member OpenDialog : viewModel: obj -> unit
    abstract member CloseDialog : unit -> unit
    abstract member CloseDialogCommand : ICommand
