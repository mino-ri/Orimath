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
        }
    }

    [DisplayName("ビュー: ツールバー"), Description("画面上部の各種機能が並んだツールバー。")]
    public class EffectViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new EffectListViewModel(args.Workspace, args.Messenger));
        }
    }

    [DisplayName("ビュー: ツールボックス"), Description("画面左のツール切り替えボックス。")]
    public class ToolViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new ToolListViewModel(args.Workspace, args.Dispatcher));
        }
    }

    [DisplayName("ビュー: 計測"), Description("選択中の点・線の情報を表示します。")]
    public class MeasureViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new MeasureViewModel(args.Workspace.Paper, args.PointConverter, args.Dispatcher));
        }
    }
}
