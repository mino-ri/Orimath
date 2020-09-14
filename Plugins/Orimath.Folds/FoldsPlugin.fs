namespace Orimath.Folds
open System.ComponentModel
open Orimath.Plugins
open Orimath.Folds.Core

[<DisplayName("ツール: 折り線"); Description("ドラッグ操作で折線をつけるツール。")>]
type FoldsPlugin() =
    interface IPlugin with
        member __.Execute(args) = args.Workspace.AddTool(DragFoldTool(args.Workspace))
