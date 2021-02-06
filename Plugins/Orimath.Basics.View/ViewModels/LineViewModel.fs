namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Core
open Orimath.Plugins
open Orimath.Controls
open Orimath.Basics.View
open Orimath.Core.NearlyEquatable

type LineViewModel(seg: LineSegment, pointConverter: IViewPointConverter, ?layer: ILayerModel) =
    inherit NotifyPropertyChanged()
    let (ScreenPoint(x1, y1)) = pointConverter.ModelToView(seg.Point1)
    let (ScreenPoint(x2, y2)) = pointConverter.ModelToView(seg.Point2)
    member _.Source = seg
    member _.X1 = x1
    member _.Y1 = y1
    member _.X2 = x2
    member _.Y2 = y2
    member _.XFactor = seg.Line.XFactor
    member _.YFactor = !-seg.Line.YFactor
    member _.Intercept = seg.Line.Intercept
    member _.Slope = !+(seg.Line.XFactor / seg.Line.YFactor)
    member _.Angle = !+(atan2 seg.Line.XFactor seg.Line.YFactor / Math.PI * 180.0) % 180.0
    member _.Length = seg.Length
   
    override _.ToString() = seg.ToString()

    interface IDisplayTargetViewModel with
        member _.GetTarget() =
            match layer with
            | Some(l) -> l, DisplayTarget.Line(seg)
            | None -> invalidOp "Layer is not set"
