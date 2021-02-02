namespace Orimath.Basics
open System.ComponentModel
open Orimath.Plugins

[<DisplayName("コマンド: 基本機能"); Description("アンドゥ・リドゥなど基本的な操作を含みます。")>]
type BasicPlugin() =
    interface IPlugin with
        member _.Execute(args) =
            args.Workspace.AddEffect(UndoEffect(args.Workspace))
            args.Workspace.AddEffect(RedoEffect(args.Workspace))
            args.Workspace.AddEffect(RotateLeftEffect(args.Workspace))
            args.Workspace.AddEffect(RotateRightEffect(args.Workspace))
            args.Workspace.AddEffect(TurnVerticallyEffect(args.Workspace))
            args.Workspace.AddEffect(TurnHorizontallyEffect(args.Workspace))
            args.Workspace.AddEffect(OpenAllEffect(args.Workspace))

[<DisplayName("ツール: 折り線"); Description("ドラッグ操作で折線をつけるツール。")>]
type FoldsPlugin() =
    interface IPlugin with
        member _.Execute(args) = args.Workspace.AddTool(Folds.DragFoldTool(args.Workspace))

[<DisplayName("ツール: 計測"); Description("ドラッグ操作で角度と距離を測るツール。")>]
type MeasurePlugin() =
    interface IPlugin with
        member _.Execute(args) = args.Workspace.AddTool(MeasureTool(args.Workspace))
