namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty

[<AllowNullLiteral>]
type ToolViewModel(tool: ITool, messenger: IMessenger) =
    inherit NotifyPropertyChanged()
    member _.Source = tool
    member _.Name = messenger.LocalizeWord(tool.Name)
    member _.ShortcutKey = tool.ShortcutKey
    member _.IconStream = tool.Icon
    member _.ToolTip =
        if String.IsNullOrWhiteSpace(tool.ShortcutKey)
        then messenger.LocalizeWord(tool.Name)
        else $"%s{messenger.LocalizeWord(tool.Name)} (%s{tool.ShortcutKey})"


type ToolListViewModel(messenger: IMessenger, workspace: IWorkspace, dispatcher: IDispatcher) =
    let lazyTool = lazy([| for e in workspace.Tools -> ToolViewModel(e, messenger) |])

    member _.Tools = lazyTool.Value

    member val CurrentTool =
        workspace.CurrentTool
        |> Prop.mapBoth
            (fun tool ->
                lazyTool.Value
                |> Array.tryFind (fun t -> t.Source = tool)
                |> Option.defaultValue null)
            (fun v -> if isNull v then Unchecked.defaultof<_> else v.Source)
        |> Prop.fetchBoth dispatcher.SyncContext
