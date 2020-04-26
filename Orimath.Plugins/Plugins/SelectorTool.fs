namespace Orimath.Plugins
open System.IO

type SelectorTool(workspace: IWorkspace) =
    abstract member Name : string
    abstract member ShortcutKey : string
    abstract member Icon : Stream
    abstract member OnClick : target: OperationTarget * modifier: OperationModifier -> unit
    abstract member BeginDrag : source: OperationTarget * modifier: OperationModifier -> bool
    abstract member DragEnter : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> bool
    abstract member DragLeave : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> bool
    abstract member DragOver : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> bool
    abstract member Drop : source: OperationTarget * target: OperationTarget * modifier: OperationModifier -> unit

    default __.Name = "選択"
    default __.ShortcutKey = ""
    default __.Icon = null
    default __.BeginDrag(_, _) = false
    default __.DragEnter(_, _, _) = false
    default __.DragLeave(_, _, _) = false
    default __.DragOver(_, _, _) = false
    default __.Drop(_, _, _) = ()
    default __.OnClick(target, modifier) =
        if modifier = OperationModifier.None then
            match target.Target with
            |DisplayTarget.None ->
                workspace.Paper.SelectedLayers <- Array.empty
                workspace.Paper.SelectedEdges <- Array.empty
                workspace.Paper.SelectedLines <- Array.empty
                workspace.Paper.SelectedPoints <- Array.empty
            |DisplayTarget.Layer(l) ->
                workspace.Paper.SelectedLayers <- [| l |]
                workspace.Paper.SelectedEdges <- Array.empty
                workspace.Paper.SelectedLines <- Array.empty
                workspace.Paper.SelectedPoints <- Array.empty
            |DisplayTarget.Edge(e) ->
                workspace.Paper.SelectedLayers <- Array.empty
                workspace.Paper.SelectedEdges <- [| e |]
                workspace.Paper.SelectedLines <- Array.empty
                workspace.Paper.SelectedPoints <- Array.empty
            |DisplayTarget.Line(l) ->
                workspace.Paper.SelectedLayers <- Array.empty
                workspace.Paper.SelectedEdges <- Array.empty
                workspace.Paper.SelectedLines <- [| l |]
                workspace.Paper.SelectedPoints <- Array.empty
            |DisplayTarget.Point(p) ->
                workspace.Paper.SelectedLayers <- Array.empty
                workspace.Paper.SelectedEdges <- Array.empty
                workspace.Paper.SelectedLines <- Array.empty
                workspace.Paper.SelectedPoints <- [| p |]

    interface ITool with
        member this.Name = this.Name
        member this.ShortcutKey = this.ShortcutKey
        member this.Icon = this.Icon
        member this.OnClick(target, modifier) = this.OnClick(target, modifier)
        member this.BeginDrag(source, modifier) = this.BeginDrag(source, modifier)
        member this.DragEnter(source, target, modifier) = this.DragEnter(source, target, modifier)
        member this.DragLeave(source, target, modifier) = this.DragLeave(source, target, modifier)
        member this.DragOver(source, target, modifier) = this.DragOver(source, target, modifier)
        member this.Drop(source, target, modifier) = this.Drop(source, target, modifier)
