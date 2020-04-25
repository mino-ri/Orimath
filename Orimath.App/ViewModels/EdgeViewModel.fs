namespace Orimath.ViewModels
open Orimath.Core
open Orimath.Converters

type EdgeViewModel(edge: Edge, pointConverter: ScreenPointConverter) =
    inherit NotifyPropertyChanged()
    let screenPoint1 = pointConverter.Convert(edge.Line.Point1)
    let screenPoint2 = pointConverter.Convert(edge.Line.Point2)
    
    member __.Source = edge
    member __.X1 = screenPoint1.X
    member __.Y1 = screenPoint1.Y
    member __.X2 = screenPoint2.X
    member __.Y2 = screenPoint2.Y
