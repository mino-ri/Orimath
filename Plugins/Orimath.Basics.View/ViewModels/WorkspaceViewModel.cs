using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class WorkspaceViewModel : NotifyPropertyChanged
    {
        private readonly IWorkspace _workspace;
        private readonly IViewPointConverter _pointConverter;
        private readonly IDispatcher _dispatcher;

        public PaperViewModel Paper { get; }

        public WorkspaceViewModel(IWorkspace workspace, IViewPointConverter pointConverter, IDispatcher dispatcher)
        {
            _workspace = workspace;
            _pointConverter = pointConverter;
            _dispatcher = dispatcher;

            Paper = new PaperViewModel(_workspace.Paper, _pointConverter, _dispatcher);
        }

        private OperationTarget ToModelTarget(ScreenOperationTarget target)
        {
            return new OperationTarget(_pointConverter.ViewToModel(target.Point), target.Target);
        }

        public void OnClick(ScreenOperationTarget target, OperationModifier modifier)
        {
            if (_workspace.CurrentTool is IClickTool tool)
                _dispatcher.OnBackgroundAsync(() => tool.OnClick(ToModelTarget(target), modifier));
        }

        public bool BeginDrag(ScreenOperationTarget source, OperationModifier modifier) =>
             (_workspace.CurrentTool as IDragTool)?.BeginDrag(ToModelTarget(source), modifier) ?? false;

        public bool DragEnter(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier) =>
            (_workspace.CurrentTool as IDragTool)?.DragEnter(ToModelTarget(source), ToModelTarget(target), modifier) ?? false;

        public void DragLeave(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier) =>
            (_workspace.CurrentTool as IDragTool)?.DragLeave(ToModelTarget(source), ToModelTarget(target), modifier);

        public void DragOver(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier) =>
            (_workspace.CurrentTool as IDragTool)?.DragOver(ToModelTarget(source), ToModelTarget(target), modifier);

        public void Drop(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier)
        {
            if (_workspace.CurrentTool is IDragTool tool)
                _dispatcher.OnBackgroundAsync(() => tool.Drop(ToModelTarget(source), ToModelTarget(target), modifier));
        }
    }

    public class ScreenOperationTarget
    {
        public System.Windows.Point Point { get; }
        public DisplayTarget Target { get; }

        public ScreenOperationTarget(System.Windows.Point point, DisplayTarget target)
        {
            Point = point;
            Target = target;
        }
    }
}
