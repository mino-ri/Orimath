namespace Orimath.Folds
open System.ComponentModel
open Orimath.Folds.Core
open Orimath.Plugins

[<DisplayName("ツール: 折り線"); Description("ドラッグ操作で折線をつけるツール。")>]
type FoldsPlugin() =
    interface IPlugin with
        member _.Execute(args) = args.Workspace.AddTool(DragFoldTool(args.Workspace))
