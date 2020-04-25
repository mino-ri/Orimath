using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Orimath.Plugins;
using Orimath.ViewModels;

namespace Orimath.Controls
{
    public class MouseHandler : DependencyObject
    {
        public IInputElement PositionRoot { get => (IInputElement)GetValue(PositionRootProperty); set => SetValue(PositionRootProperty, value); }
        public static readonly DependencyProperty PositionRootProperty =
            DependencyProperty.Register(nameof(PositionRoot), typeof(IInputElement), typeof(MouseHandler), new FrameworkPropertyMetadata(null));

        public WorkspaceViewModel Workspace { get => (WorkspaceViewModel)GetValue(WorkspaceProperty); set => SetValue(WorkspaceProperty, value); }
        public static readonly DependencyProperty WorkspaceProperty =
            DependencyProperty.Register(nameof(Workspace), typeof(WorkspaceViewModel), typeof(MouseHandler), new FrameworkPropertyMetadata(null));

        public static MouseHandler GetAttachedMouseHandler(DependencyObject obj) => (MouseHandler)obj.GetValue(AttachedMouseHandlerProperty);
        public static void SetAttachedMouseHandler(DependencyObject obj, MouseHandler value) => obj.SetValue(AttachedMouseHandlerProperty, value);
        public static readonly DependencyProperty AttachedMouseHandlerProperty =
            DependencyProperty.RegisterAttached("AttachedMouseHandler", typeof(MouseHandler), typeof(Control), new FrameworkPropertyMetadata(null, AttachedMouseHandlerChanged));

        public static MouseHandler GetRootMouseHandler(DependencyObject obj) => (MouseHandler)obj.GetValue(RootMouseHandlerProperty);
        public static void SetRootMouseHandler(DependencyObject obj, MouseHandler value) => obj.SetValue(RootMouseHandlerProperty, value);
        public static readonly DependencyProperty RootMouseHandlerProperty =
            DependencyProperty.RegisterAttached("RootMouseHandler", typeof(MouseHandler), typeof(IInputElement), new PropertyMetadata(null, RootMouseHandlerChanged));

