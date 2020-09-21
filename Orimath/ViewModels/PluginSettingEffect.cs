using Orimath.Plugins;
using System;
using System.IO;

namespace Orimath.ViewModels
{
    public class PluginSettingEffect : IEffect
    {
        private IMessenger _messenger;

        public string[] MenuHieralchy => new[] { "設定" };

        public string Name => "プラグインの設定";

        public string ShortcutKey => "";

        public Stream? Icon => null;

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public PluginSettingEffect(IMessenger messenger) => _messenger = messenger;

        public bool CanExecute() => true;

        public void Execute()
        {
            _messenger.OpenDialog(new PluginSettingViewModel(_messenger));
        }
    }
}
