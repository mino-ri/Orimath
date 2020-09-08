namespace Orimath.Folds
open Orimath.Plugins
open Orimath.Folds.Core

[<OrimathPlugin("折線ツール", "ドラッグ操作で折線をつけるツール。")>]
type FoldsPlugin() =
    interface IPlugin with
        member __.Execute(args) = args.Workspace.AddTool(DragFoldTool(args.Workspace))
