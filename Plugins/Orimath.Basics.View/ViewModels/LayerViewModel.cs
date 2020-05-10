using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using Mvvm;
using Orimath.Core;
using Orimath.Plugins;
using LineSegment = Orimath.Core.LineSegment;

namespace Orimath.Basics.View.ViewModels
{
    public sealed class LayerViewModel : NotifyPropertyChanged, IDisposable, IDisplayTargetViewModel
    {
        private readonly AttachedObservableCollection<Point, PointViewModel> _points;
        private readonly AttachedObservableCollection<LineSegment, LineViewModel> _lines;
        private IViewPointConverter _pointConverter;

        public LayerViewModel(ILayerModel layer, IViewPointConverter pointConverter, IDispatcher dispatcher)
        {
            _pointConverter = pointConverter;
            Source = layer;
            Edges = layer.Edges.Select(e => new EdgeViewModel(e, _pointConverter)).ToArray();
            Vertexes = new PointCollection(layer.Edges.Select(e => _pointConverter.ModelToView(e.Line.Point1)));

            _points = new AttachedObservableCollection<Point, PointViewModel>(
                dispatcher,
                layer.Points,
                h => Source.PointChanged += h,
                h => Source.PointChanged -= h,
                p => new PointViewModel(p, _pointConverter),
                _ => { });
            _lines = new AttachedObservableCollection<LineSegment, LineViewModel>(
                dispatcher,
                layer.Lines,
                h => Source.LineChanged += h,
                h => Source.LineChanged -= h,
                l => new LineViewModel(l, _pointConverter),
                _ => { });
        }

        public ILayerModel Source { get; }
        public EdgeViewModel[] Edges { get; }
        public PointCollection Vertexes { get; }
        public ObservableCollection<PointViewModel> Points => _points;
        public ObservableCollection<LineViewModel> Lines => _lines;

        public DisplayTarget GetTarget() => DisplayTarget.NewLayer(Source);

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            _points.Dispose();
            _lines.Dispose();
        }
    }
}
