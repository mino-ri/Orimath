namespace Orimath.Basics.View
open System.ComponentModel
open Orimath.Plugins
open Orimath.Basics
open Orimath.Basics.View.ViewModels

[<DisplayName("コマンド: 紙の新規作成"); Description("「新しい紙」「リセット」コマンドを含みます。")>]
type NewPaperPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            let newPaperExecutor = NewPaperExecutor(args.Workspace)
            args.Workspace.AddEffect(newPaperExecutor.NewPaperEffect)
            args.Workspace.AddEffect(newPaperExecutor.ResetEffect)
            args.Messenger.SetEffectParameterViewModel<NewPaperExecutor>(fun p -> upcast NewPaperDialogViewModel(args.Messenger, args.Dispatcher, p))
            args.Messenger.RegisterView(ViewPane.Dialog, typeof<NewPaperDialogViewModel>, viewPath "NewPaperDialogControl")

[<DisplayName("ビュー: 折り紙"); Description("メイン描画領域。このプラグインを削除すると、折り紙本体が表示されなくなります。")>]
type BasicViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(WorkspaceViewModel(args.Workspace, args.PointConverter, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Main, typeof<WorkspaceViewModel>, viewPath "PaperControl")

[<DisplayName("ビュー: ツールバー"); Description("画面上部の各種機能が並んだツールバー。")>]
type EffectViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(EffectListViewModel(args.Workspace, args.Messenger))
            args.Messenger.RegisterView(ViewPane.Top, typeof<EffectListViewModel>, viewPath "EffectListControl")

[<DisplayName("ビュー: ツールボックス"); Description("画面左のツール切り替えボックス。")>]
type ToolViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(ToolListViewModel(args.Workspace, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Side, typeof<ToolListViewModel>, viewPath "ToolListControl")

[<DisplayName("ビュー: 計測"); Description("選択中の点・線の情報を表示します。")>]
type MeasureViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(MeasureViewModel(args.Workspace.Paper, args.PointConverter, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Side, typeof<MeasureViewModel>, viewPath "MeasureControl")

[<DisplayName("ビュー: 展開図"); Description("展開図を表示します。")>]
type CreasePatternViewPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(new CreasePatternViewModel(args.Workspace.Paper, args.Dispatcher))
            args.Messenger.RegisterView(ViewPane.Side, typeof<CreasePatternViewModel>, viewPath "CreasePatternControl")

[<DisplayName("ビュー: 折り図"); Description("メインビューの上に折り図を表示します。対応しているツールの操作時に折り図風の図が表示されます。")>]
type FoldingInstructionPlugin() =
    interface IViewPlugin with
        member _. Execute(args: ViewPluginArgs) =
            args.Messenger.AddViewModel(new FoldingInstructionViewModel(args.Workspace, args.Dispatcher, args.PointConverter))
            args.Messenger.RegisterView(ViewPane.Main, typeof<FoldingInstructionViewModel>, viewPath "FoldingInstructionControl")
