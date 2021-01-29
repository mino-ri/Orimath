using System;
using System.IO;
using Orimath.IO;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.ViewModels
{
    public class GlobalSettingEffect : IParametricEffect
    {
        private GlobalSetting _rootSetting;
        private GlobalSetting? _setting;

        public object GetParameter() => _setting = (GlobalSetting)_rootSetting.Clone();

        public string[] MenuHieralchy => new[] { "設定" };

        public string Name => "環境設定";

        public string ShortcutKey => "";

        public Stream? Icon => null;

        public GlobalSettingEffect(GlobalSetting setting)
        {
            _rootSetting = setting;
        }

        public IGetProp<bool> CanExecute => Prop.True;

        public void Execute()
        {
            if (_setting is null) return;

            _rootSetting.ViewSize = _setting.ViewSize;
            Settings.Save(SettingName.Global, _rootSetting);
        }
    }
}
