namespace Orimath.Plugins
open System

[<NoComparison; ReferenceEquality>]
type PluginArgs =
    { Workspace: IWorkspace }

type IPlugin =
    abstract member Execute : args: PluginArgs -> unit

type IConfigurablePlugin =
    abstract member SettingType : Type
    abstract member Setting : obj with get, set
