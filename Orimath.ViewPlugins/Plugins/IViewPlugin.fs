namespace Orimath.Plugins

type ViewPluginArgs =
    { Workspace: IWorkspace
      FileManager: IFileManager
      Messenger: IMessenger
      Dispatcher: IDispatcher
      PointConverter: IViewPointConverter }


type IViewPlugin =
    abstract member Execute : args: ViewPluginArgs -> unit
