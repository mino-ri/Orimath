open System
open System.Windows
open Orimath.IO
open Orimath.Themes

[<STAThread; EntryPoint>]
let main argv =
    match Settings.load "theme" with
    | Some(t) -> ThemeBrushes.Instance <- ThemeBrushes.Load(t)
    | None -> Settings.save "theme" (ThemeBrushes.Instance.ToSerializable())

    let uri = new Uri("/Orimath;component/App.xaml", UriKind.Relative)
    let application = Application.LoadComponent(uri) :?> Application
    application.Run()
