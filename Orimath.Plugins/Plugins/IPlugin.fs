namespace Orimath.Plugins
open System

[<NoComparison; ReferenceEquality>]
type PluginArgs =
    { Workspace: IWorkspace
      FileManager: IFileManager }


type IPlugin =
    abstract member Execute : args: PluginArgs -> unit


type IConfigurablePlugin =
    abstract member SettingType : Type
    abstract member Setting : obj with get, set


[<AbstractClass>]
type ConfigurablePluginBase<'T>(init: 'T) =
    member val Setting = init with get, set

    interface IConfigurablePlugin with
        member _.SettingType = typeof<'T>
        member this.Setting
            with get() = box this.Setting
            and set v = this.Setting <- v :?> 'T
