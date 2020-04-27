namespace Orimath.Basics.ViewModels
open Mvvm
open Orimath.Plugins

type ScreenOperationTarget =
    {
        Point: ScreenPoint
        Target: DisplayTarget
    }

type WorkspaceViewModel(workspace: IWorkspace, pointConverter: IViewPointConverter, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()

    member val Paper = new PaperViewModel(workspace.Paper, pointConverter, dispatcher)
    member private __.ToModelTarget(target: ScreenOperationTarget) =
        {
            OperationTarget.Point = pointConverter.ViewToModel(target.Point)
            OperationTarget.Target = target.Target
        }

    member this.OnClick(target, modifier) =
        ignore (dispatcher.OnBackgroundAsync(fun () -> workspace.CurrentTool.OnClick(this.ToModelTarget(target), modifier)))
    member this.BeginDrag(source, modifier) = workspace.CurrentTool.BeginDrag(this.ToModelTarget(source), modifier)
    member this.DragEnter(source, target, modifier) = workspace.CurrentTool.DragEnter(this.ToModelTarget(source), this.ToModelTarget(target), modifier)
    member this.DragLeave(source, target, modifier) = workspace.CurrentTool.DragLeave(this.ToModelTarget(source), this.ToModelTarget(target), modifier)
    member this.DragOver(source, target, modifier) = workspace.CurrentTool.DragOver(this.ToModelTarget(source), this.ToModelTarget(target), modifier)
    member this.Drop(source, target, modifier) =
        ignore (dispatcher.OnBackgroundAsync(fun () -> workspace.CurrentTool.Drop(this.ToModelTarget(source), this.ToModelTarget(target), modifier)))
