namespace Orimath.Basics
open Orimath.Plugins

type PluginImplementations() =
    interface IPlugin with
        member __.Execute(args) =
            args.Workspace.AddEffect(UndoEffect(args.Workspace))
            args.Workspace.AddEffect(RedoEffect(args.Workspace))