        private static void AttachedMouseHandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Control ctrl)) return;

            if (e.OldValue is MouseHandler old)
            {
                ctrl.MouseDown -= old.Selector_MouseDown;
                ctrl.MouseUp -= old.Selector_MouseUp;
                ctrl.MouseMove -= old.Selector_MouseMove;
                ctrl.MouseLeave -= old.Selector_MouseLeave;
                ctrl.DragEnter -= old.Selector_DragEnter;
                ctrl.DragOver -= old.Selector_DragOver;
                ctrl.DragLeave -= old.Selector_DragLeave;
                ctrl.Drop -= old.Selector_Drop;
                ctrl.GiveFeedback -= old.Selector_GiveFeedback;
            }

            if (e.NewValue is MouseHandler handler)
            {
                ctrl.MouseDown += handler.Selector_MouseDown;
                ctrl.MouseUp += handler.Selector_MouseUp;
                ctrl.MouseMove += handler.Selector_MouseMove;
                ctrl.MouseLeave += handler.Selector_MouseLeave;
                ctrl.DragEnter += handler.Selector_DragEnter;
                ctrl.DragOver += handler.Selector_DragOver;
                ctrl.DragLeave += handler.Selector_DragLeave;
                ctrl.Drop += handler.Selector_Drop;
                ctrl.GiveFeedback += handler.Selector_GiveFeedback;
            }
        }

        private static void RootMouseHandlerChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is IInputElement ctrl && e.NewValue is MouseHandler handler)
                handler.PositionRoot = ctrl;
        }

        private object? _clickControl;
        private string _draggingGuid = "";
        private MouseButton _pressed;
        private Control? _draggingSource;
        private ScreenOperationTarget? _draggingData;

        private OperationModifier GetModifier(MouseButton mouseButton)
        {
            var result = mouseButton == MouseButton.Right
                    ? OperationModifier.RightButton
                    : OperationModifier.None;

            var keys = Keyboard.Modifiers;
            if (keys.HasFlag(ModifierKeys.Alt)) result |= OperationModifier.Alt;
            if (keys.HasFlag(ModifierKeys.Control)) result |= OperationModifier.Ctrl;
            if (keys.HasFlag(ModifierKeys.Shift)) result |= OperationModifier.Shift;
            return result;
        }

        private ScreenOperationTarget GetOperationTarget(DragEventArgs e, IDisplayTargetViewModel dt)
        {
            return new ScreenOperationTarget(e.GetPosition(PositionRoot), dt.GetTarget());
        }

        private bool IsValidDropSource(IDataObject data)
        {
            return data.GetData(typeof(string)) is string guid && guid == _draggingGuid;
        }

        private void Selector_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel dt)) return;

            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                _pressed = e.ChangedButton;
                _clickControl = sender;
                _draggingData = new ScreenOperationTarget(e.GetPosition(PositionRoot), dt.GetTarget());
            }

            e.Handled = true;
        }

        private void Selector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel)) return;

            if (_clickControl == sender && _pressed == e.ChangedButton)
            {
                Workspace.CurrentTool.OnClick(_draggingData, GetModifier(e.ChangedButton));
                _clickControl = null;
                _draggingData = null;
            }

            e.Handled = true;
        }

        private void BeginDrag(Control ctrl)
        {
            if (Workspace.CurrentTool.BeginDrag(_draggingData, GetModifier(_pressed)))
            {
                _draggingSource = ctrl;
                _draggingGuid = Guid.NewGuid().ToString();
                ctrl.Foreground = (Brush)ctrl.Tag;
                // この中でドロップまで待機する
                DragDrop.DoDragDrop(ctrl, _draggingGuid, DragDropEffects.Scroll);
                _draggingData = null;
                _draggingSource = null;
                _draggingGuid = "";
                ctrl.ClearValue(Control.ForegroundProperty);
            }
        }

        private void Selector_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel)) return;

            if (_clickControl == sender && _draggingData is { })
            {
                var point = e.GetPosition(PositionRoot);
                if (Math.Abs(point.X - _draggingData.Point.X) >= 5.0 ||
                    Math.Abs(point.Y - _draggingData.Point.Y) >= 5.0)
                {
                    _clickControl = null;
                    if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                        BeginDrag(ctrl);
                }
            }

            e.Handled = true;
        }

        private void Selector_MouseLeave(object sender, MouseEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel)) return;

            if (_clickControl == sender)
            {
                _clickControl = null;
                if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                    BeginDrag(ctrl);
            }

            e.Handled = true;
        }

        private void Selector_DragEnter(object sender, DragEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel dt)) return;

            if (IsValidDropSource(e.Data) && _draggingData is { } &&
                Workspace.CurrentTool.DragEnter(_draggingData, GetOperationTarget(e, dt), GetModifier(_pressed)))
            {
                e.Effects = DragDropEffects.Scroll;
                ctrl.Foreground = (Brush)ctrl.Tag;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private void Selector_DragOver(object sender, DragEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel dt)) return;

            if (IsValidDropSource(e.Data) && _draggingData is { })
                Workspace.CurrentTool.DragOver(_draggingData, GetOperationTarget(e, dt), GetModifier(_pressed));

            e.Handled = true;
        }

        private void Selector_DragLeave(object sender, DragEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel dt)) return;

            if (IsValidDropSource(e.Data) && _draggingData is { })
                Workspace.CurrentTool.DragLeave(_draggingData, GetOperationTarget(e, dt), GetModifier(_pressed));

            if (ctrl != _draggingSource)
                ctrl.ClearValue(Control.ForegroundProperty);

            e.Handled = true;
        }

        private void Selector_Drop(object sender, DragEventArgs e)
        {
            if (!(sender is Control ctrl && ctrl.DataContext is IDisplayTargetViewModel dt)) return;

            if (IsValidDropSource(e.Data) && _draggingData is { })
            {
                Workspace.CurrentTool.Drop(_draggingData, GetOperationTarget(e, dt), GetModifier(_pressed));
                ctrl.ClearValue(Control.ForegroundProperty);
                e.Handled = true;
            }
        }

        private void Selector_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            e.Handled = true;
        }
    }
}
