namespace Orimath.Plugins

type PluginArgs =
    {
        Workspace: IWorkspace
    }

type IPlugin =
    abstract member Execute : args: PluginArgs -> unit
