using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Orimath.Controls;
using Orimath.Core;
using Orimath.Plugins;
using LineSegment = Orimath.Core.LineSegment;

namespace Orimath.Basics.View.ViewModels
{
    public sealed class LayerViewModel : NotifyPropertyChanged, IDisposable, IDisplayTargetViewModel
    {
        private readonly AttachedObservableCollection<Point, PointViewModel> _points;
        private readonly AttachedObservableCollection<LineSegment, LineViewModel> _lines;
        private readonly IViewPointConverter _pointConverter;

        public LayerViewModel(ILayerModel layer, IViewPointConverter pointConverter, IDispatcher dispatcher)
        {
            _pointConverter = pointConverter;
            Source = layer;
            Edges = layer.Edges.Select(e => new EdgeViewModel(layer, e, _pointConverter)).ToArray();
            Vertexes = new PointCollection(layer.Edges.Select(e => _pointConverter.ModelToView(e.Line.Point1)));

            _points = new AttachedObservableCollection<Point, PointViewModel>(
                dispatcher,
                layer.Points,
                p => new PointViewModel(layer, p, _pointConverter),
                _ => { });
            _lines = new AttachedObservableCollection<LineSegment, LineViewModel>(
                dispatcher,
                layer.Lines,
                l => new LineViewModel(layer, l, _pointConverter),
                _ => { });
        }

        public ILayerModel Source { get; }
        public EdgeViewModel[] Edges { get; }
        public PointCollection Vertexes { get; }
        public ObservableCollection<PointViewModel> Points => _points;
        public ObservableCollection<LineViewModel> Lines => _lines;
        public LayerType LayerType => Source.LayerType;

        public (ILayerModel, DisplayTarget) GetTarget() => (Source, DisplayTarget.NewLayer(Source));

        public void Dispose()
        {
            _points.Dispose();
            _lines.Dispose();
        }
    }
}
