using System;
using System.Globalization;
using System.Windows.Data;
using ViewPoint = System.Windows.Point;
using ModelPoint = Orimath.Core.Point;

namespace Orimath.ViewPlugins
{
    [ValueConversion(typeof(ModelPoint), typeof(ViewPoint))]
    public class ScreenPointValueConverter : IValueConverter
    {
        public double OffsetX { get; set; }

        public double OffsetY { get; set; }

        public double Scale { get; set; } = 512.0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Convert((ModelPoint)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => ConvertBack((ViewPoint)value);

        public ViewPoint Convert(ModelPoint point) =>
            new ViewPoint(point.X * Scale + OffsetX, point.Y * Scale + OffsetY);

        public ModelPoint ConvertBack(ViewPoint point) =>
            new ModelPoint((point.X - OffsetX) / Scale, (point.Y - OffsetY) / Scale);
    }
}
