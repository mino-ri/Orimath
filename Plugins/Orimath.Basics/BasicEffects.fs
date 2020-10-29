namespace Orimath.Basics
open Orimath.Core
open Orimath.Plugins
open Orimath.Core.NearlyEquatable

type UndoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "元に戻す"
        member _.ShortcutKey = "Ctrl+Z"
        member _.Icon = InternalModule.getIcon "undo"
        member _.CanExecute() = workspace.Paper.CanUndo
        member _.Execute() = workspace.Paper.Undo()
        [<CLIEvent>]
        member _.CanExecuteChanged = workspace.Paper.CanUndoChanged

type RedoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "やり直し"
        member _.ShortcutKey = "Ctrl+Y"
        member _.Icon = InternalModule.getIcon "redo"
        member _.CanExecute() = workspace.Paper.CanRedo
        member _.Execute() = workspace.Paper.Redo()
        [<CLIEvent>]
        member _.CanExecuteChanged = workspace.Paper.CanUndoChanged

type OpenAllEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "すべて開く"
        member _.ShortcutKey = "Ctrl+E"
        member _.Icon = null
        member _.CanExecute() = workspace.Paper.Layers.Count >= 2
        member _.Execute() =
            use __ = workspace.Paper.BeginChange()
            let layers = workspace.Paper.Layers |> Seq.toArray
            let joinedLayer =
                let edges =
                    layers
                    |> Seq.collect(fun ly -> ly.OriginalEdges)
                    |> Seq.filter(fun e -> not e.Inner)
                    |> Seq.map(fun e -> e.Line)
                    |> LineSegmentExtensions.Merge
                    |> ResizeArray
                let points = ResizeArray()
                let mutable currentPoint = edges.[0].Point1
                while edges.Count > 0 do
                    let index = edges |> Seq.findIndex(fun e -> e.Point1 =~ currentPoint || e.Point2 =~ currentPoint)
                    let target = edges.[index]
                    currentPoint <- if target.Point1 =~ currentPoint then target.Point2 else target.Point1
                    points.Add(currentPoint)
                    edges.RemoveAt(index)
                workspace.CreateLayerFromPolygon(points, LayerType.BackSide)
            workspace.Paper.Clear(workspace.CreatePaper([joinedLayer]))
            let lines =
                layers
                |> Seq.collect(fun layer ->
                    let inv = layer.Matrix.Invert()
                    let inline invert x = x * inv
                    layer.Edges
                        |> Seq.filter(fun e -> e.Inner)
                        |> Seq.map(fun e -> e.Line * inv)
                        |> Seq.append (layer.Lines |> Seq.map invert))
                |> LineSegmentExtensions.Merge
            workspace.Paper.Layers.[0].AddLines(lines)
        [<CLIEvent>]
        member _.CanExecuteChanged = workspace.Paper.CanUndoChanged
