namespace Orimath.Plugins

type ViewPluginArgs =
    {
        Workspace: IWorkspace
        Messenger: IMessenger
        UIThreadInvoker: IUIThreadInvoker
        PointConverter: ScreenPointConverter
    }

type IViewPlugin =
    abstract member Execute : args: ViewPluginArgs -> unit
