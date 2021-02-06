namespace Orimath.ViewModels
open System
open System.Collections.ObjectModel
open System.ComponentModel
open System.Linq
open System.Reflection
open Orimath
open Orimath.Controls
open Orimath.Plugins
open Orimath.Internal
open ApplicativeProperty
open Sssl

[<AbstractClass>]
type PluginSettingPageViewModel() =
    inherit NotifyPropertyChanged()

    abstract member Header : string


type PluginViewModel(pluginType: Type, isEnabled: bool) =
    inherit NotifyPropertyChanged()

    member val IsEnabled = Prop.value isEnabled
    member _.FullName = pluginType.FullName
    member _.Type = pluginType

    member val Name =
        pluginType.GetCustomAttribute<DisplayNameAttribute>()
        |> Null.bind(fun att -> att.DisplayName)
        |> Null.defaultValue pluginType.Name

    member val Description =
        pluginType.GetCustomAttribute<DescriptionAttribute>()
        |> Null.bind(fun att -> att.Description)
        |> Null.defaultValue "(No description)"



type PluginItemSettingViewModel(plugin: IConfigurablePlugin, dispatcher: IDispatcher) =
    inherit PluginSettingPageViewModel()
    let pluginType = plugin.GetType()
    let header =
        pluginType.GetCustomAttribute<DisplayNameAttribute>()
        |> Null.bind(fun att -> att.DisplayName)
        |> Null.defaultValue pluginType.Name

    override _.Header = header
    member val Content = SettingViewModel(plugin.Setting, dispatcher)


type PluginLoadSettingViewModel(messenger: IMessenger) =
    inherit PluginSettingPageViewModel()
    let plugins = ObservableCollection<PluginViewModel>()
    let pluginIndex = Prop.value 0
    do
        let pluginMap =
            (Seq.append PluginExecutor.loadedPluginTypes PluginExecutor.loadedViewPluginTypes)
             .ToDictionary((fun t -> t.FullName), (fun t -> PluginViewModel(t, false)))
        let order = PluginExecutor.setting.PluginOrder
        for fullName in order do
            match pluginMap.TryGetValue(fullName) with
            | BoolSome(plugin) ->
                plugin.IsEnabled.Value <- true
                ignore (pluginMap.Remove(fullName))
                plugins.Add(plugin)
            | BoolNone -> ()
        for plugin in pluginMap.Values do
            plugins.Add(plugin)

    member _.Plugins = plugins
    member _.PluginIndex = pluginIndex
    
    member val UpPluginCommand =
        pluginIndex
        |> Prop.map(fun i -> 1 <= i && i < plugins.Count)
        |> Prop.command(fun _ -> plugins.Move(pluginIndex.Value, pluginIndex.Value - 1))
    
    member val DownPluginCommand =
        pluginIndex
        |> Prop.map(fun i -> 0 <= i && i < plugins.Count - 1)
        |> Prop.command(fun _ -> plugins.Move(pluginIndex.Value, pluginIndex.Value + 1))

    override _.Header = "ON/OFFと読み込み順"

    member _.Save(_: obj) =
        PluginExecutor.setting.PluginOrder <-
            [| for x in plugins do if x.IsEnabled.Value then yield x.FullName |]
        for configurable in PluginExecutor.configurablePlugins do
            PluginExecutor.setting.Settings.[configurable.GetType().ToString()] <-
                SsslObject.ConvertFrom(configurable.Setting)
        PluginExecutor.saveSetting()
        messenger.CloseDialog()


type PluginSettingViewModel(messenger: IMessenger, dispatcher: IDispatcher) =
    inherit NotifyPropertyChanged()
    let loadSetting = PluginLoadSettingViewModel(messenger)

    member val Pages =
        [| yield loadSetting :> PluginSettingPageViewModel
           for c in PluginExecutor.configurablePlugins do
               yield upcast PluginItemSettingViewModel(c, dispatcher) |]

    member val SaveCommand = Prop.ctrue |> Prop.command loadSetting.Save

    member _.CloseCommand = messenger.CloseDialogCommand