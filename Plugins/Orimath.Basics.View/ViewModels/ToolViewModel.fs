namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty

[<AllowNullLiteral>]
type ToolViewModel(tool: ITool) =
    inherit NotifyPropertyChanged()
    member _.Source = tool
    member _.Name = tool.Name
    member _.ShortcutKey = tool.ShortcutKey
    member _.IconStream = tool.Icon
    member _.ToolTip =
        if String.IsNullOrWhiteSpace(tool.ShortcutKey)
        then tool.Name
        else $"%s{tool.Name} (%s{tool.ShortcutKey})"


type ToolListViewModel(workspace: IWorkspace, dispatcher: IDispatcher) =
    let lazyTool = lazy([| for e in workspace.Tools -> ToolViewModel(e) |])

    member _.Tools = lazyTool.Value

    member val CurrentTool =
        workspace.CurrentTool
        |> Prop.mapBoth
            (fun tool -> lazyTool.Value |> Array.tryFind (fun t -> t.Source = tool) |> Option.defaultValue null)
            (fun v -> if isNull v then Unchecked.defaultof<_> else v.Source)
        |> Prop.fetchBoth dispatcher.SyncContext
