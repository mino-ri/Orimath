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
        private PluginLoadSettingViewModel _loadSetting;

        public PluginSettingPageViewModel[] Pages { get; }

        public PluginSettingViewModel(IMessenger messenger)
        {
            _loadSetting = new PluginLoadSettingViewModel(messenger);

            Pages =
                Enumerable.Concat(
                    new PluginSettingPageViewModel[] { _loadSetting },
                    PluginExecutor.ConfigurablePlugins.Select(c => new PluginItemSettingViewModel(c)))
                .ToArray();

            SaveCommand = new ActionCommand(_loadSetting.Save);
            CloseCommand = messenger.CloseDialogCommand;
        }

        public ICommand SaveCommand { get; }

        public ICommand CloseCommand { get; }
     }

    public abstract class PluginSettingPageViewModel : NotifyPropertyChanged
    {
        public abstract string Header { get; }
    }

    public class PluginLoadSettingViewModel : PluginSettingPageViewModel
    {
        private readonly IMessenger _messenger;

        public override string Header => "ON/OFFと読み込み順";

        public ObservableCollection<PluginViewModel> Plugins { get; } = new ObservableCollection<PluginViewModel>();

        public PluginLoadSettingViewModel(IMessenger messenger)
        {
            _messenger = messenger;

            SetViewModels(PluginExecutor.LoadedPluginTypes.Concat(PluginExecutor.LoadedViewPluginTypes).ToArray(),
                          PluginExecutor.Setting.PluginOrder, Plugins);

            _upPluginCommand = new ActionCommand(UpPlugin, _ => 1 <= PluginIndex && PluginIndex < Plugins.Count);
            _downPluginCommand = new ActionCommand(DownPlugin, _ => 0 <= PluginIndex && PluginIndex < Plugins.Count - 1);
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

        public void Save(object? dummy)
        {
            PluginExecutor.Setting.PluginOrder = Plugins.Where(x => x.IsEnabled).Select(x => x.FullName).ToArray();
            PluginExecutor.SaveSetting();
            _messenger.CloseDialog();
        }

        public void UpPlugin(object? dummy) => Plugins.Move(PluginIndex, PluginIndex - 1);

        public void DownPlugin(object? dummy) => Plugins.Move(PluginIndex, PluginIndex + 1);

        private readonly ActionCommand _upPluginCommand;
        public ICommand UpPluginCommand => _upPluginCommand;

        private readonly ActionCommand _downPluginCommand;
        public ICommand DownPluginCommand => _downPluginCommand;
    }

    public class PluginItemSettingViewModel : PluginSettingPageViewModel
    {
        public override string Header { get; }

        public SettingViewModel Content { get; }

        public PluginItemSettingViewModel(IConfigurablePlugin plugin)
        {
            var pluginType = plugin.GetType();
            Header = pluginType.GetCustomAttribute<DisplayNameAttribute>() is { } displayName
                ? displayName.DisplayName
                : pluginType.Name;
            
            Content = new SettingViewModel(plugin.Setting);
        }
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
