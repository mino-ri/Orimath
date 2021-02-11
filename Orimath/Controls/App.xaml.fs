namespace Orimath.Controls
open System.Windows
open Orimath.ViewModels

type App() =
    inherit Application()

    member this.ApplicationStartup(_: obj, _: StartupEventArgs) =
        (this.Resources.["rootViewModel"] :?> RootViewModel).Workspace.LoadSetting()

    member this.ApplicationExit(_: obj, _: ExitEventArgs) =
        (this.Resources.["rootViewModel"] :?> RootViewModel).Workspace.SaveSetting()
