using Orimath.Plugins;
using Orimath.Basics.View.ViewModels;

namespace Orimath.Basics.View
{
    public class BasicViewPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Messenger.AddViewModel(new WorkspaceViewModel(args.Workspace, args.PointConverter, args.Dispatcher));
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
