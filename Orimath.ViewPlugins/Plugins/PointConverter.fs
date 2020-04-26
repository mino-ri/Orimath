namespace Orimath.Plugins
open Orimath.Core

type ScreenPoint = System.Windows.Point

type ScreenPointConverter(scale: float, offsetX: float, offsetY: float) =
    member __.Scale = scale
    member __.OffsetX = offsetX
    member __.OffsetY = offsetY
    member __.ModelToScreen(point: Point) =
        ScreenPoint(point.X * scale + offsetX, point.Y * scale + offsetY)
    member __.ScreenToModel(point: ScreenPoint) =
        { X = (point.X - offsetX) / scale
          Y = (point.Y - offsetY) / scale }
