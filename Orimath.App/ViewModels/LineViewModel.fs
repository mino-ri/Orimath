namespace Orimath.ViewModels
open Orimath.Core
open Orimath.Converters

type LineViewModel(line: LineSegment, pointConverter: ScreenPointConverter) =
    inherit NotifyPropertyChanged()
    let screenPoint1 = pointConverter.Convert(line.Point1)
    let screenPoint2 = pointConverter.Convert(line.Point2)
    
    member __.Source = line
    member __.X1 = screenPoint1.X
    member __.Y1 = screenPoint1.Y
    member __.X2 = screenPoint2.X
    member __.Y2 = screenPoint2.Y
