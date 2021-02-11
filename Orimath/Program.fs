open System
open System.Globalization
open System.Windows
open Orimath.IO
open Orimath.Themes

[<STAThread; EntryPoint>]
let main argv =
    CultureInfo.CurrentCulture <- CultureInfo.InvariantCulture
    CultureInfo.CurrentUICulture <- CultureInfo.InvariantCulture
    match Settings.load "theme" with
    | Some(t) -> ThemeBrushes.Instance <- ThemeBrushes.Load(t)
    | None -> Settings.save "theme" (ThemeBrushes.Instance.ToSerializable())
    let uri = Uri("/Orimath;component/App.xaml", UriKind.Relative)
    let application = Application.LoadComponent(uri) :?> Application
    application.Run()
