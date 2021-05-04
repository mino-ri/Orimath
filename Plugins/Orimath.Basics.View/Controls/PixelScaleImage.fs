namespace Orimath.Basics.View.Controls
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Media.Imaging

type PixelScaleImage() =
    inherit Image()
    override this.MeasureOverride(constraintSize) =
        match this.Source with
        | :? BitmapSource as bitmap ->
            let dpiScale = VisualTreeHelper.GetDpi(this)
            Size(float bitmap.PixelWidth / dpiScale.DpiScaleX, float bitmap.PixelHeight / dpiScale.DpiScaleY)
        | _ -> base.MeasureOverride(constraintSize)

    override this.ArrangeOverride(_) =
        Size(this.DesiredSize.Width, this.DesiredSize.Height)
