namespace Orimath.Core
open System.Collections.Generic
open Orimath.Plugins

type Workspace() as this =
    let paper = PaperModel()
    let currentTool = ReactiveProperty.createEq this (SelectorTool(this) :> ITool)
    let tools = ResizeArray<ITool>()
    let effects = ResizeArray<IEffect>()
    let mutable initialized = false

    member _.Paper = paper
    member val Tools = tools.AsReadOnly() :> IReadOnlyList<_>
    member val Effects = effects.AsReadOnly() :> IReadOnlyList<_>
    member _.CurrentTool
        with get() = currentTool.Value
        and set v =
            if currentTool.Value <> v then
                currentTool.Value.OnDeactivated()
                currentTool.Value <- v
                currentTool.Value.OnActivated()
    member _.AddEffect(effect) =
        if initialized then invalidOp "初期化後にエフェクトを追加することはできません。"
        effects.Add(effect)
    member _.AddTool(tool) =
        if initialized then invalidOp "初期化後にツールを追加することはできません。"
        tools.Add(tool)

    member _.Initialize() =
        if initialized then invalidOp "初期化は既に完了しています。"
        currentTool.Value <- tools.[0]
        currentTool.Value.OnActivated()
        paper.Clear()
        initialized <- true
    member _.CurrentToolChanged = currentTool.ValueChanged

    interface IWorkspace with
        member this.AddEffect(effect) = this.AddEffect(effect)
        member this.AddTool(tool) = this.AddTool(tool)
        member this.Paper = upcast this.Paper
        member this.Tools = this.Tools
        member this.Effects = this.Effects
        member this.CurrentTool with get() = this.CurrentTool and set(v) = this.CurrentTool <- v
        [<CLIEvent>] member this.CurrentToolChanged = this.CurrentToolChanged

        member this.Initialize() = this.Initialize()
        member _.CreatePaper(layers) = upcast Paper.Create(layers)
        member _.CreateLayer(edges, lines, points, layerType, originalEdges, matrix) = upcast Layer.Create(edges, lines, points, layerType, originalEdges, matrix)
        member _.CreateLayerFromSize(width, height, layerType) = upcast Layer.FromSize(width, height, layerType)
        member _.CreateLayerFromPolygon(vertexes, layerType) = upcast Layer.FromPolygon(vertexes, layerType)
