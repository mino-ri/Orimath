using Orimath.Plugins;
using System.IO;
using ApplicativeProperty;

namespace Orimath.ViewModels
{
    public class PluginSettingEffect : IEffect
    {
        private IMessenger _messenger;
        private IDispatcher _dispatcher;

        public string[] MenuHieralchy => new[] { "設定" };

        public string Name => "プラグインの設定";

        public string ShortcutKey => "";

        public Stream? Icon => null;

        public PluginSettingEffect(IMessenger messenger, IDispatcher dispatcher)
        {
            _messenger = messenger;
            _dispatcher = dispatcher;
        }

        public IGetProp<bool> CanExecute => Prop.True;

        public void Execute()
        {
            _dispatcher.OnUIAsync(() => _messenger.OpenDialog(new PluginSettingViewModel(_messenger)));
        }
    }
}
