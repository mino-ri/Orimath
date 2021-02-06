namespace Orimath.Basics.View.Controls
open System
open System.IO
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Media
open System.Windows.Media.Imaging
open Orimath.Controls

[<ValueConversion(typeof<Stream>, typeof<Image>)>]
type IconImageConverter() =
    let source = BitmapImage()
    do
        source.CacheOption <- BitmapCacheOption.OnLoad
        source.BeginInit()
        source.UriSource <- Uri("pack://application:,,,/Orimath.Basics.View;component/defaultIcon.png")
        source.EndInit()

    interface IValueConverter with
        member _.Convert(value, _, _, _) =
            let src =
                match value with
                | :? Stream as stream ->
                    use stream = stream
                    let src = BitmapImage()
                    src.CacheOption <- BitmapCacheOption.OnLoad
                    src.BeginInit()
                    src.StreamSource <- stream
                    src.EndInit()
                    src
                | _ -> source
            let result = AutoDisableImage()
            result.Source <- src
            result.Stretch <- Stretch.Uniform
            result.Width <- 16.0
            result.Height <- 16.0
            box result

        member _.ConvertBack(_, _, _, _) = raise (NotImplementedException())
