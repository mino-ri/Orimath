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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert((ModelPoint)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack((ViewPoint)value);
        }

        public ViewPoint Convert(ModelPoint point)
        {
            return new ViewPoint(point.X * Scale + OffsetX, point.Y * Scale + OffsetY);
        }

        public ModelPoint ConvertBack(ViewPoint point)
        {
            return new ModelPoint((point.X - OffsetX) / Scale, (point.Y - OffsetY) / Scale);
        }
    }
}
