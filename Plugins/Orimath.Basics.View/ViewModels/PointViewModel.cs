using Orimath.Core;
using Orimath.Plugins;
using Mvvm;

namespace Orimath.Basics.View.ViewModels
{
    public class PointViewModel : NotifyPropertyChanged, IDisplayTargetViewModel
    {
        public PointViewModel(Point point, IViewPointConverter pointConverter)
        {
            Source = point;
            (X, Y) = pointConverter.ModelToView(point);
        }

        public Point Source { get; }
        public double X { get; }
        public double Y { get; }

        public DisplayTarget GetTarget() => DisplayTarget.NewPoint(Source);
        public override string ToString() => Source.ToString();
    }
}
