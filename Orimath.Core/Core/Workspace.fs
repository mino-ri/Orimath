namespace Orimath.Core
open System.Collections.Generic
open Orimath.Plugins

type Workspace() as this =
    let paper = PaperModel()
    let currentTool = ReactiveProperty.createEq this (SelectorTool(this) :> ITool)
    let tools = ResizeArray<ITool>()
    let effects = ResizeArray<IEffect>()
    let mutable initialized = false

    member __.Paper = paper
    member val Tools = tools.AsReadOnly() :> IReadOnlyList<_>
    member val Effects = effects.AsReadOnly() :> IReadOnlyList<_>
    member __.CurrentTool
        with get() = currentTool.Value
        and set v =
            if currentTool.Value <> v then
                currentTool.Value.OnDeactivated()
                currentTool.Value <- v
                currentTool.Value.OnActivated()
    member __.AddEffect(effect) =
        if initialized then invalidOp "初期化後にエフェクトを追加することはできません。"
        effects.Add(effect)
    member __.AddTool(tool) =
        if initialized then invalidOp "初期化後にツールを追加することはできません。"
        tools.Add(tool)

    member __.Initialize() =
        if initialized then invalidOp "初期化は既に完了しています。"
        currentTool.Value <- tools.[0]
        currentTool.Value.OnActivated()
        paper.Clear()
        initialized <- true
    member __.CurrentToolChanged = currentTool.ValueChanged

    interface IWorkspace with
        member this.AddEffect(effect) = this.AddEffect(effect)
        member this.AddTool(tool) = this.AddTool(tool)
        member this.Paper = upcast this.Paper
        member this.Tools = this.Tools
        member this.Effects = this.Effects
        member this.CurrentTool with get() = this.CurrentTool and set(v) = this.CurrentTool <- v
        [<CLIEvent>] member this.CurrentToolChanged = this.CurrentToolChanged

        member this.Initialize() = this.Initialize()
        member __.CreatePaper(layers) = upcast Paper.Create(layers)
        member __.CreateLayer(edges, lines, points) = upcast Layer.Create(edges, lines, points)
        member __.CreateLayerFromSize(width, height) = upcast Layer.FromSize(width, height)
        member __.CreateLayerFromPolygon(vertexes) = upcast Layer.FromPolygon(vertexes)
