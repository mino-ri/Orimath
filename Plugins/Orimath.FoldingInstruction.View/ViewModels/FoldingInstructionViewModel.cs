using System;
using System.Linq;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.FoldingInstruction.View.ViewModels
{
    public class FoldingInstructionViewModel : NotifyPropertyChanged
    {
        private readonly IWorkspace _workspace;
        private readonly IDispatcher _dispatcher;
        private readonly IViewPointConverter _pointConverter;
        private FoldingInstruction? _foldingInstruction;

        public ResettableObservableCollection<InstructionLineViewModel> Lines { get; } = new();
        public ResettableObservableCollection<InstructionArrowViewModel> Arrows { get; } = new();
        public ResettableObservableCollection<InstructionPointViewModel> Points { get; } = new();

        public FoldingInstructionViewModel(IWorkspace workspace, IDispatcher dispatcher, IViewPointConverter pointConverter)
        {
            _workspace = workspace;
            _dispatcher = dispatcher;
            _pointConverter = pointConverter;
            workspace.CurrentToolChanged += Workspace_CurrentToolChanged;
        }

        private void Workspace_CurrentToolChanged(object? sender, EventArgs e)
        {
            if (_foldingInstruction is not null)
            {
                _foldingInstruction.LinesChanged -= Model_LinesChanged;
                _foldingInstruction.ArrowsChanged -= Model_ArrowsChanged;
                _foldingInstruction.PointsChanged -= Model_PointsChanged;
            }

            _foldingInstruction = (_workspace.CurrentTool as IFoldingInstructionTool)?.FoldingInstruction;
            if (_foldingInstruction is not null)
            {
                _foldingInstruction.LinesChanged += Model_LinesChanged;
                _foldingInstruction.ArrowsChanged += Model_ArrowsChanged;
                _foldingInstruction.PointsChanged += Model_PointsChanged;

                _dispatcher.OnUIAsync(() =>
                {
                    Lines.Reset(_foldingInstruction.Lines.Select(x => new InstructionLineViewModel(_pointConverter, x)));
                    Arrows.Reset(_foldingInstruction.Arrows.Select(x => new InstructionArrowViewModel(_pointConverter, x)));
                    Points.Reset(_foldingInstruction.Points.Select(x => new InstructionPointViewModel(_pointConverter, x)));
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

        private void Model_ArrowsChanged(object? sender, EventArgs e)
        {
            if (sender is not FoldingInstruction foldingInstruction) return;
            
            var arrows = foldingInstruction.Arrows;
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

        private void Model_LinesChanged(object? sender, EventArgs e)
        {
            if (sender is not FoldingInstruction foldingInstruction) return;

            var lines = foldingInstruction.Lines;
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

        private void Model_PointsChanged(object? sender, EventArgs e)
        {
            if (sender is not FoldingInstruction foldingInstruction) return;

            var points = foldingInstruction.Points;
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
