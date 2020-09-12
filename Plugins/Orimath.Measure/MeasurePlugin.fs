namespace Orimath.Measure
open System.ComponentModel
open Orimath.Plugins

[<DisplayName("計測ツール"); Description("ドラッグ操作で角度と距離を測るツール。")>]
type MeasurePlugin() =
    interface IPlugin with
        member __.Execute(args) = args.Workspace.AddTool(MeasureTool(args.Workspace))
