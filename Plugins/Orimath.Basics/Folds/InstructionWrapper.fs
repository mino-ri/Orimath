namespace Orimath.Basics.Folds
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.FoldingInstruction
open ApplicativeProperty.PropOperators

type internal InstructionWrapper(paper: IPaper) =
    let instruction = FoldingInstruction()
    let center = { X = 0.5; Y = 0.5 }
    member _.Instruction = instruction

    member _.ResetAll() =
        instruction.Lines .<- array.Empty()
        instruction.Arrows .<- array.Empty()
        instruction.Points .<- array.Empty()

    member _.SetLines(layers, lines, chosen) =
        let mapping l = {
                Line = l
                Color =
                match chosen with
                | Some(c) when c =~ l.Line -> InstructionColor.Blue
                | _ -> InstructionColor.LightGray
            }
        instruction.Lines .<- (lines
            |> Seq.collect(fun l -> layers |> Seq.collect (Layer.clip l))
            |> LineSegment.merge
            |> Seq.map mapping
            |> Seq.toArray)

    member _.ResetArrows() =
        instruction.Arrows .<- array.Empty()
        instruction.Points .<- array.Empty()

    member _.SetArrow(chosen, opr, foldBack) =
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
            match Paper.clipBoundBy paper chosen with
            | None -> array.Empty()
            | Some(first, last) ->
                let middle = (first + last) / 2.0
                match Paper.clipBoundBy paper (Fold.axiom4 chosen middle) with
                | None -> array.Empty()
                | Some(point1, point2) ->
                    let point = if middle.GetDistance(point1) <= middle.GetDistance(point2) then point1 else point2
                    if Line.contains point chosen
                    then array.Empty()
                    else [| createArrow point (Point.reflectBy chosen point) center |]
        let getPerpendicularArrow (line: LineSegment) (chosen: Line) (isEdge: bool) =
            if isEdge then array.Empty()
            else
                let p1 = Line.signedDist line.Point1 chosen
                let p2 = Line.signedDist line.Point2 chosen
                if sign p1 = sign p2 then array.Empty()
                else
                    let p = if abs p1 < abs p2 then line.Point1 else line.Point2
                    let reflected = Point.reflectBy chosen p
                    if p =~ reflected then array.Empty()
                    else [| createArrow p (Point.reflectBy chosen p) center |]
        let arrows, points =
            let swapByDir dir point linePoint =
                match dir with
                | LineToPoint -> linePoint, point
                | PointToLine -> point, linePoint
            match opr with
            | NoOperation -> array.Empty(), array.Empty()
            | Axiom1(_, _) -> getGeneralArrow(), array.Empty()
            | Axiom2(point1, point2) -> [| createArrow point1 point2 center |], array.Empty()
            | Axiom3((line1, point), (line2, _)) ->
                let center = Line.cross line1.Line line2.Line |> Option.defaultValue center
                [| createArrow point (Point.reflectBy chosen point) center |], array.Empty()
            | Axiom4(line, _, isEdge) ->
                let perpendicular = getPerpendicularArrow line chosen isEdge
                (if Array.isEmpty perpendicular then getGeneralArrow() else perpendicular), array.Empty()
            | Axiom5(pass, _, point, dir) ->
                let reflected = Point.reflectBy chosen point
                let pStart, pEnd = swapByDir dir point reflected
                [| createArrow pStart pEnd pass |],
                [| { Point = reflected; Color = InstructionColor.Brown } |]
            | Axiom6(_, point1, _, point2, dir) ->
                let reflected1 = Point.reflectBy chosen point1
                let reflected2 = Point.reflectBy chosen point2
                let pStart2, pEnd2 = swapByDir dir point2 reflected2
                let pStart1, pEnd1 =
                    if Line.distSign pStart2 chosen = Line.distSign point1 chosen
                    then point1, reflected1
                    else reflected1, point1
                [| createArrow pStart1 pEnd1 center
                   createArrow pStart2 pEnd2 center |],
                [| { Point = reflected1; Color = InstructionColor.Brown }
                   { Point = reflected2; Color = InstructionColor.Brown } |]
            | Axiom7(pass, _, point, isEdge, dir) ->
                let reflected = Point.reflectBy chosen point
                let pStart, pEnd = swapByDir dir point reflected
                let arrows =
                    Array.append
                        (getPerpendicularArrow pass chosen isEdge)
                        [| createArrow pStart pEnd center |]
                arrows, [| { Point = reflected; Color = InstructionColor.Brown } |]
            | AxiomP(_, point, dir) ->
                let reflected = Point.reflectBy chosen point
                let pStart, pEnd = swapByDir dir point reflected
                [| createArrow pStart pEnd center |], array.Empty()
        instruction.Arrows .<- arrows
        instruction.Points .<- points
