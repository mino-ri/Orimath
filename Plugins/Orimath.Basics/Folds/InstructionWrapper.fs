namespace Orimath.Basics.Folds
open System
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.FoldingInstruction
open Orimath.Plugins
open ApplicativeProperty.PropOperators

type internal InstructionWrapper(paper: IPaper) =
    let instruction = FoldingInstruction()
    let center = { X = 0.5; Y = 0.5 }
    member _.Instruction = instruction

    member _.ResetAll() =
        instruction.Lines .<- Array.Empty()
        instruction.Arrows .<- Array.Empty()
        instruction.Points .<- Array.Empty()

    member _.SetLines(layers: seq<ILayerModel>, lines: seq<Line>, chosen) =
        let mapping l = {
                Line = l
                Color =
                match chosen with
                | Some(c) when c =~ l.Line -> InstructionColor.Blue
                | _ -> InstructionColor.LightGray
            }
        instruction.Lines .<- (layers
            |> Seq.collect(fun ly -> lines |> Seq.collect ly.Clip)
            |> LineSegmentExtensions.Merge
            |> Seq.map mapping
            |> Seq.toArray)

    member _.ResetArrows() =
        instruction.Arrows .<- Array.Empty()
        instruction.Points .<- Array.Empty()

    member _.SetArrow(chosen: Line, opr: FoldOperation, foldBack: bool) =
        let createArrow startPoint endPoint center =
            let dir =
                let s = startPoint - center
                let d = endPoint - center
                let dot = s.X * d.Y - s.Y * d.X
                if dot =~~ 0.0 then ArrowDirection.Auto
                elif dot < 0.0 then ArrowDirection.Clockwise
                else ArrowDirection.Counterclockwise
            if foldBack
            then InstructionArrow.ValleyFold(startPoint, endPoint, InstructionColor.Blue, dir)
            else InstructionArrow.Create(startPoint, endPoint, ArrowType.ValleyFold, ArrowType.ValleyFold, InstructionColor.Green, dir)
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
                    else [| createArrow point (chosen.Reflect(point)) center |]
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
                    else [| createArrow p (chosen.Reflect(p)) center |]
        let arrows, points =
            let swapByDir dir point linePoint =
                match dir with
                | LineToPoint -> linePoint, point
                | PointToLine -> point, linePoint
            match opr with
            | NoOperation -> Array.Empty(), Array.Empty()
            | Axiom1(_, _) -> getGeneralArrow(), Array.Empty()
            | Axiom2(point1, point2) -> [| createArrow point1 point2 center |], Array.Empty()
            | Axiom3((line1, point), (line2, _)) ->
                let center = line1.Line.GetCrossPoint(line2.Line) |> Option.defaultValue center
                [| createArrow point (chosen.Reflect(point)) center |], Array.Empty()
            | Axiom4(line, _, isEdge) ->
                let perpendicular = getPerpendicularArrow line chosen isEdge
                (if Array.isEmpty perpendicular then getGeneralArrow() else perpendicular), Array.Empty()
            | Axiom5(pass, _, point, dir) ->
                let reflected = chosen.Reflect(point)
                let pStart, pEnd = swapByDir dir point reflected
                [| createArrow pStart pEnd pass |],
                [| { Point = reflected; Color = InstructionColor.Brown } |]
            | Axiom6(_, point1, _, point2, dir) ->
                let reflected1 = chosen.Reflect(point1)
                let reflected2 = chosen.Reflect(point2)
                let pStart2, pEnd2 = swapByDir dir point2 reflected2
                let pStart1, pEnd1 =
                    if chosen.GetDistanceSign(pStart2) = chosen.GetDistanceSign(point1)
                    then point1, reflected1
                    else reflected1, point1
                [| createArrow pStart1 pEnd1 center
                   createArrow pStart2 pEnd2 center |],
                [| { Point = reflected1; Color = InstructionColor.Brown }
                   { Point = reflected2; Color = InstructionColor.Brown } |]
            | Axiom7(pass, _, point, isEdge, dir) ->
                let reflected = chosen.Reflect(point)
                let pStart, pEnd = swapByDir dir point reflected
                let arrows =
                    Array.append
                        (getPerpendicularArrow pass chosen isEdge)
                        [| createArrow pStart pEnd center |]
                arrows, [| { Point = reflected; Color = InstructionColor.Brown } |]
            | AxiomP(_, point, dir) ->
                let reflected = chosen.Reflect(point)
                let pStart, pEnd = swapByDir dir point reflected
                [| createArrow pStart pEnd center |], Array.Empty()
        instruction.Arrows .<- arrows
        instruction.Points .<- points
