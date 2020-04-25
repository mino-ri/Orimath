namespace Orimath.Core
open System.Collections.Generic
open Orimath.Plugins

type Workspace() as this =
    let paper = PaperModel()
    let currentTool = ReactiveProperty.createEq this (SelectorTool(this) :> ITool)
    let mutable tools = Array.empty<ITool>
    let mutable effects = Array.empty<IEffect>

    member __.Paper = paper
    member __.Tools = tools :> IReadOnlyList<ITool>
    member __.Effects = effects :> IReadOnlyList<IEffect>
    member __.CurrentTool with get() = currentTool.Value and set v = currentTool.Value <- v

    member this.Initialize() =
        tools <- PluginLoader.getPluginTools(this)
        paper.Clear()
    member __.CurrentToolChanged = currentTool.ValueChanged

    interface IWorkspace with
        member this.Paper = upcast this.Paper
        member this.Tools = this.Tools
        member this.Effects = this.Effects
        member this.CurrentTool with get() = this.CurrentTool and set(v) = this.CurrentTool <- v
        [<CLIEvent>] member this.CurrentToolChanged = this.CurrentToolChanged

        member this.Initialize() = this.Initialize()
        member __.CreatePaper(layers) = upcast (layers |> Seq.map Layer.AsLayer |> Paper.Create)
        member __.CreateLayer(edges, lines, points) = upcast Layer.Create(edges, lines, points)
        member __.CreateLayerFromSize(width, height) = upcast Layer.FromSize(width, height)
        member __.CreateLayerFromPolygon(vertexes) = upcast Layer.FromPolygon(vertexes)
