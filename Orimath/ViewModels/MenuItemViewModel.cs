using System.Collections.ObjectModel;
using System.Windows.Input;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class MenuItemViewModel : NotifyPropertyChanged
    {
        private static readonly KeyGestureConverter _keyGestureConverter = new KeyGestureConverter();

        public string Name { get; }

        public KeyGesture? ShortcutKey { get; }

        public string ShortcutKeyText { get; }

        public ICommand? Command { get; }

        public ObservableCollection<MenuItemViewModel> Children { get; } = new ObservableCollection<MenuItemViewModel>();

        public MenuItemViewModel(string name)
        {
            Name = name;
            ShortcutKeyText = "";
        }

        public MenuItemViewModel(IEffect effect, IMessenger messenger)
        {
            Name = effect.Name;
            Command = messenger.GetEffectCommand(effect);
            if (!string.IsNullOrEmpty(effect.ShortcutKey))
            {
                ShortcutKeyText = effect.ShortcutKey;
                try
                {
                    ShortcutKey = (KeyGesture)_keyGestureConverter.ConvertFromInvariantString(effect.ShortcutKey);
                }
#pragma warning disable CA1031
                catch { }
#pragma warning restore CA1031
            }
            else
            {
                ShortcutKeyText = "";
            }
        }
    }
}
