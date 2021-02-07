namespace Orimath.Plugins
open Orimath.Core

type ScreenPoint = System.Windows.Point


type IViewPointConverter =
    abstract member ModelToView : point: Point -> ScreenPoint
    abstract member ViewToModel : point: ScreenPoint -> Point


type ViewPointConverter(scaleX: float, scaleY: float, offsetX: float, offsetY: float) =
    member _.ModelToView(point: Point) =
        ScreenPoint(point.X * scaleX + offsetX, point.Y * scaleY + offsetY)
    member _.ViewToModel(point: ScreenPoint) =
        { X = (point.X - offsetX) / scaleX; Y = (point.Y - offsetY) / scaleY }

    interface IViewPointConverter with
        member this.ModelToView(point) = this.ModelToView(point)
        member this.ViewToModel(point) = this.ViewToModel(point)
