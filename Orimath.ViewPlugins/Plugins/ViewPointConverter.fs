namespace Orimath.Plugins
open Orimath.Core

type ScreenPoint = System.Windows.Point

type IViewPointConverter =
    abstract member ModelToView : point: Point -> ScreenPoint
    abstract member ViewToModel : point: ScreenPoint -> Point
