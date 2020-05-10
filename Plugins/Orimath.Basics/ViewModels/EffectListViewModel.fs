namespace Orimath.Basics.ViewModels
open Mvvm
open Orimath.Plugins

type EffectListViewModel(workspace: IWorkspace, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let mutable effects = None
    member __.Effects =
        match effects with
        | Some(result) -> result
        | None ->
            let result =
                workspace.Effects
                |> Seq.map(fun e -> EffectViewModel(e, dispatcher))
                |> Seq.toArray
            effects <- Some(result)
            result
