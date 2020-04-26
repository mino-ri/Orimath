namespace Orimath.Basics.ViewModels
open Mvvm
open Orimath.Plugins

type ToolViewModel(tool: ITool) =
    inherit NotifyPropertyChanged()
    member __.Source = tool
    member __.Name = tool.Name
    member __.ShortcutKey = tool.ShortcutKey
