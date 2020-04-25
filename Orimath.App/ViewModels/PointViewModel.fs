namespace Orimath.ViewModels
open Orimath.Core
open Orimath.Converters

type PointViewModel(point: Point, pointConverter: ScreenPointConverter) =
    inherit NotifyPropertyChanged()
    let screenPoint = pointConverter.Convert(point)
    member __.Source = point
    member __.X = screenPoint.X
    member __.Y = screenPoint.Y
