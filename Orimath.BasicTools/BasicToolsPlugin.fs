namespace Orimath.BasicTools
open Orimath.Plugins

type BasicToolsPlugin() =
    interface IPlugin with
        member __.Execute(args) = args.Workspace.AddTool(DragFoldingTool(args.Workspace))
