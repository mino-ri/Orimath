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
}
