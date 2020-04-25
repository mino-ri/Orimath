module Orimath.Core.PluginLoader
open System
open System.IO
open System.Reflection
open Orimath.Plugins

let loadAssemblies() =
    let rootFilePath = Assembly.GetEntryAssembly().Location
    let pluginDirectory = Path.Combine(Path.GetDirectoryName(rootFilePath), "Plugins")
    if Directory.Exists(pluginDirectory) then
        Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly)
        |> Seq.map Assembly.LoadFrom
        |> Seq.toList
    else
        List.empty
    
let private getInstances<'T>(assembly: Assembly) =
    assembly.GetExportedTypes()
    |> Seq.filter(typeof<'T>.IsAssignableFrom)
    |> Seq.choose(fun t -> try Some(Activator.CreateInstance(t) :?> 'T) with _ -> None)

let getPluginTools(workspace) =
    loadAssemblies()
    |> Seq.collect getInstances<IToolProvidor>
    |> Seq.collect (fun providor -> providor.GetTools(workspace))
    |> Seq.toArray
