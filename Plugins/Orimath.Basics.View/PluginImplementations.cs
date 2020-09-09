using System.ComponentModel;
using Orimath.Plugins;
using Orimath.Basics.View.ViewModels;

namespace Orimath.Basics.View
{
    [DisplayName("メインビュー"), Description("メイン描画領域。このプラグインを削除すると、折り紙本体が表示されなくなります。")]
    public class BasicViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new WorkspaceViewModel(args.Workspace, args.PointConverter, args.Dispatcher));
            
            if (args.Workspace.GetEffectOrDefault<NewPaperEffect>() is { } newPaper)
            {
                newPaper.OnExecute += (sender, e) =>
                    args.Dispatcher.OnUIAsync(() =>
                        args.Messenger.OpenDialog(new NewPaperDialogViewModel(args.Messenger, args.Dispatcher, newPaper.Executor)));
            }
        }
    }

    [DisplayName("ツールバー"), Description("画面上部の各種機能が並んだツールバー。")]
    public class EffectViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new EffectListViewModel(args.Workspace, args.Messenger));
        }
    }

    [DisplayName("ツールボックス"), Description("画面左のツール切り替えボックス。")]
    public class ToolViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new ToolListViewModel(args.Workspace, args.Dispatcher));
        }
    }

    [DisplayName("計測ビュー"), Description("選択中の点・線の情報を表示します。")]
    public class MeasureViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new MeasureViewModel(args.Workspace.Paper, args.PointConverter, args.Dispatcher));
        }
    }
}
