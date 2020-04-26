namespace Orimath.Folds
open Orimath.Plugins
open Orimath.Folds.Core

type FoldsPlugin() =
    interface IPlugin with
        member __.Execute(args) = args.Workspace.AddTool(DragFoldTool(args.Workspace))
