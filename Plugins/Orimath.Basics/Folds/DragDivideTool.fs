namespace Orimath.Basics.Folds
open Orimath.Core
open Orimath.Combination
open Orimath.Plugins
open FoldOperation
open ApplicativeProperty
open ApplicativeProperty.PropOperators
open Orimath.Core.NearlyEquatable

type DragDivideTool(workspace: IWorkspace) =
    let paper = workspace.Paper
    let instruction = InstructionWrapper(paper)
    let divisionNumber = Prop.value 5
    let draft = Prop.value false

    member _.GetLines(source, target, isPreview) =
        match source, target with
        | LineOrEdge(src), LineOrEdge(tgt) ->
            Fold.divide divisionNumber.Value src.Line tgt.Line
        | FreePoint isPreview (src), FreePoint isPreview (tgt) ->
            match Line.FromPoints(src.Point, tgt.Point) with
            | Some(line) ->
                Fold.divide divisionNumber.Value (Fold.axiom4 line src.Point) (Fold.axiom4 line tgt.Point)
            | None -> []
        | LineOrEdge(RawLine(line)), FreePoint isPreview (RawPoint(point))
        | FreePoint isPreview (RawPoint(point)), LineOrEdge(RawLine(line)) ->
            Line.FromFactorsAndPoint(line.XFactor, line.YFactor, point)
            |> Fold.divide divisionNumber.Value line
        | _ -> []

    member _.ChooseLine(source: OperationTarget, target: OperationTarget, lines: Line list list) =
        let point1 = source.Point
        let point2 = target.Point
        match Fold.axiom1 point1 point2 with
        | None -> lines.[0]
        | Some(opLine) ->
            match Line.cross opLine lines.[0].[0] with
            | None -> lines.[0]
            | Some(cross) ->
                let x1, x2 = if point1.X <= point2.X then point1.X, point2.X else point2.X, point1.X
                let y1, y2 = if point1.Y <= point2.Y then point1.Y, point2.Y else point2.Y, point1.Y
                if x1 <=~ cross.X && cross.X <=~ x2 && y1 <=~ cross.Y && cross.Y <=~ y2
                then lines.[0]
                else lines.[1]

    member _.GetInstructionLines(lines, chosens) =
        [|
            for lines in lines do
            for seg in lines |> Seq.collect (Paper.clipBy paper) do
            {
                Line = seg
                Color =
                    if chosens |> List.exists ((=~) (seg.Line))
                    then InstructionColor.Blue
                    else InstructionColor.LightGray
            }
        |]

    interface ITool with
        member _.Name = "{basic/Tool.NDivide}n-divide"
        member _.ShortcutKey = "Ctrl+D"
        member _.Icon = Orimath.Basics.Internal.getIcon "draft"
        member _.OnActivated() =
            paper.SelectedLayers .<- array.Empty()
            paper.SelectedPoints .<- array.Empty()
            paper.SelectedCreases .<- array.Empty()
            paper.SelectedEdges .<- array.Empty()
        member _.OnDeactivated() = ()

    interface IDragTool with
        member _.BeginDrag(source, _) =
            match source with
            | LineOrEdge _
            | FreePoint true _ -> true
            | _ -> false

        member this.DragEnter(source, target, modifier) =
            let isFree = modifier.HasAlt
            match this.GetLines(source, target, isFree) with
            | [] when not isFree ->
                this.GetInstructionLines(this.GetLines(source, target, true), [])
                |> instruction.SetLines
            | [] -> ()
            | lines ->
                let chosens = this.ChooseLine(source, target, lines)
                instruction.SetLines(this.GetInstructionLines(lines, chosens))
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

        member this.Drop(source, target, modifier) =
            let isFree = modifier.HasAlt
            match this.GetLines(source, target, isFree) with
            | [] -> ()
            | lines ->
                use __ = paper.BeginChange(NoInstruction)
                let chosens = this.ChooseLine(source, target, lines)
                for layer in paper.Layers do
                    layer.AddCreases(
                        chosens
                        |> Seq.collect (fun line -> Layer.clip line layer)
                        |> Crease.ofSegs (if draft.Value then CreaseType.Draft else CreaseType.Crease))
            instruction.ResetAll()

        member _.CancelDrag(_, _) = ()

    interface IFoldingInstructionTool with
        member _.FoldingInstruction = instruction.Instruction

    interface IExtendTool with
        member _.ExtendSettings(ws: IExtendToolWorkspace): unit = 
            ws.AddBooleanSetting("{basic/DragDivide.Draft}draft", draft)
            ws.AddInt32Setting("{basic/DragDivide.DivisionNumber}division number", divisionNumber, 2, 32)
            