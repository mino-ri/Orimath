﻿namespace Orimath.Basics.Folds
open Orimath.Core
open Orimath.Combination
open Orimath.Plugins
open FoldOperation
open ApplicativeProperty
open ApplicativeProperty.PropOperators

[<RequireQualifiedAccess>]
type DragFoldState =
    | Ready
    | Dragging of through: bool * foldBack: bool * frontMost: bool * free : bool


type DragFoldTool(workspace: IWorkspace) =
    let center = { X = 0.5; Y = 0.5 }
    let paper = workspace.Paper
    let instruction = InstructionWrapper(paper)
    let mutable selectedPointIndex = 0
    let mutable selectedLineIndex = 0
    let state = Prop.value DragFoldState.Ready

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
            | [| point |] -> Some(OprPoint(point, selectedPointIndex))
            | _ -> None
        let line =
            match paper.SelectedCreases.Value, paper.SelectedEdges.Value with
            | [| crease |], _ -> Some(OprLine(crease.Segment, center, false, selectedLineIndex))
            | _, [| edge |] -> Some(OprLine(edge.Segment, center, true, selectedLineIndex))
            | _ -> None
        point, line

    member private _.Fold(operation) =
        let method = operation.Method
        match chooseLine (getLines method) method with
        | Some(line) ->
            use __ = paper.BeginChange(operation)
            match operation.CreaseType, operation.IsFrontOnly with
            | CreaseType.ValleyFold, false ->
                FoldBack.foldBack workspace line (getSourcePoint method |> List.tryHead)
            | CreaseType.ValleyFold, true ->
                FoldBack.foldBackFirst workspace line method
            | _, true ->
                for l in FoldBack.getTargetLayers paper line method do
                    (l :?> ILayerModel).AddCreases([ line ])
            | _, false ->
                for layer in paper.Layers do layer.AddCreases([ line ])
        | None -> ()
        instruction.ResetAll()

    interface ITool with
        member _.Name = "{basic/Tool.Folding}Folding"
        member _.ShortcutKey = "Ctrl+F"
        member _.Icon = Orimath.Basics.Internal.getIcon "fold"
        member _.OnActivated() =
            paper.SelectedLayers .<- array.Empty()
            paper.SelectedPoints .<- array.Empty()
            paper.SelectedCreases .<- array.Empty()
            paper.SelectedEdges .<- array.Empty()
        member _.OnDeactivated() = ()

    interface IClickTool with
        member this.OnClick(target, modifier) =
            if modifier.HasRightButton then
                match target.Target with
                | DisplayTarget.Crease(crease) ->
                    this.Fold({
                        Method =
                            Axiom1(fst this.Selection,
                                OprPoint(crease.Point1, target.Layer.Index),
                                OprPoint(crease.Point2, target.Layer.Index))
                        CreaseType = CreaseType.ValleyFold
                        IsFrontOnly = modifier.HasCtrl
                    })
                | _ -> ()
            else
                let clearOther = not modifier.HasShift
                match target.Target with
                | DisplayTarget.Point(point) ->
                    if paper.IsSelected(point) then
                        paper.SelectedPoints .<- array.Empty()
                    else
                        paper.SelectedPoints .<- [| point |]
                        selectedPointIndex <- target.Layer.Index
                    if clearOther then
                        paper.SelectedCreases .<- array.Empty()
                        paper.SelectedEdges .<- array.Empty()
                | DisplayTarget.Crease(crease) ->
                    if paper.IsSelected(crease) then
                        paper.SelectedCreases .<- array.Empty()
                    else
                        paper.SelectedCreases .<- [| crease |]
                        selectedLineIndex <- target.Layer.Index
                    paper.SelectedEdges .<- array.Empty()
                    if clearOther then
                        paper.SelectedPoints .<- array.Empty()
                | DisplayTarget.Edge(edge) ->
                    if paper.IsSelected(edge) then
                        paper.SelectedEdges .<- array.Empty()
                    else
                        paper.SelectedEdges .<- [| edge |]
                        selectedLineIndex <- target.Layer.Index
                    paper.SelectedCreases .<- array.Empty()
                    if clearOther then
                        paper.SelectedPoints .<- array.Empty()
                | _ ->
                    paper.SelectedPoints .<- array.Empty()
                    paper.SelectedCreases .<- array.Empty()
                    paper.SelectedEdges .<- array.Empty()

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
            let opr, previewOnly =
                match getFoldOperation paper selectedPoint selectedLine source target modifier with
                | { Method = NoOperation } when not modifier.HasAlt ->
                    let modifier = modifier ||| OperationModifier.Alt
                    getFoldOperation paper selectedPoint selectedLine source target modifier, true
                | opr -> opr, false
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
            getFoldOperation paper selectedPoint selectedLine source target modifier
            |> this.Fold
            this.ResetState()

        member this.CancelDrag(_, _) = this.ResetState()

    interface IFoldingInstructionTool with
        member _.FoldingInstruction = instruction.Instruction
