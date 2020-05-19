using System.Collections.ObjectModel;
using System.Windows.Input;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class MenuItemViewModel : NotifyPropertyChanged
    {
        public string Name { get; }

        public ObservableCollection<MenuItemViewModel> Children { get; } = new ObservableCollection<MenuItemViewModel>();

        public ICommand? Command { get; }

        public MenuItemViewModel(string name)
        {
            Name = name;
        }

        public MenuItemViewModel(IEffect effect, IMessenger messenger)
        {
            Name = effect.Name;
            Command = messenger.GetEffectCommand(effect);
        }
    }
}
