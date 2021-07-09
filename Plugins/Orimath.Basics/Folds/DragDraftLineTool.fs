namespace Orimath.Basics.Folds
open Orimath.Core
open Orimath.Combination
open Orimath.Plugins
open FoldOperation
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type DragDraftLineTool(workspace: IWorkspace) =
    let center = { X = 0.5; Y = 0.5 }
    let paper = workspace.Paper
    let instruction = InstructionWrapper(paper)
    let state = Prop.value DragFoldState.Ready
    let selector = DisplayTargetSelector(workspace.Paper)

    member _.UpdateState(modifier: OperationModifier) =
        state .<-
            DragFoldState.Dragging(
                modifier.HasRightButton, modifier.HasShift,
                modifier.HasCtrl, modifier.HasAlt)

    member _.ResetState() = state .<- DragFoldState.Ready

    member _.State = state

    member private _.Selection =
        let point =
            match paper.SelectedPoints.Value with
            | [| point |] -> Some(OprPoint(point, selector.PointIndex))
            | _ -> None
        let line =
            match paper.SelectedCreases.Value, paper.SelectedEdges.Value with
            | [| crease |], _ -> Some(OprLine(crease.Segment, center, false, selector.LineIndex))
            | _, [| edge |] -> Some(OprLine(edge.Segment, center, true, selector.LineIndex))
            | _ -> None
        point, line

    member private _.Fold(operation) =
        let method = operation.Method
        match chooseLine (getLines method) method with
        | Some(line) ->
            use __ = paper.BeginChange(NoInstruction)
            match operation.CreaseType, operation.IsFrontOnly with
            | _, true ->
                for l in FoldBack.getTargetLayers paper line method do
                    (l :?> ILayerModel).AddCreases(Layer.clip line l |> Crease.ofSegs operation.CreaseType)
            | _, false ->
                for layer in paper.Layers do
                    layer.AddCreases(Layer.clip line layer |> Crease.ofSegs operation.CreaseType)
        | None -> ()
        instruction.ResetAll()

    interface ITool with
        member _.Name = "{basic/Tool.Draft}Draft line"
        member _.ShortcutKey = "Ctrl+D"
        member _.Icon = Orimath.Basics.Internal.getIcon "draft"
        member _.OnActivated() =
            paper.SelectedLayers .<- array.Empty()
            paper.SelectedPoints .<- array.Empty()
            paper.SelectedCreases .<- array.Empty()
            paper.SelectedEdges .<- array.Empty()
        member _.OnDeactivated() = ()

    interface IClickTool with
        member this.OnClick(target, modifier) =
            if not modifier.HasRightButton then
                selector.OnClick(target, modifier)

    interface IDragTool with
        member this.BeginDrag(source, modifier) =
            match source with
            | FreePoint true _ | LineOrEdge _ ->
                this.UpdateState(modifier)
                true
            | _ -> false

        member this.DragEnter(source, target, modifier) =
            this.UpdateState(modifier)
            let selectedPoint, selectedLine = this.Selection
            let opr, previewOnly = getPreviewDraftFoldOperation paper selectedPoint selectedLine source target modifier
            instruction.Set(opr, previewOnly)
            match target with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragLeave(_, target, modifier) =
            this.UpdateState(modifier)
            instruction.ResetAll()
            match target with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragOver(source, target, modifier) =
            this.UpdateState(modifier)
            (this :> IDragTool).DragEnter(source, target, modifier)

        member this.Drop(source, target, modifier) =
            let selectedPoint, selectedLine = this.Selection
            getDraftFoldOperation paper selectedPoint selectedLine source target modifier
            |> this.Fold
            this.ResetState()

        member this.CancelDrag(_, _) = this.ResetState()

    interface IFoldingInstructionTool with
        member _.FoldingInstruction = instruction.Instruction
