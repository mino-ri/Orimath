module Orimath.Plugins.PluginExecutor
open System
open System.Collections.Generic
open System.IO
open System.Reflection
open Orimath
open Orimath.IO
open SsslFSharp
open Orimath.Internal

let mutable loadedPluginTypes = Type.EmptyTypes

let mutable loadedViewPluginTypes = Type.EmptyTypes

let mutable setting = PluginSetting.CreateDefault()

let mutable configurablePlugins = List<IConfigurablePlugin>()

let private loadPluginTypes() =
    let pluginDirectory =
        Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Plugins")
    if Directory.Exists(pluginDirectory) then
        [|
            for file in Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly) do
            for t in Assembly.LoadFrom(file).GetExportedTypes() do
            if t.IsClass && not t.IsAbstract then t
        |]
    else
        Type.EmptyTypes

let saveSetting() = Settings.save Settings.Plugin setting

[<RequiresExplicitTypeArguments>]
let getFullNames<'T>(types: Type[]) = [|
        for t in types do
        if typeof<'T>.IsAssignableFrom(t) then t.FullName
    |]

let private loadSetting (types: Type[]) =
    match Settings.load Settings.Plugin with
    | Some(s) when isNotNull (box s) ->
        setting <- s
        if isNull setting.PluginOrder then setting.PluginOrder <- Array.Empty()
        if isNull setting.Settings then setting.Settings <- Dictionary()
    | _ ->
        setting <- PluginSetting.CreateDefault()
        setting.PluginOrder <-
            Array.append (getFullNames<IPlugin> types) (getFullNames<IViewPlugin> types)
        saveSetting()
    setting

let private setSetting (plugin: obj) fullName (setting: PluginSetting) =
    match plugin with
    | :? IConfigurablePlugin as configurable ->
        configurablePlugins.Add(configurable)
        setting.Settings.TryGetValue(fullName)
        |> BoolOption.toOption
        |> Option.bind (Sssl.tryConvertToObj configurable.SettingType)
        |> Option.filter (isNull >> not)
        |> function
        | Some(targetSetting) -> configurable.Setting <- targetSetting
        | None ->
            if isNull configurable.Setting then
                configurable.Setting <- createInstance configurable.SettingType
            setting.Settings[fullName] <-
                Sssl.convertFromObj configurable.SettingType configurable.Setting
    | _ -> ()

let private executeCore (setting: PluginSetting) args viewArgs =
    let pluginTypes = Map.ofSeq (seq { for t in loadedPluginTypes -> t.FullName, t })
    let viewPluginTypes = Map.ofSeq (seq { for t in loadedViewPluginTypes -> t.FullName, t })
    for fullName in setting.PluginOrder do
        Map.tryFind fullName pluginTypes
        |> Option.bind createInstanceAs<IPlugin>
        |> Option.iter (fun plugin ->
            setSetting plugin fullName setting
            plugin.Execute(args))
        Map.tryFind fullName viewPluginTypes
        |> Option.bind createInstanceAs<IViewPlugin>
        |> Option.iter (fun viewPlugin ->
            setSetting viewPlugin fullName setting
            viewPlugin.Execute(viewArgs))

let ExecutePlugins (viewArgs: ViewPluginArgs) =
    let args = { Workspace = viewArgs.Workspace; FileManager = viewArgs.FileManager }
    let types = loadPluginTypes()
    let setting = loadSetting types
    loadedPluginTypes <- types |> Array.filter typeof<IPlugin>.IsAssignableFrom
    loadedViewPluginTypes <- types |> Array.filter typeof<IViewPlugin>.IsAssignableFrom
    executeCore setting args viewArgs
