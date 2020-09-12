namespace Orimath.Measure
open System
open System.Reflection
open Orimath.Core
open Orimath.FoldingInstruction
open Orimath.Plugins

type MeasureTool(workspace: IWorkspace) =
    let paper = workspace.Paper
    let instruction = FoldingInstruction()

    let (|FreePoint|_|) (dt: OperationTarget) =
        match dt.Target with
        | DisplayTarget.Point(point) -> Some(point)
        | DisplayTarget.Layer(_) -> Some(dt.Point)
        | _ -> None
    let (|LineOrEdge|_|) (dt: OperationTarget) =
        match dt.Target with
        | DisplayTarget.Line(line) -> Some(line.Line, dt.Point)
        | DisplayTarget.Edge(edge) -> Some(edge.Line.Line, dt.Point)
        | _ -> None

    member __.GetDistanceLine(source: OperationTarget, target: OperationTarget) =
        match source, target with
        | FreePoint(p1), FreePoint(p2) -> LineSegment.FromPoints(p1, p2)
        | FreePoint(p1), LineOrEdge(l1, _) 
        | LineOrEdge(l1, _), FreePoint(p1) -> LineSegment.FromPoints(p1, l1.GetPerpendicularFoot(p1))
        | LineOrEdge(l1, p1), LineOrEdge(l2, _) ->
            match l1.GetCrossPoint(l2) with
            | Some(crossPoint) ->
                let cosTheta = abs (l1.XFactor * l2.XFactor + l1.YFactor * l2.YFactor)
                let sinTheta = -sqrt(1.0 - cosTheta * cosTheta)
                Some(LineSegment.FromFactorsAndPoint(sinTheta, cosTheta, crossPoint))
            | None ->
                match Line.FromFactorsAndPoint(l1.YFactor, -l1.XFactor, p1).GetCrossPoint(l2) with
                | Some(p2) -> LineSegment.FromPoints(p1, p2)
                | None -> None
        | _ -> None
    
    member __.ClearSelection() =
        paper.SelectedLayers <- array.Empty()
        paper.SelectedPoints <- array.Empty()
        paper.SelectedLines <- array.Empty()
        paper.SelectedEdges <- array.Empty()

    interface ITool with
        member __.Name = "計測"
        member __.ShortcutKey = "Ctrl+M"
        member __.Icon = Assembly.GetExecutingAssembly().GetManifestResourceStream("Orimath.Measure.Icon.png")
        member __.OnActivated() = ()
        member __.OnDeactivated() = ()
        member this.OnClick(_, _) = this.ClearSelection()
        member __.BeginDrag(source, _) =
            match source with
            | FreePoint _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragEnter(source, target, _) =
            match this.GetDistanceLine(source, target) with
            | Some(l) -> instruction.Lines <- [| { Line = l; Color = InstructionColor.Gray } |]
            | None -> ()
            
            match target with
            | FreePoint _ | LineOrEdge _ -> true
            | _ -> false

        member this.DragOver(source, target, _) =
            match this.GetDistanceLine(source, target) with
            | Some(l) -> instruction.Lines <- [| { Line = l; Color = InstructionColor.Gray } |]
            | None -> ()

            match target with
            | FreePoint _ | LineOrEdge _ -> true
            | _ -> false

        member __.DragLeave(_, target, _) =
            instruction.Lines <- Array.Empty()
            match target with
            | FreePoint _ | LineOrEdge _ -> true
            | _ -> false

        member this.Drop(source, target, _) =
            paper.SelectedLayers <- array.Empty()
            paper.SelectedPoints <- array.Empty()
            paper.SelectedEdges <- array.Empty()
            paper.SelectedLines <- Option.toArray(this.GetDistanceLine(source, target))
            instruction.Lines <- Array.Empty()

    interface IFoldingInstructionTool with
        member __.FoldingInstruction = instruction
