open System
open System.Windows

[<STAThread; EntryPoint>]
let main argv = 
    let uri = new Uri("/Orimath;component/App.xaml", UriKind.Relative)
    let application = Application.LoadComponent(uri) :?> Application
    application.Run()
