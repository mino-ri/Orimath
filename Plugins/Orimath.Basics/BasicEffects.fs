namespace Orimath.Basics
open Orimath.Core
open Orimath.Plugins
open Orimath.Core.NearlyEquatable
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type UndoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.Undo}Undo"
        member _.ShortcutKey = "Ctrl+Z"
        member _.Icon = Internal.getIcon "undo"
        member _.CanExecute = workspace.Paper.CanUndo
        member _.Execute() = workspace.Paper.Undo()

type RedoEffect(workspace: IWorkspace) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.Redo}Redo"
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
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.TurnVertically}Turn vertically"
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
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.TurnHorizontally}Turn horizontally"
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
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.RotateRight}Rotate 90° right"
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
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.RotateLeft}Rotate 90° left"
        member _.ShortcutKey = "Ctrl+Left"
        member _.Icon = Internal.getIcon "rotate_l"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Internal.transform workspace matrix false


type OpenAllEffect(workspace: IWorkspace) =
    let canExecute = workspace.Paper.Layers.CountProp .>= 2
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.Edit}Edit" |]
        member _.Name = "{basic/Effect.UnfoldAll}Unfold all"
        member _.ShortcutKey = "Ctrl+E"
        member _.Icon = Internal.getIcon "open_all"
        member _.CanExecute = canExecute
        member _.Execute() =
            use __ = workspace.Paper.BeginChange()
            let layers = workspace.Paper.Layers |> Seq.toArray
            workspace.ClearPaper([Layer.merge layers])
            let lines = 
                seq {
                    for layer in layers do
                    let inv = layer.Matrix.Invert()
                    yield! seq { for e in layer.Edges -> e.Segment * inv }
                    yield! seq { for x in layer.Creases -> x.Segment * inv }
                }
                |> LineSegment.merge
            workspace.Paper.Layers.[0].AddCreases(lines)
