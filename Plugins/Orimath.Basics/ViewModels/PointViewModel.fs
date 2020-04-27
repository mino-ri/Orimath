namespace Orimath.Basics.ViewModels
open Mvvm
open Orimath.Core
open Orimath.Plugins

type PointViewModel(point: Point, pointConverter: IViewPointConverter) =
    inherit NotifyPropertyChanged()
    let screenPoint = pointConverter.ModelToView(point)
    member __.Source = point
    member __.X = screenPoint.X
    member __.Y = screenPoint.Y

    override __.ToString() = point.ToString()

    interface IDisplayTargetViewModel with
        member __.GetTarget() = DisplayTarget.Point(point)
