namespace Orimath.Basics.Folds
open System
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.Combination
open ApplicativeProperty.PropOperators
open Orimath.Basics.Internal
open FoldOperation

module Instruction =
    let private center = { X = 0.5; Y = 0.5 }

    let private getLines layers lines chosen =
        let mapping l = {
            Line = l
            Color =
                match chosen with
                | Some(c) when c =~ l.Line -> InstructionColor.Blue
                | _ -> InstructionColor.LightGray
        }
        lines
        |> Seq.collect (fun l -> Seq.collect (Layer.clip l) layers)
        |> LineSegment.merge
        |> Seq.map mapping
        |> Seq.toArray

    let private getChosenLine layers (_: seq<Line>) chosen =
        match chosen with
        | Some(chosen) ->
            Seq.collect (Layer.clip chosen) layers
            |> LineSegment.merge
            |> Seq.map (fun l -> { Line = l; Color = InstructionColor.Blue })
            |> Seq.toArray
        | None -> Array.Empty()

    let private getArrow chosen method foldBack =
        let createArrow startPoint endPoint center =
            let dir =
                let s = startPoint - center
                let d = endPoint - center
                let dot = s.X * d.Y - s.Y * d.X
                if dot =~~ 0.0 then ArrowDirection.Auto
                elif dot < 0.0 then ArrowDirection.Clockwise
                else ArrowDirection.Counterclockwise
            if foldBack then
                InstructionArrow.ValleyFold(startPoint, endPoint, InstructionColor.Blue, dir)
            else
                InstructionArrow.Create(startPoint, endPoint,
                    ArrowType.ValleyFold, ArrowType.ValleyFold, InstructionColor.Green, dir)
        let tryCreateReflectArrow point center =
            let reflected = Point.reflectBy chosen point
            if point =~ reflected then None
            else Some(createArrow point reflected center)
        let createPerpendicularArrows pass chosen hintPoint useHintPoint =
            option {
                if useHintPoint then yield! hintPoint
                let! (RawPoint(point)) = FoldOperation.getPerpendicularDynamicPoint pass chosen
                yield point
            }
            |> Option.bind (fun point ->
                let reflected = Point.reflectBy chosen point
                if point =~ reflected then None
                else
                    let pStart, pEnd =
                        match hintPoint with
                        | Some(hint) when Line.isPositiveSide point chosen <>
                                          Line.isPositiveSide hint chosen ->
                            reflected, point
                        | _ -> point, reflected
                    Some(createArrow pStart pEnd center))
            |> Option.toArray
        let createPoint point = { Point = point; Color = InstructionColor.Brown }
        let arrowsToPoint arrows =
            arrows |> Array.collect (fun ar ->
                [| createPoint ar.Line.Point1; createPoint ar.Line.Point2 |])
        let swapByDir dir point linePoint =
            match dir with
            | FoldDirection.LineToPoint -> linePoint, point
            | _ -> point, linePoint
        match method with
        | NoOperation -> array.Empty(), array.Empty()
        | Axiom1(hint, _, _) ->
            hint
            |> Option.bind (fun point -> tryCreateReflectArrow point.Point center)
            |> Option.toArray,
            array.Empty()
        | Axiom2(RawPoint(point1), RawPoint(point2)) ->
            [| createArrow point1 point2 center |], array.Empty()
        | Axiom2P(HintPoint(point2), RawPoint(point1), dir) ->
            let pStart, pEnd = swapByDir dir point1 point2
            [| createArrow pStart pEnd center |], array.Empty()
        | Axiom3(OprLine(line1, point, _, _), RawLine(line2)) ->
            let center = Line.cross line1.Line line2 |> Option.defaultValue center
            tryCreateReflectArrow point center |> Option.toArray, array.Empty()
        | Axiom3P(_, RawPoint(point), dir) ->
            let reflected = Point.reflectBy chosen point
            let pStart, pEnd = swapByDir dir point reflected
            [| createArrow pStart pEnd center |], array.Empty()
        | Axiom4(hint, pass, _) ->
            let hint = Option.map FoldOperation.(|RawPoint|) hint
            let arrows = createPerpendicularArrows pass chosen hint true
            arrows,
            if pass.IsEdge then array.Empty() else arrowsToPoint arrows
        | Axiom5(RawPoint(pass), _, RawPoint(point), dir) ->
            let reflected = Point.reflectBy chosen point
            let pStart, pEnd = swapByDir dir point reflected
            [| createArrow pStart pEnd pass |],
            [| createPoint reflected |]
        | Axiom5M(passCandidates, _, RawPoint(point), dir) ->
            let reflected = Point.reflectBy chosen point
            let pStart, pEnd = swapByDir dir point reflected
            match List.tryFind (fun (RawPoint p) -> Line.contains p chosen) passCandidates with
            | Some(RawPoint(pass)) ->
                [| createArrow pStart pEnd pass |],
                [| createPoint reflected; createPoint pass |]
            | None ->
                [| createArrow pStart pEnd center |],
                [| createPoint reflected |]
        | Axiom6(_, RawPoint(point1), _, RawPoint(point2), dir) ->
            let reflected1 = Point.reflectBy chosen point1
            let reflected2 = Point.reflectBy chosen point2
            let pStart2, pEnd2 = swapByDir dir point2 reflected2
            let pStart1, pEnd1 =
                if Line.distSign pStart2 chosen = Line.distSign point1 chosen
                then point1, reflected1
                else reflected1, point1
            [| createArrow pStart1 pEnd1 center
               createArrow pStart2 pEnd2 center |],
            [| createPoint reflected1
               createPoint reflected2 |]
        | Axiom7(pass, _, RawPoint(point), dir) ->
            let reflected = Point.reflectBy chosen point
            let pStart, pEnd = swapByDir dir point reflected
            let perpendicularArrows = createPerpendicularArrows pass chosen (Some(pStart)) false
            Array.append perpendicularArrows [| createArrow pStart pEnd center |],
            if pass.IsEdge
            then [| createPoint reflected |]
            else Array.append (arrowsToPoint perpendicularArrows) [| createPoint reflected |]

    let getLineAndArrow paper (opr: FoldOperation) previewOnly forDynamic =
        let lines = FoldOperation.getLines opr.Method
        let chosen = if previewOnly then None else FoldOperation.chooseLine lines opr.Method
        let getLines = if forDynamic then getLines else getChosenLine
        match chosen with
        | Some(c) ->
            let targetLayers =
                if opr.IsFrontOnly
                then FoldBack.getTargetLayers paper c opr.Method :> seq<_>
                else paper.Layers :> seq<_>
            let arrows, points = getArrow c opr.Method (opr.CreaseType = CreaseType.ValleyFold)
            let points =
                if forDynamic then
                    points
                else
                    [| for p in getInstructionPoint opr.Method -> { Point = p; Color = InstructionColor.Brown } |]
                    |> Array.append points
            getLines targetLayers lines chosen, arrows, points
        | None ->
            getLines paper.Layers lines chosen, array.Empty(), array.Empty()


type internal InstructionWrapper(paper: IPaper) =
    let instruction = FoldingInstruction()
    member _.Instruction = instruction

    member _.ResetAll() =
        instruction.Lines .<- array.Empty()
        instruction.Arrows .<- array.Empty()
        instruction.Points .<- array.Empty()

    member _.Set(opr, previewOnly) =
        let lines, arrows, points = Instruction.getLineAndArrow paper opr previewOnly true
        instruction.Lines .<- lines
        instruction.Arrows .<- arrows
        instruction.Points .<- points

    member _.SetLines(lines) =
        instruction.Lines .<- lines
