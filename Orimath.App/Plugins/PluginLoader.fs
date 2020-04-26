module internal Orimath.Plugins.PluginLoader
open System
open System.IO
open System.Reflection
open System.Windows
open Orimath.Plugins

let loadPluginTypes() =
    let rootFilePath = Assembly.GetEntryAssembly().Location
    let pluginDirectory = Path.Combine(Path.GetDirectoryName(rootFilePath), "Plugins")
    if Directory.Exists(pluginDirectory) then
        Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly)
        |> Seq.map Assembly.LoadFrom
        |> Seq.collect(fun asm -> asm.GetExportedTypes())
        |> Seq.toArray
    else
        Array.empty
    
let private getInstances<'T>(types: Type[]) =
    types
    |> Seq.filter(fun t -> t.IsClass && not t.IsAbstract && typeof<'T>.IsAssignableFrom(t))
    |> Seq.choose(fun t -> try Some(Activator.CreateInstance(t) :?> 'T) with _ -> None)

let executePlugins(viewArgs: ViewPluginArgs) =
    let args = { Workspace = viewArgs.Workspace }
    let types = loadPluginTypes()
    for plugin in getInstances<IPlugin> types do plugin.Execute(args)
    for viewPlugin in getInstances<IViewPlugin> types do viewPlugin.Execute(viewArgs)

    types
    |> Seq.filter(fun t -> not t.IsAbstract && typeof<FrameworkElement>.IsAssignableFrom(t))
    |> Seq.map(fun t -> t, t.GetCustomAttribute<ViewAttribute>())
    |> Seq.filter(fun (_, att) -> not (isNull (box att)))
