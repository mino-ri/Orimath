namespace Orimath.ViewModels
open System
open System.Collections.ObjectModel
open Orimath.Plugins
open Orimath.Plugins.ThreadController

type WorkspaceViewModel(workspace: IWorkspace, invoker: IUIThreadInvoker) as this =
    inherit NotifyPropertyChanged()
    let pointConverter = PointConverter(512.0, 16.0, 16.0)
    let mutable currentTool = ToolViewModel(workspace.CurrentTool, pointConverter)
    let currentToolDscnt = workspace.CurrentToolChanged.Subscribe(this.CurrentToolChanged)

    let tools = ResettableObservableCollection()
    let effects = ResettableObservableCollection()
    member __.Tools = tools :> ObservableCollection<_>
    member __.Effects = effects :> ObservableCollection<_>

    member __.Source = workspace
    member val Paper = new PaperViewModel(workspace.Paper, pointConverter, invoker)
    member __.PointConverter = pointConverter
    member __.CurrentTool = currentTool
    
    member private this.CurrentToolChanged(_: EventArgs) =
        onUI invoker <| fun () ->
            currentTool <- this.Tools |> Seq.find(fun t -> t.Source = workspace.CurrentTool)
            this.OnPropertyChanged("CurrentTool")

    member __.Initialize() =
        workspace.Initialize()
        onUI invoker <| fun () ->
            tools.Reset(workspace.Tools |> Seq.map(fun t -> ToolViewModel(t, pointConverter)))
            workspace.CurrentTool <- workspace.Tools.[0]
        onUI invoker <| fun () ->
            effects.Reset(workspace.Effects |> Seq.map(fun e -> EffectViewModel(e, invoker)))

    member this.Dispose() =
        currentToolDscnt.Dispose()
        this.Paper.Dispose()

    interface IDisposable with
        member this.Dispose() = this.Dispose()
