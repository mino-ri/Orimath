namespace Orimath.ViewModels
open Orimath.Plugins

type IDisplayTargetViewModel =
    abstract member GetTarget : unit -> DisplayTarget
