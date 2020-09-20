namespace Orimath.Basics
open Orimath.Plugins

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
