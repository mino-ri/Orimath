namespace Orimath.Core
open System.Collections.Generic
open Orimath.Plugins
open ApplicativeProperty

type Workspace() as this =
    let paper = PaperModel()
    let currentTool = Prop.value (SelectorTool(this) :> ITool)
    let tools = ResizeArray<ITool>()
    let effects = ResizeArray<IEffect>()
    let mutable initialized = false
    do currentTool
        |> Observable.pairwise
        |> Observable.add (fun (newValue, oldValue) ->
            oldValue.OnDeactivated()
            newValue.OnActivated())

    member _.Paper = paper
    member val Tools = tools.AsReadOnly() :> IReadOnlyList<_>
    member val Effects = effects.AsReadOnly() :> IReadOnlyList<_>
    member _.CurrentTool = currentTool
    member _.AddEffect(effect) =
        if initialized then invalidOp "初期化後にエフェクトを追加することはできません。"
        effects.Add(effect)
    member _.AddTool(tool) =
        if initialized then invalidOp "初期化後にツールを追加することはできません。"
        tools.Add(tool)

    member _.Initialize() =
        if initialized then invalidOp "初期化は既に完了しています。"
        if tools.Count > 0 then
            currentTool.Value <- tools.[0]
            currentTool.Value.OnActivated()
        paper.Clear()
        initialized <- true

    interface IWorkspace with
        member this.AddEffect(effect) = this.AddEffect(effect)
        member this.AddTool(tool) = this.AddTool(tool)
        member this.Paper = upcast this.Paper
        member this.Tools = this.Tools
        member this.Effects = this.Effects
        member this.CurrentTool = upcast this.CurrentTool
        member this.Initialize() = this.Initialize()
