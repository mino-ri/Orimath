using System;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class MeasureViewModel : NotifyPropertyChanged
    {
        private readonly IPaperModel _paper;
        private readonly IViewPointConverter _pointConverter;
        private readonly IDispatcher _dispatcher;

        public MeasureViewModel(IPaperModel paper, IViewPointConverter pointConverter, IDispatcher dispatcher)
        {
            _paper = paper;
            _pointConverter = pointConverter;
            _dispatcher = dispatcher;

            _paper.SelectedEdgesChanged += (_, __) => _dispatcher.OnUIAsync(() =>
            {
                SelectedEdges = Array.ConvertAll(_paper.SelectedEdges, e => new EdgeViewModel(e, _pointConverter));
                OnPropertyChanged(nameof(SelectedEdges));
            });
            _paper.SelectedLinesChanged += (_, __) => _dispatcher.OnUIAsync(() =>
            {
                SelectedLines = Array.ConvertAll(_paper.SelectedLines, l => new LineViewModel(l, _pointConverter));
                OnPropertyChanged(nameof(SelectedLines));
            });
            _paper.SelectedPointsChanged += (_, __) => _dispatcher.OnUIAsync(() =>
            {
                SelectedPoints = Array.ConvertAll(_paper.SelectedPoints, p => new PointViewModel(p, _pointConverter));
                OnPropertyChanged(nameof(SelectedPoints));
            });
        }

        public EdgeViewModel[] SelectedEdges { get; private set; } = Array.Empty<EdgeViewModel>();
        public LineViewModel[] SelectedLines { get; private set; } = Array.Empty<LineViewModel>();
        public PointViewModel[] SelectedPoints { get; private set; } = Array.Empty<PointViewModel>();
    }
}
