namespace Orimath.Basics.ViewModels
open System
open Mvvm
open Orimath.Plugins

type ToolListViewModel(workspace: IWorkspace, dispatcher: IDispatcher) as this =
    inherit NotifyPropertyChanged()
    let mutable tools = None
    do workspace.CurrentToolChanged.Add(fun _ ->
        ignore (dispatcher.OnUIAsync(Action(this.OnCurrentToolChanged))))

    member this.OnCurrentToolChanged() = this.OnPropertyChanged("CurrentTool")

    member __.Tools =
        match tools with
        | Some(result) -> result
        | None ->
            let result =
                workspace.Tools
                |> Seq.map(ToolViewModel)
                |> Seq.toArray
            tools <- Some(result)
            result

    member this.CurrentTool
        with get() =
            this.Tools
            |> Array.tryFind(fun vm -> vm.Source = workspace.CurrentTool)
            |> Option.defaultValue Unchecked.defaultof<_>
        and set(v: ToolViewModel) = workspace.CurrentTool <- v.Source
