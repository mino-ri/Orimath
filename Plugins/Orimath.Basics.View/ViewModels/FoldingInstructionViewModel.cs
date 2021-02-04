using System.Linq;
using Orimath.Controls;
using Orimath.Plugins;
using Orimath.FoldingInstruction;
using ApplicativeProperty;

namespace Orimath.Basics.View.ViewModels
{
    public class FoldingInstructionViewModel : NotifyPropertyChanged
    {
        private readonly IDispatcher _dispatcher;
        private readonly IViewPointConverter _pointConverter;
        private CompositeDisposable? _disposables;

        public ResettableObservableCollection<InstructionLineViewModel> Lines { get; } = new();
        public ResettableObservableCollection<InstructionArrowViewModel> Arrows { get; } = new();
        public ResettableObservableCollection<InstructionPointViewModel> Points { get; } = new();

        public FoldingInstructionViewModel(IWorkspace workspace, IDispatcher dispatcher, IViewPointConverter pointConverter)
        {
            _dispatcher = dispatcher;
            _pointConverter = pointConverter;
            workspace.CurrentTool.Subscribe(Workspace_CurrentToolChanged);
        }

        private void Workspace_CurrentToolChanged(ITool tool)
        {
            _disposables?.Dispose();
            _disposables = null;

            if ((tool as IFoldingInstructionTool)?.FoldingInstruction is { } _foldingInstruction)
            {
                _foldingInstruction.Lines.Subscribe(Model_LinesChanged);
                _foldingInstruction.Arrows.Subscribe(Model_ArrowsChanged);
                _foldingInstruction.Points.Subscribe(Model_PointsChanged);

                _dispatcher.OnUIAsync(() =>
                {
                    Lines.Reset(_foldingInstruction.Lines.Value.Select(x => new InstructionLineViewModel(_pointConverter, x)));
                    Arrows.Reset(_foldingInstruction.Arrows.Value.Select(x => new InstructionArrowViewModel(_pointConverter, x)));
                    Points.Reset(_foldingInstruction.Points.Value.Select(x => new InstructionPointViewModel(_pointConverter, x)));
                    Visible = true;
                });

            }
            else
            {
                _dispatcher.OnUIAsync(() =>
                {
                    Lines.Clear();
                    Arrows.Clear();
                    Points.Clear();
                    Visible = false;
                });
            }
        }

        private void Model_ArrowsChanged(InstructionArrow[] arrows)
        {
            _dispatcher.OnUIAsync(() =>
            {
                if (arrows.Length == Arrows.Count)
                {
                    for (var i = 0; i < arrows.Length; i++)
                        Arrows[i].SetModel(arrows[i]);
                }
                else
                {
                    Arrows.Reset(arrows.Select(x => new InstructionArrowViewModel(_pointConverter, x)));
                }
            });
        }

        private void Model_LinesChanged(InstructionLine[] lines)
        {
            _dispatcher.OnUIAsync(() =>
            {
                if (lines.Length == Lines.Count)
                {
                    for (var i = 0; i < lines.Length; i++)
                        Lines[i].SetModel(lines[i]);
                }
                else
                {
                    Lines.Reset(lines.Select(x => new InstructionLineViewModel(_pointConverter, x)));
                }
            });
        }

        private void Model_PointsChanged(InstructionPoint[] points)
        {
            _dispatcher.OnUIAsync(() =>
            {
                if (points.Length == Points.Count)
                {
                    for (var i = 0; i < points.Length; i++)
                        Points[i].SetModel(points[i]);
                }
                else
                {
                    Points.Reset(points.Select(x => new InstructionPointViewModel(_pointConverter, x)));
                }
            });
        }

        private bool _visible;
        public bool Visible { get => _visible; set => SetValue(ref _visible, value); }
    }
}
