using System;
using System.Collections.ObjectModel;
using Mvvm;
using Orimath.Plugins;
using ApplicativeProperty;

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
                l => new LayerViewModel(l, _pointConverter, _dispatcher),
                l => l.Dispose());

            _paper.SelectedLayers.Subscribe(values => _dispatcher.OnUIAsync(() =>
            {
                SelectedLayers = Array.ConvertAll(values, l => new LayerViewModel(l, _pointConverter, _dispatcher));
                OnPropertyChanged(nameof(SelectedLayers));
            }));
            _paper.SelectedEdges.Subscribe(values => _dispatcher.OnUIAsync(() =>
            {
                SelectedEdges = Array.ConvertAll(values, e => new EdgeViewModel(e, _pointConverter));
                OnPropertyChanged(nameof(SelectedEdges));
            }));
            _paper.SelectedLines.Subscribe(values => _dispatcher.OnUIAsync(() =>
            {
                SelectedLines = Array.ConvertAll(values, l => new LineViewModel(l, _pointConverter));
                OnPropertyChanged(nameof(SelectedLines));
            }));
            _paper.SelectedPoints.Subscribe(values => _dispatcher.OnUIAsync(() =>
            {
                SelectedPoints = Array.ConvertAll(values, p => new PointViewModel(p, _pointConverter));
                OnPropertyChanged(nameof(SelectedPoints));
            }));
        }

        public ObservableCollection<LayerViewModel> Layers => _layers;
        public LayerViewModel[] SelectedLayers { get; private set; } = Array.Empty<LayerViewModel>();
        public EdgeViewModel[] SelectedEdges { get; private set; } = Array.Empty<EdgeViewModel>();
        public LineViewModel[] SelectedLines { get; private set; } = Array.Empty<LineViewModel>();
        public PointViewModel[] SelectedPoints { get; private set; } = Array.Empty<PointViewModel>();
    }
}
