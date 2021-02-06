namespace Orimath.Basics.View.Controls
open System
open System.Windows
open System.Windows.Controls
open System.Windows.Input
open Orimath.Plugins
open Orimath.Basics.View
open Orimath.Basics.View.ViewModels

type MouseHandler() =
    inherit DependencyObject()
    let mutable clickControl = null: obj
    let mutable draggingGuid = ""
    let mutable pressed = MouseButton.Left
    let mutable draggingSource: Control = null
    let mutable draggingData = None
    let mutable workspace = None
    let isValidDropSource(data: IDataObject) = downcast data.GetData(typeof<string>) = draggingGuid
    let (|TargetControl|_|) (o: obj) =
        match o with
        | :? Control as ctrl when (ctrl.DataContext :? IDisplayTargetViewModel) ->
            Some(ctrl, ctrl.DataContext :?> IDisplayTargetViewModel)
        | _ -> None

    static let getModifier mouseButton =
        let mutable result =
            if mouseButton = MouseButton.Right
                then OperationModifier.RightButton
                else OperationModifier.None
        let keys = Keyboard.Modifiers
        if keys.HasFlag(ModifierKeys.Alt) then result <- result ||| OperationModifier.Alt
        if keys.HasFlag(ModifierKeys.Control) then result <- result ||| OperationModifier.Ctrl
        if keys.HasFlag(ModifierKeys.Shift) then result <- result ||| OperationModifier.Shift
        result
    
    member this.PositionRoot with get() = this.GetValue(MouseHandler.PositionRootProperty) :?> IInputElement and set (v: IInputElement) = this.SetValue(MouseHandler.PositionRootProperty, box v)
    static member val PositionRootProperty =
        DependencyProperty.Register("PositionRoot", typeof<IInputElement>, typeof<MouseHandler>, new FrameworkPropertyMetadata(null))

    static member GetAttachedMouseHandler(obj: DependencyObject) = obj.GetValue(MouseHandler.AttachedMouseHandlerProperty) :?> MouseHandler
    static member SetAttachedMouseHandler(obj: DependencyObject, value: MouseHandler) = obj.SetValue(MouseHandler.AttachedMouseHandlerProperty, box value)
    static member val AttachedMouseHandlerProperty =
        DependencyProperty.RegisterAttached("AttachedMouseHandler", typeof<MouseHandler>, typeof<Control>, new FrameworkPropertyMetadata(null, PropertyChangedCallback MouseHandler.AttachedMouseHandlerChanged))

    static member GetRootMouseHandler(obj: DependencyObject) = obj.GetValue(MouseHandler.RootMouseHandlerProperty) :?> MouseHandler
    static member SetRootMouseHandler(obj: DependencyObject, value: MouseHandler) = obj.SetValue(MouseHandler.RootMouseHandlerProperty, box value)
    static member val RootMouseHandlerProperty =
        DependencyProperty.RegisterAttached("RootMouseHandler", typeof<MouseHandler>, typeof<IInputElement>, new PropertyMetadata(null, PropertyChangedCallback MouseHandler.RootMouseHandlerChanged))

    member this.Workspace =
            match workspace with
            | Some(ws) -> ws
            | None ->
                workspace <-
                    this.PositionRoot
                    |> tryCast<FrameworkElement>
                    |> Option.bind(fun fe -> fe.DataContext |> tryCast<WorkspaceViewModel>)
                workspace |> Option.defaultValue Unchecked.defaultof<_>

    member private _.ResetWorkspace() = workspace <- None

    static member private AttachedMouseHandlerChanged (sender: DependencyObject) (e: DependencyPropertyChangedEventArgs) =
        sender |> iterOf(fun (ctrl: Control) ->
            e.OldValue |> iterOf(fun (handler: MouseHandler) ->
                ctrl.MouseDown.RemoveHandler(MouseButtonEventHandler handler.Selector_MouseDown)
                ctrl.MouseUp.RemoveHandler(MouseButtonEventHandler handler.Selector_MouseUp)
                ctrl.MouseMove.RemoveHandler(MouseEventHandler handler.Selector_MouseMove)
                ctrl.MouseLeave.RemoveHandler(MouseEventHandler handler.Selector_MouseLeave)
                ctrl.DragEnter.RemoveHandler(DragEventHandler handler.Selector_DragEnter)
                ctrl.DragOver.RemoveHandler(DragEventHandler handler.Selector_DragOver)
                ctrl.DragLeave.RemoveHandler(DragEventHandler handler.Selector_DragLeave)
                ctrl.Drop.RemoveHandler(DragEventHandler handler.Selector_Drop)
                ctrl.GiveFeedback.RemoveHandler(GiveFeedbackEventHandler handler.Selector_GiveFeedback))
            e.NewValue |> iterOf(fun (handler: MouseHandler) ->
                ctrl.MouseDown.AddHandler(MouseButtonEventHandler handler.Selector_MouseDown)
                ctrl.MouseUp.AddHandler(MouseButtonEventHandler handler.Selector_MouseUp)
                ctrl.MouseMove.AddHandler(MouseEventHandler handler.Selector_MouseMove)
                ctrl.MouseLeave.AddHandler(MouseEventHandler handler.Selector_MouseLeave)
                ctrl.DragEnter.AddHandler(DragEventHandler handler.Selector_DragEnter)
                ctrl.DragOver.AddHandler(DragEventHandler handler.Selector_DragOver)
                ctrl.DragLeave.AddHandler(DragEventHandler handler.Selector_DragLeave)
                ctrl.Drop.AddHandler(DragEventHandler handler.Selector_Drop)
                ctrl.GiveFeedback.AddHandler(GiveFeedbackEventHandler handler.Selector_GiveFeedback)))

    static member private RootMouseHandlerChanged (sender: DependencyObject) (e: DependencyPropertyChangedEventArgs) =
        match box sender, e.NewValue with
        | (:? IInputElement as ctrl), (:? MouseHandler as handler) ->
            handler.PositionRoot <- ctrl
            handler.ResetWorkspace()
        | _ -> ()

    member private this.GetOperationTarget(e: DragEventArgs, dt: IDisplayTargetViewModel) =
        ScreenOperationTarget(e.GetPosition(this.PositionRoot), dt.GetTarget())

    member private this.Selector_MouseDown (sender: obj) (e: MouseButtonEventArgs) =
        match sender with
        | TargetControl(_, dt) ->
            if e.ChangedButton = MouseButton.Left || e.ChangedButton = MouseButton.Right then
                pressed <- e.ChangedButton
                clickControl <- sender
                draggingData <- Some(ScreenOperationTarget(e.GetPosition(this.PositionRoot), dt.GetTarget()))
            e.Handled <- true
        | _ -> ()

    member private this.Selector_MouseUp (sender: obj) (e: MouseButtonEventArgs) =
        match sender with
        | TargetControl(_, _) ->
            if (clickControl = sender && pressed = e.ChangedButton && draggingData.IsSome) then
                this.Workspace.OnClick(draggingData.Value, getModifier(e.ChangedButton))
                clickControl <- null
                draggingData <- None
            e.Handled <- true
        | _ -> ()

    member private this.BeginDrag(ctrl: Control) =
        if (draggingData.IsSome && this.Workspace.BeginDrag(draggingData.Value, getModifier(pressed))) then
            draggingSource <- ctrl
            draggingGuid <- Guid.NewGuid().ToString()
            ctrl.Tag |> iterOf(fun brush -> ctrl.Foreground <- brush)
            // この中でドロップまで待機する
            ignore (DragDrop.DoDragDrop(ctrl, draggingGuid, DragDropEffects.Scroll))
            draggingData <- None
            draggingSource <- null
            draggingGuid <- ""
            ctrl.ClearValue(Control.ForegroundProperty)

    member private this.Selector_MouseMove (sender: obj) (e: MouseEventArgs) =
        match sender with
        | TargetControl(ctrl, _) ->
            if clickControl = sender && draggingData.IsSome then
                let point = e.GetPosition(this.PositionRoot)
                if (abs(point.X - draggingData.Value.Point.X) >= 5.0 ||
                    abs(point.Y - draggingData.Value.Point.Y) >= 5.0) then
                    clickControl <- null
                    if e.LeftButton = MouseButtonState.Pressed || e.RightButton = MouseButtonState.Pressed then
                        this.BeginDrag(ctrl)
            e.Handled <- true
        | _ -> ()

    member private this.Selector_MouseLeave (sender: obj) (e: MouseEventArgs) =
        match sender with
        | TargetControl(ctrl, _) ->
            if clickControl = sender then
                clickControl <- null
                if e.LeftButton = MouseButtonState.Pressed || e.RightButton = MouseButtonState.Pressed then
                    this.BeginDrag(ctrl)
            e.Handled <- true
        | _ -> ()

    member private this.Selector_DragEnter (sender: obj) (e: DragEventArgs) =
        match sender with
        | TargetControl(ctrl, dt) ->
            if isValidDropSource(e.Data) && draggingData.IsSome &&
               this.Workspace.DragEnter(draggingData.Value, this.GetOperationTarget(e, dt), getModifier(pressed)) then
                e.Effects <- DragDropEffects.Scroll
                ctrl.Tag |> iterOf(fun brush -> ctrl.Foreground <- brush)
            else
                e.Effects <- DragDropEffects.None
            e.Handled <- true
        | _ -> ()

    member private this.Selector_DragOver (sender: obj) (e: DragEventArgs) =
        match sender with
        | TargetControl(_, dt) ->
            if isValidDropSource(e.Data) && draggingData.IsSome then
                ignore (this.Workspace.DragOver(draggingData.Value, this.GetOperationTarget(e, dt), getModifier(pressed)))
            e.Handled <- true
        | _ -> ()

    member private this.Selector_DragLeave (sender: obj) (e: DragEventArgs) =
        match sender with
        | TargetControl(ctrl, dt) ->
            if isValidDropSource(e.Data) && draggingData.IsSome then
                ignore (this.Workspace.DragLeave(draggingData.Value, this.GetOperationTarget(e, dt), getModifier(pressed)))
            if ctrl <> draggingSource then
                ctrl.ClearValue(Control.ForegroundProperty)
            e.Handled <- true
        | _ -> ()

    member private this.Selector_Drop (sender: obj) (e: DragEventArgs) =
        match sender with
        | TargetControl(ctrl, dt) ->
            if isValidDropSource(e.Data) && draggingData.IsSome then
                this.Workspace.Drop(draggingData.Value, this.GetOperationTarget(e, dt), getModifier(pressed))
                ctrl.ClearValue(Control.ForegroundProperty)
            e.Handled <- true
        | _ -> ()

    member private this.Selector_GiveFeedback (sender: obj) (e: GiveFeedbackEventArgs) =
        e.UseDefaultCursors <- false
        e.Handled <- true
