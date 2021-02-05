namespace Orimath.Controls
open System.Windows
open Orimath.ViewModels

type App() =
    inherit Application()

    member this.Application_Startup(_: obj, _: StartupEventArgs) =
        (this.Resources.["rootViewModel"] :?> RootViewModel).Workspace.LoadSetting()

    member this.Application_Exit(_: obj, _: ExitEventArgs) =
        (this.Resources.["rootViewModel"] :?> RootViewModel).Workspace.SaveSetting()
