namespace Orimath.Basics.View.ViewModels
open Orimath.Plugins

type ScreenOperationTarget(point: System.Windows.Point, tuple: ILayerModel * DisplayTarget) =
    member _.Point = point
    member _.Layer = fst tuple
    member _.Target = snd tuple


type IDisplayTargetViewModel =
    abstract member GetTarget : unit -> ILayerModel * DisplayTarget
