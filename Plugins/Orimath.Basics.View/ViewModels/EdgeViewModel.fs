namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Core
open Orimath.Plugins
open Orimath.Controls
open Orimath.Basics.View
open Orimath.Core.NearlyEquatable

type EdgeViewModel(edge: Edge, pointConverter: IViewPointConverter, ?layer: ILayerModel) =
    inherit NotifyPropertyChanged()
    let (ScreenPoint(x1, y1)) = pointConverter.ModelToView(edge.Point1)
    let (ScreenPoint(x2, y2)) = pointConverter.ModelToView(edge.Point2)
    member _.Source = edge
    member _.X1 = x1
    member _.Y1 = y1
    member _.X2 = x2
    member _.Y2 = y2
    member _.XFactor = edge.Line.XFactor
    member _.YFactor = !-edge.Line.YFactor
    member _.Intercept = edge.Line.Intercept
    member _.Slope = !+(edge.Line.XFactor / -edge.Line.YFactor)
    member _.Angle = !+(atan2 edge.Line.XFactor -edge.Line.YFactor / Math.PI * 180.0) % 180.0
    member _.Length = edge.Length
   
    override _.ToString() = edge.ToString()

    interface IDisplayTargetViewModel with
        member _.GetTarget() =
            match layer with
            | Some(l) -> l, DisplayTarget.Edge(edge)
            | None -> invalidOp "Layer is not set"
