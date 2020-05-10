namespace Orimath.Basics
open System
open Orimath.Plugins

type UndoEffect(workspace: IWorkspace) =
    let canExecuteChanged = Event<EventHandler, EventArgs>()
    interface IEffect with
        member __.Name = "元に戻す"
        member __.ShortcutKey = "Ctrl+Z"
        member __.Icon = null
        member __.CanExecute() = workspace.Paper.CanUndo
        member __.Execute() = workspace.Paper.Undo()
        [<CLIEvent>]
        member __.CanExecuteChanged = canExecuteChanged.Publish

type RedoEffect(workspace: IWorkspace) =
    let canExecuteChanged = Event<EventHandler, EventArgs>()
    interface IEffect with
        member __.Name = "やり直し"
        member __.ShortcutKey = "Ctrl+Y"
        member __.Icon = null
        member __.CanExecute() = workspace.Paper.CanRedo
        member __.Execute() = workspace.Paper.Redo()
        [<CLIEvent>]
        member __.CanExecuteChanged = canExecuteChanged.Publish
