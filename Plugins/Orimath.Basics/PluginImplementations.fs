namespace Orimath.Basics
open Orimath.Plugins

[<OrimathPlugin("基本機能", "アンドゥ・リドゥなど基本的な操作を含みます。")>]
type BasicPlugin() =
    interface IPlugin with
        member __.Execute(args) =
            args.Workspace.AddEffect(UndoEffect(args.Workspace))
            args.Workspace.AddEffect(RedoEffect(args.Workspace))

[<OrimathPlugin("紙の新規作成", "「新しい紙」「リセット」コマンドを含みます。")>]
type NewPaperPlugin() =
    interface IPlugin with
        member __.Execute(args) =
            let newPaperExecutor = NewPaperExecutor(args.Workspace)
            args.Workspace.AddEffect(newPaperExecutor.NewPaperEffect)
            args.Workspace.AddEffect(NewPaperEffect(newPaperExecutor))
