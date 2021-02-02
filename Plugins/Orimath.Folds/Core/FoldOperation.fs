namespace Orimath.Folds.Core
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.Plugins

[<Struct>]
type FoldDirection = PointToLine | LineToPoint

type FoldOperation =
    | NoOperation
    | Axiom1 of Point * Point
    | Axiom2 of Point * Point
    | Axiom3 of (LineSegment * Point) * (LineSegment * Point)
    | Axiom4 of LineSegment * Point * isEdge: bool
    | Axiom5 of pass: Point * (LineSegment * Point) * Point * direction: FoldDirection
    | Axiom6 of LineSegment * Point * (LineSegment * Point) * Point * direction: FoldDirection
    | Axiom7 of pass: LineSegment * (LineSegment * Point) * Point * isPassEdge: bool * direction: FoldDirection
    | AxiomP of (LineSegment * Point) * Point * direction: FoldDirection

[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module internal FoldOperation =
    let (|FreePoint|_|) (free: bool) (dt: OperationTarget) =
        match dt.Target with
        | DisplayTarget.Point(point) -> Some(point)
        | DisplayTarget.Layer(_) when free -> Some(dt.Point)
        | _ -> None

    let (|LineOrEdge|_|) (dt: OperationTarget) =
        match dt.Target with
        | DisplayTarget.Line(line) -> Some(line, dt.Point)
        | DisplayTarget.Edge(edge) -> Some(edge.Line, dt.Point)
        | _ -> None

    let (|LineOrEdge2|_|) (dt: OperationTarget) =
        match dt.Target with
        | DisplayTarget.Line(line) -> Some(line, false)
        | DisplayTarget.Edge(edge) -> Some(edge.Line, true)
        | _ -> None

    let getPass (paper: IPaperModel) =
        let passPoint = Array.tryItem 0 paper.SelectedPoints.Value
        let passLine =
            if paper.SelectedEdges.Value.Length > 0
            then Some(paper.SelectedEdges.Value.[0].Line, true)
            elif paper.SelectedLines.Value.Length > 0
            then Some(paper.SelectedLines.Value.[0], false)
            else None
        passPoint, passLine

    let getOperation (paper: IPaperModel) (source: OperationTarget) (target: OperationTarget) (modifier: OperationModifier) =
        let free = modifier.HasFlag(OperationModifier.Alt)
        if modifier.HasFlag(OperationModifier.RightButton) then
            match source, target with
            | FreePoint free (point1), FreePoint free (point2) -> Axiom1(point1, point2)
            | FreePoint free (point), LineOrEdge2(line, isEdge)
            | LineOrEdge2(line, isEdge), FreePoint free (point) -> Axiom4(line, point, isEdge)
            | _ -> NoOperation
        else
            let lpOpr direction line point =
                match getPass paper with
                | Some(pass), None -> Axiom5(pass, line, point, direction)
                | None, Some(pass, isEdge) -> Axiom7(pass, line, point, isEdge, direction)
                | Some(p), Some(l, _) -> Axiom6(l, p, line, point, direction)
                | _ -> AxiomP(line, point, direction)

            match source, target with
            | FreePoint free (point1), FreePoint free (point2) -> Axiom2(point1, point2)
            | LineOrEdge(line1), LineOrEdge(line2) -> Axiom3(line1, line2)
            | FreePoint free (point), LineOrEdge(line) -> lpOpr PointToLine line point
            | LineOrEdge(line), FreePoint free (point) -> lpOpr LineToPoint line point
            | _ -> NoOperation

    let getLines opr =
        match opr with
        | NoOperation -> []
        | Axiom1(point1, point2) -> Fold.axiom1 point1 point2 |> Option.toList
        | Axiom2(point1, point2) -> Fold.axiom2 point1 point2 |> Option.toList
        | Axiom3((line1, _), (line2, _)) -> Fold.axiom3 line1.Line line2.Line
        | Axiom4(line, point, _) -> [Fold.axiom4 line.Line point]
        | Axiom5(pass, (line, _), point, _) -> Fold.axiom5 pass line.Line point
        | Axiom6(line1, point1, (line2, _), point2, _) -> Fold.axiom6 line1.Line point1 line2.Line point2
        | Axiom7(pass, (line, _), point, _, _) -> Fold.axiom7 pass.Line line.Line point |> Option.toList
        | AxiomP((line, _), point, _) -> Fold.axiomP line.Line point |> Option.toList

    let chooseLine (lines: Line list) (opr: FoldOperation) =
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
            | Axiom5(_, (_, point1), point2, _)
            | Axiom6(_, _, (_, point1), point2, _) ->
                Some(lines |> List.minBy(fun line -> point1.GetDistance(line.Reflect(point2))))
            | _ -> Some(lines.[0])

    let getSourcePoint opr =
        match opr with
        | NoOperation
        | Axiom1 _
        | Axiom4 _ -> None
        | Axiom2(p, _)
        | Axiom3((_, p), _) -> Some(p)
        | Axiom5(_, (_, lp), p, dir)
        | Axiom6(_, _, (_, lp), p, dir)
        | Axiom7(_, (_, lp), p, _, dir)
        | AxiomP((_, lp), p, dir) ->
            match dir with
            | LineToPoint -> Some(lp)
            | PointToLine -> Some(p)
