namespace Orimath.Basics.View.ViewModels
open Orimath.Controls
open Orimath.Plugins
open ApplicativeProperty

type MeasureViewModel(paper: IPaperModel, pointConverter: IViewPointConverter, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let mapArray mapping prop =
        prop |> Prop.map (Array.map mapping) |> Prop.fetch dispatcher.SyncContext

    member val SelectedPoints =
        paper.SelectedPoints |> mapArray (fun p -> PointViewModel(p, pointConverter))

    member val SelectedLines =
        paper.SelectedCreases |> mapArray (fun c -> LineViewModel(c, pointConverter))

    member val SelectedEdges =
        paper.SelectedEdges |> mapArray (fun e -> EdgeViewModel(e, pointConverter))
