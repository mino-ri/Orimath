using Orimath.Plugins;
using Orimath.UITest.ViewModels;
using System;
using System.IO;

namespace Orimath.UITest
{
    public class UITestPlugin : IViewPlugin
    {
        public void Execute(ViewPluginArgs args)
        {
            args.Workspace.AddEffect(new UITestEffect(args.Messenger, args.Dispatcher));
        }
    }

    public class UITestEffect : IEffect
    {
        private readonly IMessenger _messenger;
        private readonly IDispatcher _dispatcher;
        private ControlListViewModel? _viewModel;

        public UITestEffect(IMessenger messenger, IDispatcher dispatcher)
        {
            _messenger = messenger;
            _dispatcher = dispatcher;
        }

        public string[] MenuHieralchy { get; } = new[] { "デバッグ" };
        public string Name => "UIテストを開く";
        public string ShortcutKey => "";
        public Stream? Icon => null;
        public event EventHandler? CanExecuteChanged;
        public bool CanExecute() => true;

        public void Execute()
        {
            _dispatcher.OnUIAsync(() => _messenger.OpenDialog(_viewModel ??= new ControlListViewModel(_messenger)));
        }
    }
}
