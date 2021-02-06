namespace Orimath.Basics.View.ViewModels
open Orimath.Core
open Orimath.Plugins
open Orimath.Controls
open Orimath.Basics.View

type PointViewModel(point: Point, pointConverter: IViewPointConverter, ?layer: ILayerModel) =
    inherit NotifyPropertyChanged()
    let (ScreenPoint(x, y)) = pointConverter.ModelToView(point)
    member _.Source = point
    member _.X = x
    member _.Y = y
    
    override _.ToString() = point.ToString()

    interface IDisplayTargetViewModel with
        member _.GetTarget() =
            match layer with
            | Some(l) -> l, DisplayTarget.Point(point)
            | None -> invalidOp "Layer is not set"
