using System;
using Mvvm;
using Orimath.Core;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class EdgeViewModel : NotifyPropertyChanged, IDisplayTargetViewModel
    {
        public EdgeViewModel(Edge edge, IViewPointConverter pointConverter)
        {
            Source = edge;
            (X1, Y1) = pointConverter.ModelToView(edge.Line.Point1);
            (X2, Y2) = pointConverter.ModelToView(edge.Line.Point2);
        }

        public Edge Source { get; }
        public double X1 { get; }
        public double Y1 { get; }
        public double X2 { get; }
        public double Y2 { get; }
        public double XFactor => Source.Line.Line.XFactor;
        public double YFactor => NearlyEquatable.Negate(Source.Line.Line.YFactor);
        public double Intercept => Source.Line.Line.Intercept;
        public double Slope => NearlyEquatable.UnaryPlus(XFactor / YFactor);
        public double Angle => NearlyEquatable.UnaryPlus(Math.Atan2(XFactor, YFactor) / Math.PI * 180.0) % 180.0;
        public double Length => Source.Line.Length;

        public DisplayTarget GetTarget() => DisplayTarget.NewEdge(Source);
        public override string ToString() => $"{Source.Line.Line}\r\n傾き:{Slope:0.#####} 角度:{Angle:0.#####}°";
    }
}
