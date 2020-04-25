namespace Orimath.ViewModels
open Orimath.Plugins
open Orimath.Plugins.ThreadController

type ScreenOperationTarget =
    {
        Point: ScreenPoint
        Target: DisplayTarget
    }

type ToolViewModel(tool: ITool, pointConverter: ScreenPointConverter) =
    inherit NotifyPropertyChanged()
    member __.Source = tool
    member __.Name = tool.Name
    member __.ShortcutKey = tool.ShortcutKey

    member __.ToModelTarget(target: ScreenOperationTarget) =
        {
            OperationTarget.Point = pointConverter.ScreenToModel(target.Point)
            OperationTarget.Target = target.Target
        }

    member this.OnClick(target, modifier) =
        runAsync <| fun () -> tool.OnClick(this.ToModelTarget(target), modifier)
    member this.BeginDrag(source, modifier) = tool.BeginDrag(this.ToModelTarget(source), modifier)
    member this.DragEnter(source, target, modifier) = tool.DragEnter(this.ToModelTarget(source), this.ToModelTarget(target), modifier)
    member this.DragLeave(source, target, modifier) = tool.DragLeave(this.ToModelTarget(source), this.ToModelTarget(target), modifier)
    member this.DragOver(source, target, modifier) = tool.DragOver(this.ToModelTarget(source), this.ToModelTarget(target), modifier)
    member this.Drop(source, target, modifier) =
        runAsync <| fun () -> tool.Drop(this.ToModelTarget(source), this.ToModelTarget(target), modifier)
