using System;
using System.Globalization;
using System.Windows.Data;

namespace Orimath.Controls
{
    [ValueConversion(typeof(double), typeof(double))]
    public class SubtractionValueConverter : IValueConverter
    {
        public double Amount { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Max(0d, (double)value - Amount);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
