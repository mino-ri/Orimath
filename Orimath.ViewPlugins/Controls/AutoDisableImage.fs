namespace Orimath.Controls
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Media.Imaging

type internal FpmOptions = FrameworkPropertyMetadataOptions

type AutoDisableImage() =
    inherit Image()
    static let mutable init = false
    do
        if not init then
            init <- true
            AutoDisableImage.IsEnabledProperty.OverrideMetadata(
                typeof<AutoDisableImage>,
                new FrameworkPropertyMetadata(true, FpmOptions.AffectsRender))
            AutoDisableImage.SourceProperty.OverrideMetadata(
                typeof<AutoDisableImage>,
                new FrameworkPropertyMetadata(
                    null,
                    FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender,
                    PropertyChangedCallback AutoDisableImage.OnSourceChanged))

    member val GraySource: BitmapSource option = None with get, set

    override this.OnRender(dc: DrawingContext) =
        if this.IsEnabled then
            base.OnRender(dc)
        elif this.GraySource.IsSome then
            dc.DrawImage(this.GraySource.Value, Rect(Point(), this.RenderSize))

    static member private OnSourceChanged(d: DependencyObject) (_: DependencyPropertyChangedEventArgs) =
        let image = d :?> AutoDisableImage
        match image.Source with
        | :? BitmapSource as imageSoure ->
            let bitmap = new FormatConvertedBitmap(imageSoure, PixelFormats.Bgra32, null, 0.0)
            let width = bitmap.PixelWidth
            let height = bitmap.PixelHeight
            let pixels = Array.zeroCreate (width * height * 4) : byte[]
            let stride = (width * bitmap.Format.BitsPerPixel + 7) / 8
            bitmap.CopyPixels(pixels, stride, 0)
            for i in 0..4..pixels.Length - 1 do
                let b = float pixels.[i]
                let g = float pixels.[i + 1]
                let r = float pixels.[i + 2]
                let gray = int (r * 0.298912 + g * 0.586611 + b * 0.114478)
                let v = if gray > 255 then 255uy else byte gray
                pixels.[i] <- v
                pixels.[i + 1] <- v
                pixels.[i + 2] <- v
                pixels.[i + 3] <- byte (float pixels.[i + 3] * 0.75)
            image.GraySource <-
                Some(BitmapSource.Create(width, height, bitmap.DpiX, bitmap.DpiY, PixelFormats.Bgra32, null, pixels, stride))
        | _ -> ()
