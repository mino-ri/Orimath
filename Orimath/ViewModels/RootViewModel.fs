namespace Orimath.ViewModels
open Orimath.Core
open Orimath.Controls

type RootViewModel() =
    inherit NotifyPropertyChanged()
    member val Workspace = WorkspaceViewModel(Workspace())
