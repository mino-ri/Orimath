using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Orimath.UITest.ViewModels
{
    [ValueConversion(typeof(bool), typeof(VerticalAlignment))]
    public class VerticalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => 
            value is bool @bool && @bool
                ? VerticalAlignment.Stretch
                : VerticalAlignment.Center;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }

    [ValueConversion(typeof(bool), typeof(HorizontalAlignment))]
    public class HorizontalAlignmentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is bool @bool && @bool
                ? HorizontalAlignment.Stretch
                : HorizontalAlignment.Center;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
