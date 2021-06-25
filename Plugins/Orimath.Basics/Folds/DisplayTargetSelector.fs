namespace Orimath.Basics.Folds
open Orimath.Plugins
open FoldOperation
open ApplicativeProperty.PropOperators


type DisplayTargetSelector(paper: IPaperModel) =
    let mutable selectedPointIndex = 0
    let mutable selectedLineIndex = 0

    member _.PointIndex = selectedPointIndex
    member _.LineIndex = selectedLineIndex

    member _.OnClick(target, modifier: OperationModifier) =
        let clearOther = not modifier.HasShift
        match target.Target with
        | DisplayTarget.Point(point) ->
            if paper.IsSelected(point) then
                paper.SelectedPoints .<- array.Empty()
            else
                paper.SelectedPoints .<- [| point |]
                selectedPointIndex <- target.Layer.Index
            if clearOther then
                paper.SelectedCreases .<- array.Empty()
                paper.SelectedEdges .<- array.Empty()
        | DisplayTarget.Crease(crease) ->
            if paper.IsSelected(crease) then
                paper.SelectedCreases .<- array.Empty()
            else
                paper.SelectedCreases .<- [| crease |]
                selectedLineIndex <- target.Layer.Index
            paper.SelectedEdges .<- array.Empty()
            if clearOther then
                paper.SelectedPoints .<- array.Empty()
        | DisplayTarget.Edge(edge) ->
            if paper.IsSelected(edge) then
                paper.SelectedEdges .<- array.Empty()
            else
                paper.SelectedEdges .<- [| edge |]
                selectedLineIndex <- target.Layer.Index
            paper.SelectedCreases .<- array.Empty()
            if clearOther then
                paper.SelectedPoints .<- array.Empty()
        | _ ->
            paper.SelectedPoints .<- array.Empty()
            paper.SelectedCreases .<- array.Empty()
            paper.SelectedEdges .<- array.Empty()
