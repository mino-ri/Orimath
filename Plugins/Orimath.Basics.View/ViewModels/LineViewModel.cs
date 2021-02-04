﻿using System;
using Orimath.Controls;
using Orimath.Core;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class LineViewModel : NotifyPropertyChanged, IDisplayTargetViewModel
    {
        private readonly ILayerModel? _layer;

        public LineViewModel(ILayerModel? layer, LineSegment line, IViewPointConverter pointConverter)
        {
            _layer = layer;
            Source = line;
            (X1, Y1) = pointConverter.ModelToView(line.Point1);
            (X2, Y2) = pointConverter.ModelToView(line.Point2);
        }

        public LineSegment Source { get; }
        public double X1 { get; }
        public double Y1 { get; }
        public double X2 { get; }
        public double Y2 { get; }
        public double XFactor => Source.Line.XFactor;
        public double YFactor => NearlyEquatable.Negate(Source.Line.YFactor);
        public double Intercept => Source.Line.Intercept;
        public double Slope => NearlyEquatable.UnaryPlus(XFactor / YFactor);
        public double Angle => NearlyEquatable.UnaryPlus(Math.Atan2(XFactor, YFactor) / Math.PI * 180.0) % 180.0;
        public double Length => Source.Length;

        public (ILayerModel, DisplayTarget) GetTarget()
        {
            if (_layer is null) throw new InvalidOperationException();
            return (_layer, DisplayTarget.NewLine(Source));
        }
        public override string ToString() => $"{Source.Line}\r\n傾き:{Slope:0.#####} 角度:{Angle:0.#####}°";
    }
}
