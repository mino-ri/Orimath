namespace Orimath.Basics.ViewModels
open Orimath.Plugins

type IDisplayTargetViewModel =
    abstract member GetTarget : unit -> DisplayTarget
