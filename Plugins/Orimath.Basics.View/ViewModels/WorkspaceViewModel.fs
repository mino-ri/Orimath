namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Controls
open Orimath.Plugins

type WorkspaceViewModel(workspace: IWorkspace, pointConverter: IViewPointConverter, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let toModelTarget (target: ScreenOperationTarget) =
        { Point = pointConverter.ViewToModel(target.Point)
          Layer = target.Layer
          Target = target.Target }

    member val Paper = new PaperViewModel(workspace.Paper, pointConverter, dispatcher)

    member _.OnClick(target, modifier) =
        match workspace.CurrentTool.Value with
        | :? IClickTool as tool ->
            dispatcher.Background { tool.OnClick(toModelTarget(target), modifier) }
        | _ -> ()

    member _.BeginDrag(source, modifier) =
        match workspace.CurrentTool.Value with
        | :? IDragTool as tool -> tool.BeginDrag(toModelTarget(source), modifier)
        | _ -> false

    member _.DragEnter(source, target, modifier) =
        match workspace.CurrentTool.Value with
        | :? IDragTool as tool -> tool.DragEnter(toModelTarget(source), toModelTarget(target), modifier)
        | _ -> false

    member _.DragLeave(source, target, modifier) =
        match workspace.CurrentTool.Value with
        | :? IDragTool as tool -> tool.DragLeave(toModelTarget(source), toModelTarget(target), modifier)
        | _ -> false

    member _.DragOver(source, target, modifier) =
        match workspace.CurrentTool.Value with
        | :? IDragTool as tool -> tool.DragOver(toModelTarget(source), toModelTarget(target), modifier)
        | _ -> false

    member _.Drop(source, target, modifier) =
        match workspace.CurrentTool.Value with
        | :? IDragTool as tool ->
            dispatcher.Background { tool.Drop(toModelTarget(source), toModelTarget(target), modifier) }
        | _ -> ()
