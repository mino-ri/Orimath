namespace Orimath.Converters
open System
open System.Windows.Data
open Orimath.Core

[<ValueConversion(typeof<Point>, typeof<Windows.Point>)>]
type ScreenPointConverter() =
    member val Scale = 512.0 with get, set
    member val OffsetX = 0.0 with get, set
    member val OffsetY = 0.0 with get, set

    member this.Convert(point: Point) = 
        Windows.Point(point.X * this.Scale + this.OffsetX, point.Y * this.Scale + this.OffsetY)
    
    member this.ConvertBack(point: Windows.Point) =
        { X = (point.X - this.OffsetX) / this.Scale
          Y = (point.Y - this.OffsetY) / this.Scale }

    interface IValueConverter with
        member this.Convert(value, _, _, _) = box (this.Convert(unbox value))
        member this.ConvertBack(value, _, _, _) = box (this.ConvertBack(unbox value))
