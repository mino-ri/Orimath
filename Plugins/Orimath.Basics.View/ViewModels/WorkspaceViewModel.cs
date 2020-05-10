using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class WorkspaceViewModel : NotifyPropertyChanged
    {
        private IWorkspace _workspace;
        private IViewPointConverter _pointConverter;
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

        public void OnClick(ScreenOperationTarget target, OperationModifier modifier) =>
            _dispatcher.OnBackgroundAsync(() => _workspace.CurrentTool.OnClick(ToModelTarget(target), modifier));

        public bool BeginDrag(ScreenOperationTarget source, OperationModifier modifier) =>
            _workspace.CurrentTool.BeginDrag(ToModelTarget(source), modifier);

        public bool DragEnter(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier) =>
            _workspace.CurrentTool.DragEnter(ToModelTarget(source), ToModelTarget(target), modifier);

        public void DragLeave(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier) =>
            _workspace.CurrentTool.DragLeave(ToModelTarget(source), ToModelTarget(target), modifier);

        public void DragOver(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier) =>
            _workspace.CurrentTool.DragOver(ToModelTarget(source), ToModelTarget(target), modifier);

        public void Drop(ScreenOperationTarget source, ScreenOperationTarget target, OperationModifier modifier) =>
            _dispatcher.OnBackgroundAsync(() => _workspace.CurrentTool.Drop(ToModelTarget(source), ToModelTarget(target), modifier));
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
