namespace Orimath.ViewModels
open Orimath.Plugins
open Orimath.Converters

type ToolViewModel(tool: ITool, pointConverter: ScreenPointConverter) =
    inherit NotifyPropertyChanged()
    member __.Source = tool
    member __.Name = tool.Name
    member __.ShortcutKey = tool.ShortcutKey

    member __.OnClick(target, point, modifier) =
        runAsync <| fun () -> tool.OnClick(target, pointConverter.ConvertBack(point), modifier)
    member __.BeginDrag(target, point, modifier) = tool.BeginDrag(target, pointConverter.ConvertBack(point), modifier)
    member __.DragEnter(target, point, modifier) = tool.DragEnter(target, pointConverter.ConvertBack(point), modifier)
    member __.DragLeave(target, point, modifier) = tool.DragLeave(target, pointConverter.ConvertBack(point), modifier)
    member __.DragOver(target, point, modifier) = tool.DragOver(target, pointConverter.ConvertBack(point), modifier)
    member __.Drop(target, point, modifier) =
        runAsync <| fun () -> tool.Drop(target, pointConverter.ConvertBack(point), modifier)
