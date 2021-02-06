namespace Orimath.Basics.View.ViewModels
open System
open System.Collections.ObjectModel
open System.Windows.Media
open Orimath.Controls
open Orimath.Core
open Orimath.Plugins

type LayerViewModel(layer: ILayerModel, pointConverter: IViewPointConverter, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let points = 
        new AttachedObservableCollection<Point, PointViewModel>
            (dispatcher,
             layer.Points,
             (fun p -> PointViewModel(p, pointConverter, layer)),
             ignore)
    let lines =
        new AttachedObservableCollection<LineSegment, LineViewModel>
            (dispatcher,
             layer.Lines,
             (fun l -> new LineViewModel(l, pointConverter, layer)),
             ignore)

    member _.Source = layer
    member val Edges = [| for e in layer.Edges -> new EdgeViewModel(e, pointConverter, layer) |]
    member val Vertexes = PointCollection(seq { for e in layer.Edges -> pointConverter.ModelToView(e.Line.Point1) })
    member _.Lines = lines :> ObservableCollection<_>
    member _.Points = points :> ObservableCollection<_>
    member _.LayerType = layer.LayerType

    member _.Dispose() =
        points.Dispose()
        lines.Dispose()

    interface IDisplayTargetViewModel with
        member _.GetTarget() = layer, DisplayTarget.Layer(layer)

    interface IDisposable with
        member this.Dispose() = this.Dispose()
