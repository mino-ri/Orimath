namespace Orimath.Basics.ViewModels
open System
open System.Collections.ObjectModel
open Mvvm
open Orimath.Plugins
open Orimath.Plugins.ThreadController

type PaperViewModel(paper: IPaperModel, pointConverter: ScreenPointConverter, invoker: IUIThreadInvoker) as this =
    inherit NotifyPropertyChanged()
    let disposables = ResizeArray<IDisposable>()
    let layers =
        new AttachedObservableCollection<_, _>(invoker, paper.Layers, paper.LayerChanged,
            (fun l -> new LayerViewModel(l, pointConverter, invoker)), (fun l -> l.Dispose()))
    do disposables.Add(layers)
    let mutable selectedLayers = Array.empty
    let mutable selectedEdges = Array.empty
    let mutable selectedLines = Array.empty
    let mutable selectedPoints = Array.empty

    do disposables.Add(paper.SelectedLayersChanged.Subscribe(fun _ -> onUI invoker this.SelectedLayersChanged))
    do disposables.Add(paper.SelectedEdgesChanged.Subscribe(fun _ -> onUI invoker this.SelectedEdgesChanged))
    do disposables.Add(paper.SelectedLinesChanged.Subscribe(fun _ -> onUI invoker this.SelectedLinesChanged))
    do disposables.Add(paper.SelectedPointsChanged.Subscribe(fun _ -> onUI invoker this.SelectedPointsChanged))

    member __.Source = paper
    member __.Layers = layers :> ObservableCollection<_>
    member __.SelectedLayers = selectedLayers
    member __.SelectedEdges = selectedEdges
    member __.SelectedLines = selectedLines
    member __.SelectedPoints = selectedPoints

    member private this.SelectedLayersChanged() =
        selectedLayers <- paper.SelectedLayers |> Array.choose(fun l -> layers |> Seq.tryFind(fun lm -> lm.Source = l))
        this.OnPropertyChanged("SelectedLayers")

    member private this.SelectedEdgesChanged() =
        selectedEdges <- paper.SelectedEdges |> Array.map(fun e -> EdgeViewModel(e, pointConverter))
        this.OnPropertyChanged("SelectedEdges")

    member private this.SelectedLinesChanged() =
        selectedLines <- paper.SelectedLines |> Array.map(fun l -> LineViewModel(l, pointConverter))
        this.OnPropertyChanged("SelectedLines")

    member private this.SelectedPointsChanged() =
        selectedPoints <- paper.SelectedPoints |> Array.map(fun p -> PointViewModel(p, pointConverter))
        this.OnPropertyChanged("SelectedPoints")

    member __.Dispose() =
        for d in disposables do d.Dispose()
        disposables.Clear()

    interface IDisposable with
        member this.Dispose() = this.Dispose()
