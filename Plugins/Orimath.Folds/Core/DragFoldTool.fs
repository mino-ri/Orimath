namespace Orimath.Folds.Core
open Orimath.Core
open Orimath.Plugins

type FoldOperation =
    | NoOperation
    | Axiom1 of Point * Point
    | Axiom2 of Point * Point
    | Axiom3 of Line * Line
    | Axiom4 of Line * Point
    | Axiom5 of pass: Point * Line * Point
    | Axiom6 of Line * Point * Line * Point
    | Axiom7 of pass: Line * Line * Point

type DragFoldTool(workspace: IWorkspace) =
    let paper = workspace.Paper
    let (|LineOrEdge|_|) dt =
        match dt with
        | DisplayTarget.Line(line) -> Some(line.Line)
        | DisplayTarget.Edge(edge) -> Some(edge.Line.Line)
        | _ -> None

    member __.GetOperation(source: OperationTarget, target: OperationTarget, modifier: OperationModifier) =
        if modifier.HasFlag(OperationModifier.RightButton) then
            match source.Target, target.Target with
            | DisplayTarget.Point(point1), DisplayTarget.Point(point2) -> Axiom1(point1, point2)
            | DisplayTarget.Point(point), LineOrEdge(line)
            | LineOrEdge(line), DisplayTarget.Point(point) -> Axiom4(line, point)
            | _ -> NoOperation
        else
            match source.Target, target.Target with
            | DisplayTarget.Point(point1), DisplayTarget.Point(point2) -> Axiom2(point1, point2)
            | LineOrEdge(line1), LineOrEdge(line2) -> Axiom3(line1, line2)
            | DisplayTarget.Point(point), LineOrEdge(line)
            | LineOrEdge(line), DisplayTarget.Point(point) ->
                let passPoint = Array.tryItem 0 paper.SelectedPoints
                let passLine =
                    if paper.SelectedEdges.Length > 0
                    then Some(paper.SelectedEdges.[0].Line)
                    else Array.tryItem 0 paper.SelectedLines
                match passPoint, passLine with
                | Some(pass), None -> Axiom5(pass, line, point)
                | None, Some(pass) -> Axiom7(pass.Line, line, point)
                | Some(p), Some(l) -> Axiom6(l.Line, p, line, point)
                | _ -> NoOperation
            | _ -> NoOperation
        
    interface ITool with
        member __.Name = "折り線"
        member __.ShortcutKey = ""
        member __.Icon = null
        member __.OnClick(target, modifier) =
            if not (modifier.HasFlag(OperationModifier.RightButton)) then
                match target.Target with
                | DisplayTarget.Point(point) ->
                    paper.SelectedPoints <-
                        if paper.IsSelected(point) then array.Empty() else [| point |]
                    paper.SelectedLines <- array.Empty()
                    paper.SelectedEdges <- array.Empty()
                | DisplayTarget.Line(line) ->
                    paper.SelectedPoints <- array.Empty()
                    paper.SelectedLines <-
                        if paper.IsSelected(line) then array.Empty() else [| line |]
                    paper.SelectedEdges <- array.Empty()
                | DisplayTarget.Edge(edge) ->
                    paper.SelectedPoints <- array.Empty()
                    paper.SelectedLines <- array.Empty()
                    paper.SelectedEdges <-
                        if paper.IsSelected(edge) then array.Empty() else [| edge |]
                | _ ->
                    paper.SelectedPoints <- array.Empty()
                    paper.SelectedLines <- array.Empty()
                    paper.SelectedEdges <- array.Empty()

        member __.BeginDrag(source, modifier) =
            match source.Target with
            | DisplayTarget.Point _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragEnter(source, target, modifier) =
            match target.Target with
            | DisplayTarget.Point _ | LineOrEdge _ -> true
            | _ -> false
        member this.DragLeave(source, target, modifier) =
            match target.Target with
            | DisplayTarget.Point _ | LineOrEdge _ -> true
            | _ -> false
        member this.DragOver(source, target, modifier) =
            match target.Target with
            | DisplayTarget.Point _ | LineOrEdge _ -> true
            | _ -> false

        member this.Drop(source, target, modifier) =
            let lines =
                match this.GetOperation(source, target, modifier) with
                | NoOperation -> []
                | Axiom1(point1, point2) -> Fold.axiom1 point1 point2 |> Option.toList
                | Axiom2(point1, point2) -> Fold.axiom2 point1 point2 |> Option.toList
                | Axiom3(line1, line2) -> Fold.axiom3 line1 line2
                | Axiom4(line, point) -> [Fold.axiom4 line point]
                | Axiom5(pass, line, point) -> Fold.axiom5 pass line point
                | Axiom6(line1, point1, line2, point2) -> Fold.axiom6 line1 point1 line2 point2
                | Axiom7(pass, line, point) -> Fold.axiom7 pass line point |> Option.toList
            for layer in paper.Layers do layer.AddLines(lines)
