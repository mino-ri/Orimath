namespace Orimath.Measure
open System.ComponentModel
open Orimath.Plugins

[<DisplayName("ツール: 計測"); Description("ドラッグ操作で角度と距離を測るツール。")>]
type MeasurePlugin() =
    interface IPlugin with
        member _.Execute(args) = args.Workspace.AddTool(MeasureTool(args.Workspace))
