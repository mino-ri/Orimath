namespace Orimath.Basics.View.ViewModels
open System.Collections.ObjectModel
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty

type PaperViewModel(paper: IPaperModel, pointConverter: IViewPointConverter, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let layers =
        new AttachedObservableCollection<ILayerModel, LayerViewModel>(
            dispatcher,
            paper.Layers,
            (fun l -> new LayerViewModel(l, pointConverter, dispatcher)),
            (fun l -> l.Dispose()))
    let mapArray mapping prop = prop |> Prop.map (Array.map mapping) |> Prop.fetch dispatcher.SyncContext

    member _.Layers = layers :> ObservableCollection<_>
    
    member val SelectedPoints =
        paper.SelectedPoints |> mapArray (fun p -> PointViewModel(p, pointConverter))
    
    member val SelectedLines =
        paper.SelectedLines |> mapArray (fun l -> LineViewModel(l, pointConverter))
    
    member val SelectedEdges =
        paper.SelectedEdges |> mapArray (fun e -> EdgeViewModel(e, pointConverter))
    
    member val SelectedLayers =
        paper.SelectedLayers |> mapArray (fun l -> new LayerViewModel(l, pointConverter, dispatcher))
