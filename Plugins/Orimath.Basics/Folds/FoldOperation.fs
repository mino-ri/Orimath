[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal Orimath.Basics.Folds.FoldOperation
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.Basics.Internal
open Orimath.Plugins

let (|RawPoint|) p = match p with OprPoint(p, _) -> p

let (|RawLine|) line = match line with OprLine(seg, _, _, _) -> seg.Line

let (|RawSegment|) line = match line with OprLine(seg, _, _, _) -> seg

let (|HintOprPoint|) line = match line with OprLine(_, hintPoint, _, layerIndex) -> OprPoint(hintPoint, layerIndex)

let (|HintPoint|) line = match line with OprLine(_, hintPoint, _, _) -> hintPoint

let (|FreePoint|_|) isFree (dt: OperationTarget) =
    match dt.Target with
    | DisplayTarget.Point(point) -> Some(OprPoint(point, dt.Layer.Index))
    | DisplayTarget.Layer(_) when isFree -> Some(OprPoint(dt.Point, dt.Layer.Index))
    | _ -> None

let (|LineOrEdge|_|) (dt: OperationTarget) =
    match dt.Target with
    | DisplayTarget.Crease(crease) -> Some(OprLine(crease.Segment, dt.Point, false, dt.Layer.Index))
    | DisplayTarget.Edge(edge) -> Some(OprLine(edge.Segment, dt.Point, true, dt.Layer.Index))
    | _ -> None

let getGeneralDynamicPoint (paper: IPaperModel) (foldLine: Line) =
    option {
        let! first, last = Paper.clipBoundBy paper foldLine
        let middle = (first + last) / 2.0
        let! point1, point2 = Paper.clipBoundBy paper (Fold.axiom4 foldLine middle)
        let point = if Point.dist middle point1 <= Point.dist middle point2 then point1 else point2
        if not (Line.contains point foldLine) then
            let! layer = Seq.tryFind (Layer.containsPoint point) paper.Layers
            return OprPoint(point, layer.Index)
    }

let getPerpendicularDynamicPoint (OprLine(line, _, isEdge, layerIndex)) (foldLine: Line) =
    option {
        if not isEdge then
            let p1 = Line.signedDist line.Point1 foldLine
            let p2 = Line.signedDist line.Point2 foldLine
            if sign p1 <> sign p2 then
                let p = if abs p1 < abs p2 then line.Point1 else line.Point2
                let reflected = Point.reflectBy foldLine p
                if p <>~ reflected then
                    return OprPoint(p, layerIndex)
    }

let getPass (paper: IPaperModel) =
    let passPoint = Array.tryItem 0 paper.SelectedPoints.Value
    let passLine =
        if paper.SelectedEdges.Value.Length > 0
        then Some(paper.SelectedEdges.Value[0].Segment, true)
        elif paper.SelectedCreases.Value.Length > 0
        then Some(paper.SelectedCreases.Value[0].Segment, false)
        else None
    passPoint, passLine

let getFoldMethod paper selectedPoint selectedLine source target passFold free =
    if passFold then
        match source, target with
        | FreePoint free (point1), FreePoint free (point2) ->
            let hintPoint = option {
                yield! selectedPoint
                let! foldLine = Fold.axiom1 point1.Point point2.Point
                yield! getGeneralDynamicPoint paper foldLine
            }
            Axiom1(hintPoint, point1, point2)
        | FreePoint free (point), LineOrEdge(line)
        | LineOrEdge(line), FreePoint free (point) ->
            let hintPoint = option {
                yield! selectedPoint
                let foldLine = Fold.axiom4 line.Line point.Point
                yield! getPerpendicularDynamicPoint line foldLine
                yield! getGeneralDynamicPoint paper foldLine
            }
            Axiom4(hintPoint, line, point)
        | _ -> NoOperation
    else
        let lpOpr direction line point =
            match selectedPoint, selectedLine with
            | Some(pass), None -> Axiom5(pass, line, point, direction)
            | None, Some(pass) -> Axiom7(pass, line, point, direction)
            | Some(point1), Some(line1) -> Axiom6(line1, point1, line, point, direction)
            | _ ->
                if free then
                    Axiom2P(line, point, direction)
                elif direction = FoldDirection.PointToLine then
                    let passCandidates = [
                        for p in paper.Layers[point.LayerIndex].Points do
                            if p <> point.Point then
                                OprPoint(p, point.LayerIndex)
                    ]
                    Axiom5M(passCandidates, line, point, direction)
                else
                    Axiom3P(line, point, direction)
        match source, target with
        | FreePoint free (point1), FreePoint free (point2) -> Axiom2(point1, point2)
        | LineOrEdge(line1), LineOrEdge(line2) -> Axiom3(line1, line2)
        | FreePoint free (point), LineOrEdge(line) -> lpOpr FoldDirection.PointToLine line point
        | LineOrEdge(line), FreePoint free (point) -> lpOpr FoldDirection.LineToPoint line point
        | _ -> NoOperation

let private getFoldOperationCore paper selectedPoint selectedLine source target passFold free creaseType isFrontOnly =
    { Method = getFoldMethod paper selectedPoint selectedLine source target passFold free
      CreaseType = creaseType
      IsFrontOnly = isFrontOnly }

let getFoldOperation paper selectedPoint selectedLine source target (modifier: OperationModifier) isDraft =
    let creaseType =
        if isDraft then CreaseType.Draft
        elif modifier.HasShift then CreaseType.ValleyFold
        else CreaseType.Crease
    getFoldOperationCore
        paper selectedPoint selectedLine source target
        modifier.HasRightButton modifier.HasAlt creaseType modifier.HasCtrl

let getPreviewFoldOperation paper selectedPoint selectedLine source target (modifier: OperationModifier) isDraft =
    match getFoldOperation paper selectedPoint selectedLine source target modifier isDraft with
    | { Method = NoOperation } when not modifier.HasAlt ->
        let modifier = modifier ||| OperationModifier.Alt
        getFoldOperation paper selectedPoint selectedLine source target modifier isDraft, true
    | opr -> opr, false

let getLines opr =
    match opr with
    | NoOperation -> []
    | Axiom1(_, RawPoint(point1), RawPoint(point2)) -> Fold.axiom1 point1 point2 |> Option.toList
    | Axiom2(RawPoint(point1), RawPoint(point2)) -> Fold.axiom2 point1 point2 |> Option.toList
    | Axiom3(RawLine(line1), RawLine(line2)) -> Fold.axiom3 line1 line2
    | Axiom4(_, RawLine(line), RawPoint(point)) -> [Fold.axiom4 line point]
    | Axiom5(RawPoint(pass), RawLine(line), RawPoint(point), _) -> Fold.axiom5 pass line point
    | Axiom6(RawLine(line1), RawPoint(point1), RawLine(line2), RawPoint(point2), _) ->
        Fold.axiom6 line1 point1 line2 point2
    | Axiom7(RawLine(pass), RawLine(line), RawPoint(point), _) ->
        Fold.axiom7 pass line point |> Option.toList
    | Axiom2P(HintPoint(point1), RawPoint(point2), _) -> Fold.axiom2 point1 point2 |> Option.toList
    | Axiom3P(RawLine(line), RawPoint(point), _) -> Fold.axiomP line point |> Option.toList
    | Axiom5M(passCandidates, line, RawPoint(point), _) ->
        [
            for RawPoint(pass) in passCandidates do
            for l in Fold.axiom5 pass line.Line point do
                if LineSegment.containsPoint (Point.reflectBy l point) line.Segment then
                    yield l
        ]

let chooseLine (lines: Line list) opr =
    match lines with
    | [] -> None
    | [l] -> Some(l)
    | _ ->
        match opr with
        | Axiom3(HintPoint(point1), HintPoint(point2)) ->
            match Fold.axiom1 point1 point2 with
            | None -> Some(lines[0])
            | Some(opLine) ->
                match Line.cross opLine lines[0] with
                | None -> Some(lines[0])
                | Some(cross) ->
                    let x1, x2 = if point1.X <= point2.X then point1.X, point2.X else point2.X, point1.X
                    let y1, y2 = if point1.Y <= point2.Y then point1.Y, point2.Y else point2.Y, point1.Y
                    if x1 <=~ cross.X && cross.X <=~ x2 && y1 <=~ cross.Y && cross.Y <=~ y2
                    then Some(lines[0])
                    else Some(lines[1])
        | Axiom5(_, HintPoint(point1), RawPoint(point2), _)
        | Axiom6(_, _, HintPoint(point1), RawPoint(point2), _)
        | Axiom5M(_, HintPoint(point1), RawPoint(point2), _) ->
            Some(lines |> List.minBy (fun line -> Point.dist point1 (Point.reflectBy line point2)))
        | _ -> Some(lines[0])

let getSourcePoint method =
    match method with
    | NoOperation -> []
    | Axiom1(hint, _, _)
    | Axiom4(hint, _, _) -> hint |> Option.toList
    | Axiom2(p1, p2)
    | Axiom3(HintOprPoint(p1), HintOprPoint(p2)) -> [ p1; p2 ]
    | Axiom5(_, HintOprPoint(lp), p, dir)
    | Axiom6(_, _, HintOprPoint(lp), p, dir)
    | Axiom7(_, HintOprPoint(lp), p, dir)
    | Axiom2P(HintOprPoint(lp), p, dir)
    | Axiom3P(HintOprPoint(lp), p, dir)
    | Axiom5M(_, HintOprPoint(lp), p, dir) ->
        match dir with
        | FoldDirection.LineToPoint -> [ lp; p ]
        | _ -> [ p; lp ]

let getInstructionPoint method =
    match method with
    | Axiom3 _
    | Axiom3P _
    | NoOperation -> []
    | Axiom1(_, RawPoint(p1), RawPoint(p2))
    | Axiom2(RawPoint(p1), RawPoint(p2))
    | Axiom5(RawPoint(p1), _, RawPoint(p2), _)
    | Axiom6(_, RawPoint(p1), _, RawPoint(p2), _) -> [ p1; p2 ]
    | Axiom4(_, _, RawPoint(p))
    | Axiom7(_, _, RawPoint(p), _)
    | Axiom5M(_, _, RawPoint(p), _)
    | Axiom2P(_, RawPoint(p), _) -> [ p ]

let countContains layer method =
    let count s = s |> Seq.sumBy (fun b -> if b then 1 else 0)
    match method with
    | NoOperation -> 0
    | Axiom1(_, RawPoint(point1), RawPoint(point2))
    | Axiom2(RawPoint(point1), RawPoint(point2)) ->
        count [
            Layer.containsPoint point1 layer
            Layer.containsPoint point2 layer
        ]
    | Axiom3(RawSegment(seg1), RawSegment(seg2)) ->
        count [
            Layer.clip seg1.Line layer |> Seq.isEmpty |> not
            Layer.clip seg2.Line layer |> Seq.isEmpty |> not
        ]
    | Axiom4(_, RawSegment(seg), RawPoint(point))
    | Axiom5(_, RawSegment(seg), RawPoint(point), _)
    | Axiom6(_, _, RawSegment(seg), RawPoint(point), _)
    | Axiom7(_, RawSegment(seg), RawPoint(point), _)
    | Axiom2P(RawSegment(seg), RawPoint(point), _)
    | Axiom3P(RawSegment(seg), RawPoint(point), _)
    | Axiom5M(_, RawSegment(seg), RawPoint(point), _) ->
        count [
            Layer.containsPoint point layer
            Layer.clip seg.Line layer |> Seq.isEmpty |> not
        ]
