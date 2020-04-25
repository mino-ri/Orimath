namespace Orimath.Plugins
open Orimath.Core

type PointConverter(scale: float, offsetX: float, offsetY: float) =
    member __.Scale = scale
    member __.OffsetX = offsetX
    member __.OffsetY = offsetY
    member __.ModelToScreen(point: Point) = 
        { X = point.X * scale + offsetX
          Y = point.Y * scale + offsetY }
    member __.ScreenToModel(point: Point) =
        { X = (point.X - offsetX) / scale
          Y = (point.Y - offsetY) / scale }
