namespace Orimath.Basics
open Orimath.Core
open Orimath.Plugins
open Orimath.Core.NearlyEquatable
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type UndoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "元に戻す"
        member _.ShortcutKey = "Ctrl+Z"
        member _.Icon = Internal.getIcon "undo"
        member _.CanExecute = workspace.Paper.CanUndo
        member _.Execute() = workspace.Paper.Undo()

type RedoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "やり直し"
        member _.ShortcutKey = "Ctrl+Y"
        member _.Icon = Internal.getIcon "redo"
        member _.CanExecute = workspace.Paper.CanRedo
        member _.Execute() = workspace.Paper.Redo()

type TurnVerticallyEffect(workspace: IWorkspace) =
    let matrix =
        { M11 = 1.0; M12 = 0.0
          M21 = 0.0; M22 = -1.0
          OffsetX = 0.0; OffsetY = 1.0 }
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "縦に裏返す"
        member _.ShortcutKey = "Ctrl+Up"
        member _.Icon = Internal.getIcon "turn_v"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Internal.transform workspace matrix true

type TurnHorizontallyEffect(workspace: IWorkspace) =
    let matrix =
        { M11 = -1.0; M12 = 0.0
          M21 = 0.0; M22 = 1.0
          OffsetX = 1.0; OffsetY = 0.0 }
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "横に裏返す"
        member _.ShortcutKey = "Ctrl+Down"
        member _.Icon = Internal.getIcon "turn_h"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Internal.transform workspace matrix true

type RotateRightEffect(workspace: IWorkspace) =
    let matrix =
        { M11 = 0.0; M12 = -1.0
          M21 = 1.0; M22 = 0.0
          OffsetX = 0.0; OffsetY = 1.0 }
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "右に90°回転"
        member _.ShortcutKey = "Ctrl+Right"
        member _.Icon = Internal.getIcon "rotate_r"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Internal.transform workspace matrix false

type RotateLeftEffect(workspace: IWorkspace) =
    let matrix =
        { M11 = 0.0; M12 = 1.0
          M21 = -1.0; M22 = 0.0
          OffsetX = 1.0; OffsetY = 0.0 }
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "左に90°回転"
        member _.ShortcutKey = "Ctrl+Left"
        member _.Icon = Internal.getIcon "rotate_l"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Internal.transform workspace matrix false


type OpenAllEffect(workspace: IWorkspace) =
    let canExecute = workspace.Paper.Layers.CountProp .>= 2
    interface IEffect with
        member val MenuHieralchy = [| "編集" |]
        member _.Name = "すべて開く"
        member _.ShortcutKey = "Ctrl+E"
        member _.Icon = Internal.getIcon "open_all"
        member _.CanExecute = canExecute
        member _.Execute() =
            use __ = workspace.Paper.BeginChange()
            let layers = workspace.Paper.Layers |> Seq.toArray
            let joinedLayer =
                let edges =
                    seq { for ly in layers do
                          for e in ly.OriginalEdges do
                          if not e.Inner then
                              yield e.Line }
                    |> LineSegment.merge
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
                seq { for layer in layers do
                      let inv = layer.Matrix.Invert()
                      yield! seq { for e in layer.Edges -> e.Line * inv }
                      yield! seq { for x in layer.Lines -> x * inv } }
                |> LineSegment.merge
            workspace.Paper.Layers.[0].AddLines(lines)
