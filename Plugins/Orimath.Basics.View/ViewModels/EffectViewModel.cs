using System.Windows.Input;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class EffectViewModel : NotifyPropertyChanged
    {
        private readonly IEffect _effect;

        public ICommand EffectCommand { get; }

        public string Name => _effect.Name;

        public string ShortcutKey => _effect.ShortcutKey;

        public string ToolTip => $"{Name} ({ShortcutKey})";

        public EffectViewModel(IEffect effect, IMessenger messenger)
        {
            _effect = effect;
            EffectCommand = messenger.GetEffectCommand(effect);
        }
    }
}
