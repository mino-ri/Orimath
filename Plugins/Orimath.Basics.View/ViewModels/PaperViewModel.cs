using Mvvm;
using Orimath.Plugins;
using System;
using System.Collections.ObjectModel;

namespace Orimath.Basics.View.ViewModels
{
    public class PaperViewModel : NotifyPropertyChanged
    {
        private readonly AttachedObservableCollection<ILayerModel, LayerViewModel> _layers;
        private readonly IPaperModel _paper;
        private readonly IViewPointConverter _pointConverter;
        private readonly IDispatcher _dispatcher;

        public PaperViewModel(IPaperModel paper, IViewPointConverter pointConverter, IDispatcher dispatcher)
        {
            _paper = paper;
            _pointConverter = pointConverter;
            _dispatcher = dispatcher;

            _layers = new AttachedObservableCollection<ILayerModel, LayerViewModel>(
                _dispatcher,
                _paper.Layers,
                h => _paper.LayerChanged += h,
                h => _paper.LayerChanged -= h,
                l => new LayerViewModel(l, _pointConverter, _dispatcher),
                l => l.Dispose());

            _paper.SelectedLayersChanged += (_, _) => _dispatcher.OnUIAsync(() =>
            {
                SelectedLayers = Array.ConvertAll(_paper.SelectedLayers, l => new LayerViewModel(l, _pointConverter, _dispatcher));
                OnPropertyChanged(nameof(SelectedLayers));
            });
            _paper.SelectedEdgesChanged += (_, _) => _dispatcher.OnUIAsync(() =>
            {
                SelectedEdges = Array.ConvertAll(_paper.SelectedEdges, e => new EdgeViewModel(e, _pointConverter));
                OnPropertyChanged(nameof(SelectedEdges));
            });
            _paper.SelectedLinesChanged += (_, _) => _dispatcher.OnUIAsync(() =>
            {
                SelectedLines = Array.ConvertAll(_paper.SelectedLines, l => new LineViewModel(l, _pointConverter));
                OnPropertyChanged(nameof(SelectedLines));
            });
            _paper.SelectedPointsChanged += (_, _) => _dispatcher.OnUIAsync(() =>
            {
                SelectedPoints = Array.ConvertAll(_paper.SelectedPoints, p => new PointViewModel(p, _pointConverter));
                OnPropertyChanged(nameof(SelectedPoints));
            });
        }

        public ObservableCollection<LayerViewModel> Layers => _layers;
        public LayerViewModel[] SelectedLayers { get; private set; } = Array.Empty<LayerViewModel>();
        public EdgeViewModel[] SelectedEdges { get; private set; } = Array.Empty<EdgeViewModel>();
        public LineViewModel[] SelectedLines { get; private set; } = Array.Empty<LineViewModel>();
        public PointViewModel[] SelectedPoints { get; private set; } = Array.Empty<PointViewModel>();
    }
}
