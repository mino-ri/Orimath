using System;
using System.ComponentModel;
using Orimath.Plugins;
using Orimath.Basics.View.ViewModels;

namespace Orimath.Basics.View
{
    [DisplayName("ビュー: 折り紙"), Description("メイン描画領域。このプラグインを削除すると、折り紙本体が表示されなくなります。")]
    public class BasicViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new WorkspaceViewModel(args.Workspace, args.PointConverter, args.Dispatcher));
            args.Messenger.RegisterView<WorkspaceViewModel, PaperControl>(ViewPane.Main);
        }
    }

    [DisplayName("コマンド: 紙の新規作成"), Description("「新しい紙」「リセット」コマンドを含みます。")]
    public class NewPaperPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            var newPaperExecutor = new NewPaperExecutor(args.Workspace);
            args.Workspace.AddEffect(newPaperExecutor.NewPaperEffect);
            args.Workspace.AddEffect(newPaperExecutor.ResetEffect);
            args.Messenger.SetEffectParameterViewModel<NewPaperExecutor>(p => new NewPaperDialogViewModel(args.Messenger, args.Dispatcher, p));
            args.Messenger.RegisterView<NewPaperDialogViewModel, NewPaperDialogControl>(ViewPane.Dialog);
        }
    }

    [DisplayName("ビュー: ツールバー"), Description("画面上部の各種機能が並んだツールバー。")]
    public class EffectViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new EffectListViewModel(args.Workspace, args.Messenger));
            args.Messenger.RegisterView<EffectListViewModel, EffectListControl>(ViewPane.Top);
        }
    }

    [DisplayName("ビュー: ツールボックス"), Description("画面左のツール切り替えボックス。")]
    public class ToolViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new ToolListViewModel(args.Workspace, args.Dispatcher));
            args.Messenger.RegisterView<ToolListViewModel, ToolListControl>(ViewPane.Side);
        }
    }

    [DisplayName("ビュー: 展開図"), Description("展開図を表示します。")]
    public class NetViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new NetViewModel(args.Workspace.Paper, args.Dispatcher));
            args.Messenger.RegisterView<NetViewModel, NetControl>(ViewPane.Side);
        }
    }

    [DisplayName("ビュー: 計測"), Description("選択中の点・線の情報を表示します。")]
    public class MeasureViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new MeasureViewModel(args.Workspace.Paper, args.PointConverter, args.Dispatcher));
            args.Messenger.RegisterView<MeasureViewModel, MeasureControl>(ViewPane.Side);
        }
    }

    [DisplayName("ビュー: 折り図"), Description("メインビューの上に折り図を表示します。対応しているツールの操作時に折り図風の図が表示されます。")]
    public class FoldingInstructionPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new FoldingInstructionViewModel(args.Workspace, args.Dispatcher, args.PointConverter));
            args.Messenger.RegisterView<FoldingInstructionViewModel, FoldingInstructionControl>(ViewPane.Main);
        }
    }
}
