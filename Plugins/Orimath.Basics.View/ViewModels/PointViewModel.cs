using System;
using Orimath.Core;
using Orimath.Plugins;
using Orimath.Controls;

namespace Orimath.Basics.View.ViewModels
{
    public class PointViewModel : NotifyPropertyChanged, IDisplayTargetViewModel
    {
        private readonly ILayerModel? _layer;

        public PointViewModel(ILayerModel? layer, Point point, IViewPointConverter pointConverter)
        {
            _layer = layer;
            Source = point;
            (X, Y) = pointConverter.ModelToView(point);
        }

        public Point Source { get; }
        public double X { get; }
        public double Y { get; }

        public (ILayerModel, DisplayTarget) GetTarget()
        {
            if (_layer is null) throw new InvalidOperationException();
            return (_layer, DisplayTarget.NewPoint(Source));
        }
        public override string ToString() => Source.ToString();
    }
}
