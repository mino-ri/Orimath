namespace Orimath.Folds
open System.ComponentModel
open Orimath.Plugins
open Orimath.Folds.Core

[<DisplayName("折線ツール"); Description("ドラッグ操作で折線をつけるツール。")>]
type FoldsPlugin() =
    interface IPlugin with
        member __.Execute(args) = args.Workspace.AddTool(DragFoldTool(args.Workspace))
