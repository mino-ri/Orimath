namespace Orimath.Basics
open Orimath.Plugins
open ApplicativeProperty

type LoadEffect(fileManager: IFileManager) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.File}File" |]
        member _.Name = "{basic/Effect.LoadFile}Load file"
        member _.ShortcutKey = "Ctrl+O"
        member _.Icon = Internal.getIcon "open_file"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Async.RunSynchronously(fileManager.LoadPaper())


type SaveEffect(fileManager: IFileManager) =
    interface IEffect with
        member val MenuHieralchy = [| "{Menu.File}File" |]
        member _.Name = "{basic/Effect.SaveFile}Save file"
        member _.ShortcutKey = "Ctrl+S"
        member _.Icon = Internal.getIcon "save_file"
        member _.CanExecute = upcast Prop.ctrue
        member _.Execute() = Async.RunSynchronously(fileManager.SavePaper())
