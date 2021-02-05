namespace Orimath.Controls
open System
open System.IO
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media
open System.Windows.Media.Imaging

[<ValueConversion(typeof<Stream>, typeof<Image>)>]
type IconImageConverter() =
    interface IValueConverter with
        member _.Convert(value, _, _, _) =
            match value with
            | :? Stream as stream ->
                use stream = stream
                let source = BitmapImage()
                source.CacheOption <- BitmapCacheOption.OnLoad
                source.BeginInit()
                source.StreamSource <- stream
                source.EndInit()
                let result = AutoDisableImage()
                result.Source <- source
                result.Stretch <- Stretch.Uniform
                result.Width <- 16.0
                result.Height <- 16.0
                box result
            | _ -> null

        member _.ConvertBack(_, _, _, _) = raise (NotImplementedException())
