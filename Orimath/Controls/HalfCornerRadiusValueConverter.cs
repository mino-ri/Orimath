using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Orimath.Controls
{
    [ValueConversion(typeof(double), typeof(CornerRadius))]
    public class HalfCornerRadiusValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var d = (double)value;
            return new CornerRadius(d / 2d);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
