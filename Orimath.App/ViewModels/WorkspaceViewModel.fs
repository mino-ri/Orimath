namespace Orimath.App.ViewModels
open Orimath.Core
open Orimath.Plugins
open Orimath.ViewPlugins

type WorkspaceViewModel(workspace: IWorkspace) =
    inherit NotifyPropertyChanged()
    let ws = workspace