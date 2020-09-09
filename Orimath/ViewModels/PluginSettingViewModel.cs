using Mvvm;
using Orimath.Plugins;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;

namespace Orimath.ViewModels
{
    public class PluginSettingViewModel : NotifyPropertyChanged
    {
        private readonly IMessenger _messenger;

        public ObservableCollection<PluginViewModel> Plugins { get; } = new ObservableCollection<PluginViewModel>();

        public ObservableCollection<PluginViewModel> ViewPlugins { get; } = new ObservableCollection<PluginViewModel>();

        public PluginSettingViewModel(IMessenger messenger)
        {
            _messenger = messenger;

            SetViewModels(PluginExecutor.LoadedPluginTypes, PluginExecutor.Setting.PluginOrder, Plugins);
            SetViewModels(PluginExecutor.LoadedViewPluginTypes, PluginExecutor.Setting.ViewPluginOrder, ViewPlugins);

            _upPluginCommand = new ActionCommand(UpPlugin, _ => 1 <= PluginIndex && PluginIndex < Plugins.Count);
            _downPluginCommand = new ActionCommand(DownPlugin, _ => 0 <= PluginIndex && PluginIndex < Plugins.Count - 1);
            _upViewPluginCommand = new ActionCommand(UpViewPlugin, _ => 1 <= ViewPluginIndex && ViewPluginIndex < ViewPlugins.Count);
            _downViewPluginCommand = new ActionCommand(DownViewPlugin, _ => 0 <= ViewPluginIndex && ViewPluginIndex < ViewPlugins.Count - 1);

            SaveCommand = new ActionCommand(Save);
            CloseCommand = messenger.CloseDialogCommand;
        }

        private static void SetViewModels(Type[] pluginTypes, string[] order, ObservableCollection<PluginViewModel> collection)
        {
            var plugins = pluginTypes
                .Select(t => new PluginViewModel(t, false))
                .ToDictionary(vm => vm.FullName);

            foreach (var fullName in order)
            {
                if (plugins.TryGetValue(fullName, out var plugin))
                {
                    plugin.IsEnabled = true;
                    plugins.Remove(fullName);
                    collection.Add(plugin);
                }
            }

            foreach(var plugin in  plugins.Values)
            {
                collection.Add(plugin);
            }
        }

        private int _pluginIndex;
        public int PluginIndex
        {
            get => _pluginIndex;
            set
            {
                if (SetValue(ref _pluginIndex, value))
                {
                    _upPluginCommand.OnCanExecuteChanged();
                    _downPluginCommand.OnCanExecuteChanged();
                }
            }
        }

        private int _viewPluginIndex;
        public int ViewPluginIndex
        {
            get => _viewPluginIndex;
            set
            {
                if (SetValue(ref _viewPluginIndex, value))
                {
                    _upViewPluginCommand.OnCanExecuteChanged();
                    _downViewPluginCommand.OnCanExecuteChanged();
                }
            }
        }

        public void Save(object? dummy)
        {
            PluginExecutor.Setting.PluginOrder = Plugins.Where(x => x.IsEnabled).Select(x => x.FullName).ToArray();
            PluginExecutor.Setting.ViewPluginOrder = ViewPlugins.Where(x => x.IsEnabled).Select(x => x.FullName).ToArray();
            PluginExecutor.SaveSetting();
            _messenger.CloseDialog();
        }

        public void UpPlugin(object? dummy) => Plugins.Move(PluginIndex, PluginIndex - 1);

        public void DownPlugin(object? dummy) => Plugins.Move(PluginIndex, PluginIndex + 1);

        public void UpViewPlugin(object? dummy) => ViewPlugins.Move(ViewPluginIndex, ViewPluginIndex - 1);

        public void DownViewPlugin(object? dummy) => ViewPlugins.Move(ViewPluginIndex, ViewPluginIndex + 1);

        public ICommand SaveCommand { get; }

        public ICommand CloseCommand { get; }

        private readonly ActionCommand _upPluginCommand;
        public ICommand UpPluginCommand => _upPluginCommand;

        private readonly ActionCommand _downPluginCommand;
        public ICommand DownPluginCommand => _downPluginCommand;

        private readonly ActionCommand _upViewPluginCommand;
        public ICommand UpViewPluginCommand => _upViewPluginCommand;

        private readonly ActionCommand _downViewPluginCommand;
        public ICommand DownViewPluginCommand => _downViewPluginCommand;
    }

    public class PluginViewModel : NotifyPropertyChanged
    {
        public string Name { get; }
        public string Description { get; }
        public string FullName { get; }
        public Type Type { get; }

        private bool _isEnabled;
        public bool IsEnabled { get => _isEnabled; set => SetValue(ref _isEnabled, value); }

        public PluginViewModel(Type pluginType, bool isEnabled)
        {
            Name = pluginType.GetCustomAttribute<DisplayNameAttribute>() is { } displayName
                ? displayName.DisplayName
                : pluginType.Name;
            Description = pluginType.GetCustomAttribute<DescriptionAttribute>() is { } description
                ? description.Description
                : "(No description)";

            FullName = pluginType.FullName!;
            Type = pluginType;
            _isEnabled = isEnabled;
        }
    }
}
