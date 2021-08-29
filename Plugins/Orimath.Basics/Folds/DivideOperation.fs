[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Orimath.Basics.Folds.DivideOperation
open Orimath.Core
open Orimath.Core.NearlyEquatable
open Orimath.Plugins
open Orimath.Combination
open FoldOperation

let getFoldMethod source target free =
    let lpOpr direction line point =
        if free
        then Axiom2P(line, point, direction)
        else Axiom3P(line, point, direction)
    match source, target with
    | FreePoint free (point1), FreePoint free (point2) -> Axiom2(point1, point2)
    | LineOrEdge(line1), LineOrEdge(line2) -> Axiom3(line1, line2)
    | FreePoint free (point), LineOrEdge(line) -> lpOpr FoldDirection.PointToLine line point
    | LineOrEdge(line), FreePoint free (point) -> lpOpr FoldDirection.LineToPoint line point
    | _ -> NoOperation

let getDivideOperation source target (modifier: OperationModifier) isDraft divisionNumber =
    { Method = getFoldMethod source target modifier.HasAlt
      CreaseType = if isDraft then CreaseType.Draft else CreaseType.Crease
      DivisionNumber = divisionNumber }

let getPreviewDivideOperation source target (modifier: OperationModifier) isDraft divisionNumber =
    match getDivideOperation source target modifier isDraft divisionNumber with
    | { Method = NoOperation } when not modifier.HasAlt ->
        let modifier = modifier ||| OperationModifier.Alt
        getDivideOperation source target modifier isDraft divisionNumber, true
    | opr -> opr, false

let getLines (opr: DivideOperation) =
    match opr.Method with
    | Axiom2(RawPoint(point1), RawPoint(point2))
    | Axiom2P(HintPoint(point1), RawPoint(point2), _) ->
        match Line.FromPoints(point1, point2) with
        | Some(line) -> Fold.divide opr.DivisionNumber (Fold.axiom4 line point1) (Fold.axiom4 line point2)
        | None -> []
    | Axiom3(RawLine(line1), RawLine(line2)) ->
        Fold.divide opr.DivisionNumber line1 line2
    | Axiom3P(RawLine(line), RawPoint(point), _) ->
        Line.FromFactorsAndPoint(line.XFactor, line.YFactor, point)
        |> Fold.divide opr.DivisionNumber line
    | _ -> []

let chooseLine (opr: DivideOperation) (lines: Line list list) =
    match opr.Method with
    | Axiom2(RawPoint(point1), RawPoint(point2))
    | Axiom2P(HintPoint(point1), RawPoint(point2), _)
    | Axiom3(HintPoint(point1), HintPoint(point2))
    | Axiom3P(HintPoint(point1), RawPoint(point2), _) ->
        match lines with
        | [ chosen ] -> chosen
        | [ line1; line2 ] ->
            match Fold.axiom1 point1 point2 with
            | None -> line1
            | Some(opLine) ->
                match Line.cross opLine line1.[0] with
                | None -> line1
                | Some(cross) ->
                    let x1, x2 = if point1.X <= point2.X then point1.X, point2.X else point2.X, point1.X
                    let y1, y2 = if point1.Y <= point2.Y then point1.Y, point2.Y else point2.Y, point1.Y
                    if x1 <=~ cross.X && cross.X <=~ x2 && y1 <=~ cross.Y && cross.Y <=~ y2
                    then line1
                    else line2
        | _ -> []
    | _ -> []

let getInstructionLines paper (opr: DivideOperation) isPreviewOnly forDynamic =
    let lines = getLines opr
    if not forDynamic then
        let chosens = chooseLine opr lines
        [|
            for seg in chosens |> Seq.collect (Paper.clipBy paper) do
            { Line = seg; Color = InstructionColor.Blue }
        |]
    elif isPreviewOnly then
        [|
            for lines in lines do
            for seg in lines |> Seq.collect (Paper.clipBy paper) do
            { Line = seg; Color = InstructionColor.LightGray }
        |]
    else
        let chosens = chooseLine opr lines
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
