namespace Orimath.Basics
open Orimath.Plugins
open ApplicativeProperty

type OpenEffect(fileManager: IFileManager) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.File}File" |]
        member _.Name = "{basic/Effect.Open}Open..."
        member _.ShortcutKey = "Ctrl+O"
        member _.Icon = Internal.getIcon "open_file"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Async.RunSynchronously(fileManager.LoadPaper())


type SaveAsEffect(fileManager: IFileManager) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.File}File" |]
        member _.Name = "{basic/Effect.SaveAs}Save as..."
        member _.ShortcutKey = "Ctrl+Shift+S"
        member _.Icon = Internal.getIcon "save_file_as"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Async.RunSynchronously(fileManager.SavePaperAs())


type SaveEffect(fileManager: IFileManager) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.File}File" |]
        member _.Name = "{basic/Effect.Save}Save"
        member _.ShortcutKey = "Ctrl+S"
        member _.Icon = Internal.getIcon "save_file"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Async.RunSynchronously(fileManager.SavePaper())
