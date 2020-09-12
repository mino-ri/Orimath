namespace Orimath.Folds.Core
open System
open System.Reflection
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.FoldingInstruction
open Orimath.Plugins

type FoldOperation =
    | NoOperation
    | Axiom1 of Point * Point
    | Axiom2 of Point * Point
    | Axiom3 of (Line * Point) * (Line * Point)
    | Axiom4 of Line * Point
    | Axiom5 of pass: Point * (Line * Point) * Point
    | Axiom6 of Line * Point * (Line * Point) * Point
    | Axiom7 of pass: Line * Line * Point
    | AxiomP of Line * Point

type DragFoldTool(workspace: IWorkspace) =
    let paper = workspace.Paper
    let instruction = FoldingInstruction()

    let (|FreePoint|_|) (free: bool) (dt: OperationTarget) =
        match dt.Target with
        | DisplayTarget.Point(point) -> Some(point)
        | DisplayTarget.Layer(_) when free -> Some(dt.Point)
        | _ -> None
    let (|LineOrEdge|_|) (dt: OperationTarget) =
        match dt.Target with
        | DisplayTarget.Line(line) -> Some(line.Line, dt.Point)
        | DisplayTarget.Edge(edge) -> Some(edge.Line.Line, dt.Point)
        | _ -> None

    member private __.GetPass() =
        let passPoint = Array.tryItem 0 paper.SelectedPoints
        let passLine =
            if paper.SelectedEdges.Length > 0
            then Some(paper.SelectedEdges.[0].Line)
            else Array.tryItem 0 paper.SelectedLines
        passPoint, passLine

    member private this.GetOperation(source: OperationTarget, target: OperationTarget, modifier: OperationModifier) =
        let free = modifier.HasFlag(OperationModifier.Alt)
        if modifier.HasFlag(OperationModifier.RightButton) then
            match source, target with
            | FreePoint free (point1), FreePoint free (point2) -> Axiom1(point1, point2)
            | FreePoint free (point), LineOrEdge(line, _)
            | LineOrEdge(line, _), FreePoint free (point) -> Axiom4(line, point)
            | _ -> NoOperation
        else
            match source, target with
            | FreePoint free (point1), FreePoint free (point2) -> Axiom2(point1, point2)
            | LineOrEdge(line1), LineOrEdge(line2) -> Axiom3(line1, line2)
            | FreePoint free (point), LineOrEdge(line)
            | LineOrEdge(line), FreePoint free (point) ->
                match this.GetPass() with
                | Some(pass), None -> Axiom5(pass, line, point)
                | None, Some(pass) -> Axiom7(pass.Line, fst line, point)
                | Some(p), Some(l) -> Axiom6(l.Line, p, line, point)
                | _ -> AxiomP(fst line, point)
            | _ -> NoOperation
        
    member private __.GetLines(opr) =
        match opr with
        | NoOperation -> []
        | Axiom1(point1, point2) -> Fold.axiom1 point1 point2 |> Option.toList
        | Axiom2(point1, point2) -> Fold.axiom2 point1 point2 |> Option.toList
        | Axiom3((line1, _), (line2, _)) -> Fold.axiom3 line1 line2
        | Axiom4(line, point) -> [Fold.axiom4 line point]
        | Axiom5(pass, (line, _), point) -> Fold.axiom5 pass line point
        | Axiom6(line1, point1, (line2, _), point2) -> Fold.axiom6 line1 point1 line2 point2
        | Axiom7(pass, line, point) -> Fold.axiom7 pass line point |> Option.toList
        | AxiomP(line, point) -> Fold.axiomP line point |> Option.toList

    member private __.ChooseLine(lines: Line list, opr: FoldOperation) =
        match lines with
        | [] -> None
        | [l] -> Some(l)
        | _ ->
            match opr with
            | Axiom3((_, point1), (_, point2)) ->
                match Fold.axiom1 point1 point2 with
                | None -> Some(lines.[0])
                | Some(opLine) ->
                    match opLine.GetCrossPoint(lines.[0]) with
                    | None -> Some(lines.[0])
                    | Some(cross) ->
                        let x1, x2 = if point1.X <= point2.X then point1.X, point2.X else point2.X, point1.X
                        let y1, y2 = if point1.Y <= point2.Y then point1.Y, point2.Y else point2.Y, point1.Y
                        if x1 <=~ cross.X && cross.X <=~ x2 && y1 <=~ cross.Y && cross.Y <=~ y2
                        then Some(lines.[0])
                        else Some(lines.[1])
            | Axiom5(_, (_, point1), point2)
            | Axiom6(_, _, (_, point1), point2) ->
                Some(lines |> List.minBy(fun line -> point1.GetDistance(line.Reflect(point2))))
            | _ -> Some(lines.[0])

    member private __.SetArrow(source: OperationTarget, target: OperationTarget, chosen: Line, opr: FoldOperation) =
        instruction.Arrows <-
            match opr with
            | NoOperation -> Array.Empty()
            | Axiom1(_, _)
            | Axiom4(_, _) ->
                match paper.Layers.[0].ClipBound(chosen) with
                | None -> Array.Empty()
                | Some(first, last) ->
                    let middle = (first + last) / 2.0
                    match paper.Layers.[0].ClipBound(Fold.axiom4 chosen middle) with
                    | None -> Array.Empty()
                    | Some(point1, point2) ->
                        let point = if middle.GetDistance(point1) <= middle.GetDistance(point2) then point1 else point2
                        if chosen.Contains(point)
                        then Array.Empty()
                        else [| InstructionArrow.ValleyFold(point, chosen.Reflect(point), InstructionColor.Blue) |]
            | Axiom2(point1, point2) -> [| InstructionArrow.ValleyFold(point1, point2, InstructionColor.Blue) |]
            | Axiom3((_, point), _) -> [| InstructionArrow.ValleyFold(point, chosen.Reflect(point), InstructionColor.Blue) |]
            | Axiom5(_, _, point) -> [| InstructionArrow.ValleyFold(point, chosen.Reflect(point), InstructionColor.Blue) |]
            | Axiom6(_, point1, _, point2) -> [| InstructionArrow.ValleyFold(point1, chosen.Reflect(point1), InstructionColor.Blue)
                                                 InstructionArrow.ValleyFold(point2, chosen.Reflect(point2), InstructionColor.Blue) |]
            | Axiom7(_, _, point) -> [| InstructionArrow.ValleyFold(point, chosen.Reflect(point), InstructionColor.Blue) |]
            | AxiomP(_, point) ->
                match source with
                | LineOrEdge _ -> [| InstructionArrow.ValleyFold(chosen.Reflect(point), point, InstructionColor.Blue) |]
                | _ -> [| InstructionArrow.ValleyFold(point, chosen.Reflect(point), InstructionColor.Blue) |]

    interface ITool with
        member __.Name = "折り線"
        member __.ShortcutKey = "F"
        member __.Icon = Assembly.GetExecutingAssembly().GetManifestResourceStream("Orimath.Basics.Icon.png")
        member __.OnActivated() =
            paper.SelectedLayers <- array.Empty()
            paper.SelectedPoints <- array.Empty()
            paper.SelectedLines <- array.Empty()
            paper.SelectedEdges <- array.Empty()
        member __.OnDeactivated() = ()
        member __.OnClick(target, modifier) =
            if not (modifier.HasFlag(OperationModifier.RightButton)) then
                let clearOther = not (modifier.HasFlag(OperationModifier.Shift))
                match target.Target with
                | DisplayTarget.Point(point) ->
                    paper.SelectedPoints <-
                        if paper.IsSelected(point) then array.Empty() else [| point |]
                    if clearOther then
                        paper.SelectedLines <- array.Empty()
                        paper.SelectedEdges <- array.Empty()
                | DisplayTarget.Line(line) ->
                    paper.SelectedLines <-
                        if paper.IsSelected(line) then array.Empty() else [| line |]
                    paper.SelectedEdges <- array.Empty()
                    if clearOther then
                        paper.SelectedPoints <- array.Empty()
                | DisplayTarget.Edge(edge) ->
                    paper.SelectedLines <- array.Empty()
                    paper.SelectedEdges <-
                        if paper.IsSelected(edge) then array.Empty() else [| edge |]
                    if clearOther then
                        paper.SelectedPoints <- array.Empty()
                | _ ->
                    paper.SelectedPoints <- array.Empty()
                    paper.SelectedLines <- array.Empty()
                    paper.SelectedEdges <- array.Empty()

        member __.BeginDrag(source, modifier) =
            match source with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragEnter(source, target, modifier) =
            let opr = this.GetOperation(source, target, modifier)
            let lines, chosen =
                match opr with
                | NoOperation ->
                    let lines = this.GetLines(this.GetOperation(source, target, modifier ||| OperationModifier.Alt))
                    lines, None
                | _ ->
                    let lines = this.GetLines(opr)
                    lines, this.ChooseLine(lines, opr)
            let mapping l = {
                  Line = l
                  Color =
                    match chosen with
                    | Some(c) when c =~ l.Line -> InstructionColor.Blue
                    | _ -> InstructionColor.LightGray
                }
            instruction.Lines <- lines
                |> Seq.collect(paper.Layers.[0].Clip)
                |> Seq.map mapping
                |> Seq.toArray

            match chosen with
            | Some(c) -> this.SetArrow(source, target, c, opr)
            | None -> instruction.Arrows <- Array.Empty()

            match target with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member __.DragLeave(_, target, _) =
            instruction.Lines <- Array.Empty()
            instruction.Arrows <- Array.Empty()
            match target with
            | FreePoint true _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragOver(source, target, modifier) = (this :> ITool).DragEnter(source, target, modifier)

        member this.Drop(source, target, modifier) =
            let opr = this.GetOperation(source, target, modifier)
            let lines = this.ChooseLine(this.GetLines(opr), opr) |> Option.toList
            use __ = paper.BeginChange()
            for layer in paper.Layers do layer.AddLines(lines)
            instruction.Lines <- Array.Empty()
            instruction.Arrows <- Array.Empty()

    interface IFoldingInstructionTool with
        member __.FoldingInstruction = instruction