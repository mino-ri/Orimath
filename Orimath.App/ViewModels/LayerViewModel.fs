namespace Orimath.ViewModels
open System
open System.Collections.ObjectModel
open System.Windows.Media
open Orimath.Plugins
open Orimath.Converters

type LayerViewModel(layer: ILayerModel, pointConverter: ScreenPointConverter, invoker: IUIThreadInvoker) =
    inherit NotifyPropertyChanged()
    let points = new AttachedObservableCollection<_, _>(invoker, layer.Points, layer.PointChanged, (fun p -> PointViewModel(p, pointConverter)), ignore)
    let lines = new AttachedObservableCollection<_, _>(invoker, layer.Lines, layer.LineChanged, (fun l -> LineViewModel(l, pointConverter)), ignore)

    member __.Source = layer
    member val Edges = layer.Edges |> Seq.map(fun e -> EdgeViewModel(e, pointConverter))
    member val Vertexes =
        layer.Edges
        |> Seq.map(fun e -> pointConverter.Convert(e.Line.Point1))
        |> PointCollection
    member __.Points = points :> ObservableCollection<_>
    member __.Lines = lines :> ObservableCollection<_>

    member __.Dispose() = points.Dispose(); lines.Dispose()

    interface IDisposable with
        member this.Dispose() = this.Dispose()
