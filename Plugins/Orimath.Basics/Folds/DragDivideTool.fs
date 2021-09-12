namespace Orimath.Basics.Folds
open Orimath.Core
open Orimath.Combination
open Orimath.Plugins
open FoldOperation
open DivideOperation
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type DragDivideTool(workspace: IWorkspace) =
    let paper = workspace.Paper
    let instruction = InstructionWrapper(paper)
    let selector = DisplayTargetSelector(workspace.Paper)
    let divisionNumber = Prop.value 5
    let draft = Prop.value false

    interface ITool with
        member _.Name = "{basic/Tool.DragDivide}n-divide folding"
        member _.ShortcutKey = "Ctrl+D"
        member _.Icon = Orimath.Basics.Internal.getIcon "divide"
        member _.OnActivated() =
            paper.SelectedLayers .<- array.Empty()
            paper.SelectedPoints .<- array.Empty()
            paper.SelectedCreases .<- array.Empty()
            paper.SelectedEdges .<- array.Empty()
        member _.OnDeactivated() = ()

    interface IClickTool with
        member _.OnClick(target, modifier) =
            if not modifier.HasRightButton then selector.OnClick(target, modifier)

    interface IDragTool with
        member _.BeginDrag(source, _) =
            match source with
            | LineOrEdge _
            | FreePoint true _ -> true
            | _ -> false

        member _.DragEnter(source, target, modifier) =
            let opr, isPreviewOnly =
                getPreviewDivideOperation source target modifier draft.Value divisionNumber.Value
            instruction.SetLines(getInstructionLines paper opr isPreviewOnly true)
            match target with
            | LineOrEdge _
            | FreePoint true _ -> true
            | _ -> false

        member _.DragLeave(_, target, _) =
            instruction.ResetAll()
            match target with
            | LineOrEdge _
            | FreePoint true _ -> true
            | _ -> false

        member this.DragOver(source, target, modifier) =
            (this :> IDragTool).DragEnter(source, target, modifier)

        member _.Drop(source, target, modifier) =
            let opr = getDivideOperation source target modifier draft.Value divisionNumber.Value
            match getLines opr with
            | [] -> ()
            | lines ->
                let tag = if opr.CreaseType = CreaseType.Draft then box NoInstruction else box opr
                use __ = paper.BeginChange(tag)
                let chosens = chooseLine opr lines
                for layer in Seq.toArray paper.Layers do
                    let creases =
                        chosens
                        |> Seq.collect (fun line -> Layer.clip line layer)
                        |> Crease.ofSegs (if draft.Value then CreaseType.Draft else CreaseType.Crease)
                    paper.MergeCreases(layer, creases)
            instruction.ResetAll()

        member _.CancelDrag(_, _) = ()

    interface IFoldingInstructionTool with
        member _.FoldingInstruction = instruction.Instruction

    interface IExtendTool with
        member _.ExtendSettings(ws: IExtendToolWorkspace): unit = 
            ws.AddBooleanSetting("{basic/DragDivide.Draft}draft", draft)
            ws.AddInt32Setting("{basic/DragDivide.DivisionNumber}division number", divisionNumber, 2, 32)
