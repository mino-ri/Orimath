namespace Orimath.Folds.Core
open System
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.FoldingInstruction
open Orimath.Plugins
open FoldOperation
open ApplicativeProperty.PropOperators

type internal InstructionWrapper(paper: IPaper) =
    let instruction = FoldingInstruction()
    member _.Instruction = instruction

    member _.ResetAll() =
        instruction.Lines .<- Array.Empty()
        instruction.Arrows .<- Array.Empty()
        instruction.Points .<- Array.Empty()

    member _.SetLines(lines, chosen) =
        let mapping l = {
                Line = l
                Color =
                match chosen with
                | Some(c) when c =~ l.Line -> InstructionColor.Blue
                | _ -> InstructionColor.LightGray
            }
        instruction.Lines .<- (lines
            |> Seq.collect(paper.Clip)
            |> LineSegmentExtensions.Merge
            |> Seq.map mapping
            |> Seq.toArray)

    member _.ResetArrows() =
        instruction.Arrows .<- Array.Empty()
        instruction.Points .<- Array.Empty()

    member _.SetArrow(source: OperationTarget, target: OperationTarget, chosen: Line, opr: FoldOperation) =
        let getGeneralArrow() =
            match paper.ClipBound(chosen) with
            | None -> Array.Empty()
            | Some(first, last) ->
                let middle = (first + last) / 2.0
                match paper.ClipBound(Fold.axiom4 chosen middle) with
                | None -> Array.Empty()
                | Some(point1, point2) ->
                    let point = if middle.GetDistance(point1) <= middle.GetDistance(point2) then point1 else point2
                    if chosen.Contains(point)
                    then Array.Empty()
                    else [| InstructionArrow.ValleyFold(point, chosen.Reflect(point), InstructionColor.Blue) |]
        let getPerpendicularArrow (line: LineSegment) (chosen: Line) (isEdge: bool) =
            if isEdge then Array.Empty()
            else
                let p1 = chosen.GetSignedDistance(line.Point1)
                let p2 = chosen.GetSignedDistance(line.Point2)
                if sign p1 = sign p2 then Array.Empty()
                else
                    let p = if abs p1 < abs p2 then line.Point1 else line.Point2
                    let reflected = chosen.Reflect(p)
                    if p =~ reflected then Array.Empty()
                    else [| InstructionArrow.ValleyFold(p, chosen.Reflect(p), InstructionColor.Blue) |]
        let arrows, points =
            let swapByDir dir point linePoint =
                match dir with
                | LineToPoint -> linePoint, point
                | PointToLine -> point, linePoint
            match opr with
            | NoOperation -> Array.Empty(), Array.Empty()
            | Axiom1(_, _) -> getGeneralArrow(), Array.Empty()
            | Axiom2(point1, point2) -> [| InstructionArrow.ValleyFold(point1, point2, InstructionColor.Blue) |], Array.Empty()
            | Axiom3((_, point), _) -> [| InstructionArrow.ValleyFold(point, chosen.Reflect(point), InstructionColor.Blue) |], Array.Empty()
            | Axiom4(line, _, isEdge) ->
                let perpendicular = getPerpendicularArrow line chosen isEdge
                (if Array.isEmpty perpendicular then getGeneralArrow() else perpendicular), Array.Empty()
            | Axiom5(_, _, point, dir) ->
                let reflected = chosen.Reflect(point)
                let pStart, pEnd = swapByDir dir point reflected
                [| InstructionArrow.ValleyFold(pStart, pEnd, InstructionColor.Blue) |],
                [| { Point = reflected; Color = InstructionColor.Brown } |]
            | Axiom6(_, point1, _, point2, dir) ->
                let reflected1 = chosen.Reflect(point1)
                let reflected2 = chosen.Reflect(point2)
                let pStart1, pEnd1 = swapByDir dir point1 reflected1
                let pStart2, pEnd2 =
                    if chosen.GetDistanceSign(pStart1) = chosen.GetDistanceSign(point2)
                    then point2, reflected2
                    else reflected2, point2
                [| InstructionArrow.ValleyFold(pStart1, pEnd1, InstructionColor.Blue)
                   InstructionArrow.ValleyFold(pStart2, pEnd2, InstructionColor.Blue) |],
                [| { Point = reflected1; Color = InstructionColor.Brown }
                   { Point = reflected2; Color = InstructionColor.Brown } |]
            | Axiom7(pass, _, point, isEdge, dir) ->
                let reflected = chosen.Reflect(point)
                let pStart, pEnd = swapByDir dir point reflected
                let arrows =
                    Array.append
                        (getPerpendicularArrow pass chosen isEdge)
                        [| InstructionArrow.ValleyFold(pStart, pEnd, InstructionColor.Blue) |]
                arrows, [| { Point = reflected; Color = InstructionColor.Brown } |]
            | AxiomP(_, point, dir) ->
                let reflected = chosen.Reflect(point)
                let pStart, pEnd = swapByDir dir point reflected
                [| InstructionArrow.ValleyFold(pStart, pEnd, InstructionColor.Blue) |], Array.Empty()
        instruction.Arrows .<- arrows
        instruction.Points .<- points
