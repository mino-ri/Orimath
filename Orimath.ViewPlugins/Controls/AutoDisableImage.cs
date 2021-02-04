using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FpmOptions = System.Windows.FrameworkPropertyMetadataOptions;

namespace Orimath.Controls
{
    public class AutoDisableImage : Image
    {
        private BitmapSource? _graySource;

        static AutoDisableImage()
        {
            IsEnabledProperty.OverrideMetadata(typeof(AutoDisableImage),
                new FrameworkPropertyMetadata(true, FpmOptions.AffectsRender));
            SourceProperty.OverrideMetadata(typeof(AutoDisableImage),
                new FrameworkPropertyMetadata(null, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender,
                    OnSourceChanged));
        }

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not AutoDisableImage image || image.Source is not BitmapSource imageSoure)
                return;

            var bitmap = new FormatConvertedBitmap(imageSoure, PixelFormats.Bgra32, null, 0.0);
            var width = bitmap.PixelWidth;
            var height = bitmap.PixelHeight;
            var pixels = new byte[width * height * 4];
            var stride = (width * bitmap.Format.BitsPerPixel + 7) / 8;
            bitmap.CopyPixels(pixels, stride, 0);

            for (var i = 0; i < pixels.Length; i += 4)
            {
                var b = pixels[i];
                var g = pixels[i + 1];
                var r = pixels[i + 2];
                var gray = (int)(r * 0.298912 + g * 0.586611 + b * 0.114478);
                var v = gray > 255 ? (byte)255 : (byte)gray;

                pixels[i] = v;
                pixels[i + 1] = v;
                pixels[i + 2] = v;
                pixels[i + 3] = (byte)(pixels[i + 3] * 0.75);
            }

            image._graySource = BitmapSource.Create(width, height, bitmap.DpiX, bitmap.DpiY, PixelFormats.Bgra32, null, pixels, stride);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (IsEnabled)
            {
                base.OnRender(dc);
            }
            else if (_graySource is not null)
            {
                dc.DrawImage(_graySource, new Rect(new Point(), RenderSize));
            }
        }
    }
}
