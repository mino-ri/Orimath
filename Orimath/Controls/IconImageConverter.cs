using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Orimath.Controls
{
    [ValueConversion(typeof(Stream), typeof(Image))]
    public class IconImageConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Stream stream)
            {
                using (stream)
                {
                    var source = new BitmapImage
                    {
                        CacheOption = BitmapCacheOption.OnLoad
                    };
                    source.BeginInit();
                    source.StreamSource = stream;
                    source.EndInit();

                    return new Image
                    {
                        Source = source,
                        Stretch = Stretch.Uniform,
                        Width = 16.0,
                        Height = 16.0,
                    };
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
