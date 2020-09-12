﻿using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class MenuItemViewModel : NotifyPropertyChanged
    {

        public string Name { get; }

        public KeyGesture? ShortcutKey { get; }

        public string ShortcutKeyText { get; }

        public ICommand? Command { get; }

        public Stream? IconStream { get; }

        public ObservableCollection<MenuItemViewModel> Children { get; } = new ObservableCollection<MenuItemViewModel>();

        public MenuItemViewModel(string name)
        {
            Name = name;
            ShortcutKeyText = "";
        }

        public MenuItemViewModel(string name, ICommand command)
        {
            Name = name;
            ShortcutKeyText = "";
            Command = command;
        }

        public MenuItemViewModel(IEffect effect, IMessenger messenger)
        {
            Name = effect.Name;
            Command = messenger.GetEffectCommand(effect);
            IconStream = effect.Icon;
            ShortcutKey = Internal.ConvertToKeyGesture(effect.ShortcutKey);
            ShortcutKeyText = ShortcutKey is { } ? effect.ShortcutKey : "";
        }
    }
}
