namespace Orimath.Plugins

[<NoComparison; ReferenceEquality>]
type PluginArgs =
    {
        Workspace: IWorkspace
    }

type IPlugin =
    abstract member Execute : args: PluginArgs -> unit
