namespace Orimath.ViewModels
open Orimath.Core
open Orimath.Plugins

type PointViewModel(point: Point, pointConverter: PointConverter) =
    inherit NotifyPropertyChanged()
    let screenPoint = pointConverter.ModelToScreen(point)
    member __.Source = point
    member __.X = screenPoint.X
    member __.Y = screenPoint.Y

    override __.ToString() = point.ToString()
