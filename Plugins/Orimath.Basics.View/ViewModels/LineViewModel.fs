namespace Orimath.Basics.View.ViewModels
open System
open Orimath.Core
open Orimath.Plugins
open Orimath.Controls
open Orimath.Basics.View
open Orimath.Core.NearlyEquatable
open Orimath.Combination

type LineViewModel(crease: Crease, pointConverter: IViewPointConverter, ?layer: ILayerModel) =
    inherit NotifyPropertyChanged()
    let (ScreenPoint(x1, y1)) = pointConverter.ModelToView(crease.Point1)
    let (ScreenPoint(x2, y2)) = pointConverter.ModelToView(crease.Point2)
    member _.Source = crease
    member _.X1 = x1
    member _.Y1 = y1
    member _.X2 = x2
    member _.Y2 = y2
    member _.XFactor = crease.Line.XFactor
    member _.YFactor = !-crease.Line.YFactor
    member _.Intercept = crease.Line.Intercept
    member _.Slope = !+(crease.Line.XFactor / crease.Line.YFactor)
    member _.Angle = !+(atan2 crease.Line.XFactor crease.Line.YFactor / Math.PI * 180.0) % 180.0
    member _.Length = crease.Length
    member _.Color =
        match crease.Type with
        | CreaseType.Draft -> InstructionColor.LightGray
        | _ -> InstructionColor.Black
   
    override _.ToString() = crease.ToString()

    interface IDisplayTargetViewModel with
        member _.GetTarget() =
            match layer with
            | Some(l) -> l, DisplayTarget.Crease(crease)
            | None -> invalidOp "Layer is not set"
