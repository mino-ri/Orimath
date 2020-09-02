using Orimath.Plugins;
using Orimath.Basics.View.ViewModels;

namespace Orimath.Basics.View
{
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

    public class EffectViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new EffectListViewModel(args.Workspace, args.Messenger));
        }
    }

    public class ToolViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new ToolListViewModel(args.Workspace, args.Dispatcher));
        }
    }

    public class MeasureViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new MeasureViewModel(args.Workspace.Paper, args.PointConverter, args.Dispatcher));
        }
    }
}
