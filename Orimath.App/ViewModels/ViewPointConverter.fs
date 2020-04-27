namespace Orimath.ViewModels
open Orimath.Core
open Orimath.Plugins

type ViewPointConverter(scale: float, offsetX: float, offsetY: float) =
    member __.ModelToView(point: Point) =
        ScreenPoint(point.X * scale + offsetX, point.Y * scale + offsetY)

    member __.ViewToModel(point: ScreenPoint) =
        { X = (point.X - offsetX) / scale; Y = (point.Y - offsetY) / scale }

    interface IViewPointConverter with
        member this.ModelToView(point) = this.ModelToView(point)
        member this.ViewToModel(point) = this.ViewToModel(point)
