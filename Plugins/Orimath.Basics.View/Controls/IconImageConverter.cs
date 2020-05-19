using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.Xml;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Orimath.Basics.View.Controls
{
    [ValueConversion(typeof(Stream), typeof(Image))]
    public class IconImageConverter : IValueConverter
    {
        private readonly BitmapImage _source;

        public IconImageConverter()
        {
            _source = new BitmapImage
            {
                CacheOption = BitmapCacheOption.OnLoad
            };
            _source.BeginInit();
            _source.UriSource = new Uri("defaultIcon.ico", UriKind.Relative);
            _source.EndInit();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Stream stream)
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
            else
            {
                return new Image
                {
                    Source = _source,
                    Stretch = Stretch.Uniform,
                    Width = 16.0,
                    Height = 16.0,
                };
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
