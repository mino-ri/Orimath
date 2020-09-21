using System;
using System.IO;
using Orimath.IO;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class GlobalSettingEffect : IParametricEffect
    {
        public object Parameter { get; }

        public string[] MenuHieralchy => new[] { "設定" };

        public string Name => "環境設定";

        public string ShortcutKey => "";

        public Stream? Icon => null;

        public event EventHandler CanExecuteChanged { add { } remove { } }

        public GlobalSettingEffect(GlobalSetting setting)
        {
            Parameter = setting;
        }

        public bool CanExecute() => true;

        public void Execute()
        {
            Settings.Save(SettingName.Global, Parameter);
        }
    }
}
