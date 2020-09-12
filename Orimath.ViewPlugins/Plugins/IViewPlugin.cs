namespace Orimath.Plugins
{
    public class ViewPluginArgs
    {
        public IWorkspace Workspace { get; }
        public IMessenger Messenger { get; }
        public IDispatcher Dispatcher { get; }
        public IViewPointConverter PointConverter { get; }

        public ViewPluginArgs(IWorkspace workspace, IMessenger messenger, IDispatcher dispatcher, IViewPointConverter pointConverter)
        {
            Workspace = workspace;
            Messenger = messenger;
            Dispatcher = dispatcher;
            PointConverter = pointConverter;
        }
    }

    public interface IViewPlugin
    {
        void Execute(ViewPluginArgs args);
    }
}
