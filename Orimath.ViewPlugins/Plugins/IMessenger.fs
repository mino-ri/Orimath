namespace Orimath.Plugins
open System
open System.Runtime.CompilerServices
open System.Windows
open System.Windows.Input

type ViewPane =
    | Main = 0
    | Top = 1
    | Side = 2
    | Dialog = 3

type IMessenger =
    abstract member AddViewModel : viewModel: obj -> unit
    abstract member RemoveViewModel : viewModelType: Type -> int
    abstract member RemoveViewModel : viewModel: obj -> unit
    abstract member OpenDialog : viewModel: obj -> unit
    abstract member CloseDialog : unit -> unit
    abstract member CloseDialogCommand : ICommand
    abstract member RegisterView : viewModelType: Type * pane: ViewPane * viewProvidor: Func<obj, FrameworkElement> -> unit

[<Extension>]
type Messenger =
    [<Extension>]
    static member RegisterView<'T>(messenger: IMessenger, pane: ViewPane, viewProvidor: Func<'T, FrameworkElement>) =
        messenger.RegisterView(typeof<'T>, pane, fun vm -> viewProvidor.Invoke(unbox vm))
