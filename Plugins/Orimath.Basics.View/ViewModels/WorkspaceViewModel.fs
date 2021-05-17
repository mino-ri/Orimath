namespace Orimath.Basics.View.ViewModels
open Orimath.Core
open Orimath.Controls
open Orimath.Plugins

type WorkspaceViewModel(workspace: IWorkspace, pointConverter: IViewPointConverter, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let toModelTarget (target: ScreenOperationTarget) =
        let point = pointConverter.ViewToModel(target.Point)
        let adjustedPoint =
            match target.Target with
            | DisplayTarget.Point(point) -> point
            | DisplayTarget.Crease(crease) -> Line.perpFoot point crease.Line
            | DisplayTarget.Edge(edge) -> Line.perpFoot point edge.Line
            | DisplayTarget.Layer _ -> point
        { Point = adjustedPoint
          Layer = target.Layer
          Target = target.Target }

    member val Paper = PaperViewModel(workspace.Paper, pointConverter, dispatcher)

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
        | :? IDragTool as tool ->
            tool.DragEnter(toModelTarget(source), toModelTarget(target), modifier)
        | _ -> false

    member _.DragLeave(source, target, modifier) =
        match workspace.CurrentTool.Value with
        | :? IDragTool as tool ->
            tool.DragLeave(toModelTarget(source), toModelTarget(target), modifier)
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

    member _.CancelDrag(source, modifier) =
        match workspace.CurrentTool.Value with
        | :? IDragTool as tool ->
            dispatcher.Background { tool.CancelDrag(toModelTarget(source), modifier) }
        | _ -> ()
