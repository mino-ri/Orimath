using System;
using System.IO;
using Orimath.Plugins;
using Orimath.UITest.ViewModels;
using ApplicativeProperty;

namespace Orimath.UITest
{
    public class UITestPlugin : IViewPlugin, IConfigurablePlugin
    {
        public Type SettingType => typeof(UITestPluginSetting);

        public object Setting { get; set; } = new UITestPluginSetting();

        public void Execute(ViewPluginArgs args)
        {
            args.Workspace.AddEffect(new UITestEffect(args.Messenger, args.Dispatcher, (UITestPluginSetting)Setting));
        }
    }

    public class UITestPluginSetting
    {
        public string ContentText { get; set; } = "Content";
    }

    public class UITestEffect : IEffect
    {
        private readonly IMessenger _messenger;
        private readonly IDispatcher _dispatcher;
        private ControlListViewModel? _viewModel;
        private readonly UITestPluginSetting _setting;

        public UITestEffect(IMessenger messenger, IDispatcher dispatcher, UITestPluginSetting setting)
        {
            _messenger = messenger;
            _dispatcher = dispatcher;
            _setting = setting;
        }

        public string[] MenuHieralchy { get; } = new[] { "デバッグ" };
        public string Name => "UIテストを開く";
        public string ShortcutKey => "";
        public Stream? Icon => null;
        public IGetProp<bool> CanExecute => Prop.True;

        public void Execute()
        {
            _dispatcher.OnUIAsync(() => _messenger.OpenDialog(_viewModel ??= new ControlListViewModel(_messenger, _setting)));
        }
    }
}
