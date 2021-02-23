namespace Orimath.Basics.Folds
open Orimath.Core
open Orimath.Combination
open Orimath.Plugins
open FoldOperation
open ApplicativeProperty.PropOperators

type DragFoldTool(workspace: IWorkspace) =
    let paper = workspace.Paper
    let instruction = InstructionWrapper(paper)

    member private _.GetSourcePoint(opr, chosen) =
        getSourcePoint opr
        |> Option.orElseWith (fun () -> Array.tryHead paper.SelectedPoints.Value)
        |> Option.orElseWith (fun () ->
            match opr with
            | Axiom1 _ -> FoldBack.getGeneralDynamicPoint paper chosen
            | Axiom4(seg, _, isEdge) ->
                FoldBack.getPerpendicularDynamicPoint seg chosen isEdge
                |> Option.orElseWith (fun () -> FoldBack.getGeneralDynamicPoint paper chosen)
            | _ -> None)

    member _.MakeCrease(line: Line) =
        for layer in paper.Layers do layer.AddCreases([ line ])

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
        member _.OnClick(target, modifier) =
            if modifier.HasFlag(OperationModifier.RightButton) then
                match target.Target with
                | DisplayTarget.Crease(crease) ->
                    let dynamicPoint = paper.SelectedPoints.Value |> Array.tryHead
                    if modifier.HasFlag(OperationModifier.Ctrl)
                    then FoldBack.foldBackFirst workspace crease.Line dynamicPoint []
                    else FoldBack.foldBack workspace crease.Line dynamicPoint
                | _ -> ()
            else
                let clearOther = not (modifier.HasFlag(OperationModifier.Shift))
                match target.Target with
                | DisplayTarget.Point(point) ->
                    paper.SelectedPoints .<-
                        if paper.IsSelected(point) then array.Empty() else [| point |]
                    if clearOther then
                        paper.SelectedCreases .<- array.Empty()
                        paper.SelectedEdges .<- array.Empty()
                | DisplayTarget.Crease(crease) ->
                    paper.SelectedCreases .<-
                        if paper.IsSelected(crease) then array.Empty() else [| crease |]
                    paper.SelectedEdges .<- array.Empty()
                    if clearOther then
                        paper.SelectedPoints .<- array.Empty()
                | DisplayTarget.Edge(edge) ->
                    paper.SelectedCreases .<- array.Empty()
                    paper.SelectedEdges .<-
                        if paper.IsSelected(edge) then array.Empty() else [| edge |]
                    if clearOther then
                        paper.SelectedPoints .<- array.Empty()
                | _ ->
                    paper.SelectedPoints .<- array.Empty()
                    paper.SelectedCreases .<- array.Empty()
                    paper.SelectedEdges .<- array.Empty()

    interface IDragTool with
        member _.BeginDrag(source, _) =
            match source with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragEnter(source, target, modifier) =
            let opr = getOperation paper source target modifier
            let lines, chosen =
                match opr with
                | NoOperation ->
                    getLines (getOperation paper source target (modifier ||| OperationModifier.Alt)),
                    None
                | _ ->
                    let lines = getLines opr
                    lines, chooseLine lines opr
            match chosen with
            | Some(c) ->
                let targetLayers =
                    if modifier.HasFlag(OperationModifier.Ctrl) then
                        FoldBack.getTargetLayers workspace c (this.GetSourcePoint(opr, c)) [source; target] :> seq<_>
                    else
                        paper.Layers :> seq<_>
                instruction.SetLines(targetLayers, lines, chosen)
                instruction.SetArrow(c, opr, modifier.HasFlag(OperationModifier.Shift))
            | None ->
                instruction.SetLines(paper.Layers, lines, chosen)
                instruction.ResetArrows()
            match target with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member _.DragLeave(_, target, _) =
            instruction.ResetAll()
            match target with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragOver(source, target, modifier) = (this :> IDragTool).DragEnter(source, target, modifier)

        member this.Drop(source, target, modifier) =
            let opr = getOperation paper source target modifier
            match chooseLine (getLines opr) opr with
            | Some(line) ->
                use __ = paper.BeginChange()
                match modifier.HasFlag(OperationModifier.Shift), modifier.HasFlag(OperationModifier.Ctrl) with
                | false, false -> this.MakeCrease(line)
                | false, true ->
                    for l in FoldBack.getTargetLayers workspace line
                        (this.GetSourcePoint(opr, line)) [source; target] do
                        l.AddCreases([ line ])
                | true, false -> FoldBack.foldBack workspace line (this.GetSourcePoint(opr, line))
                | true, true -> FoldBack.foldBackFirst workspace line (this.GetSourcePoint(opr, line)) [source; target]
            | None -> ()
            instruction.ResetAll()

    interface IFoldingInstructionTool with
        member _.FoldingInstruction = instruction.Instruction
