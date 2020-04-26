namespace Orimath.Basics.ViewModels
open System
open Mvvm
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.Plugins

type EdgeViewModel(edge: Edge, pointConverter: ScreenPointConverter) =
    inherit NotifyPropertyChanged()
    let screenPoint1 = pointConverter.ModelToScreen(edge.Line.Point1)
    let screenPoint2 = pointConverter.ModelToScreen(edge.Line.Point2)
    
    member __.Source = edge
    member __.X1 = screenPoint1.X
    member __.Y1 = screenPoint1.Y
    member __.X2 = screenPoint2.X
    member __.Y2 = screenPoint2.Y
    member __.Slope = !+(edge.Line.Line.XFactor / !-edge.Line.Line.YFactor)
    member __.Angle = !+(atan2 edge.Line.Line.XFactor !-edge.Line.Line.YFactor / Math.PI * 180.0) % 180.0

     override this.ToString() =
         edge.Line.Line.ToString() +
         String.Format("\r\n傾き:{0:0.#####} 角度:{1:0.#####}°", this.Slope, this.Angle)

    interface IDisplayTargetViewModel with
        member __.GetTarget() = DisplayTarget.Edge(edge)
