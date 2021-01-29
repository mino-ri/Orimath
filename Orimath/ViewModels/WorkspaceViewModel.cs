using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Mvvm;
using Orimath.IO;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.ViewModels
{
    public class WorkspaceViewModel : NotifyPropertyChanged, IMessenger
    {
        private readonly IWorkspace _workspace;
        private readonly OrimathDispatcher _dispatcher = new();
        private readonly Dictionary<IEffect, ICommand> _effectCommands = new();
        private readonly Dictionary<Type, Func<object, object>> _effectParameterCreator = new();

        private readonly ObservableCollection<object> _preViewModels = new();
        private IEffect[] _systemEffects;

        private GlobalSetting _setting = new();

        private bool _initialized;

        public Dictionary<Type, (ViewPane pane, Type type)> ViewDefinitions { get; } = new();

        public ObservableCollection<object> MainViewModels { get; } = new();

        public ObservableCollection<object> TopViewModels { get; } = new();

        public ObservableCollection<object> SideViewModels { get; } = new();

        public ObservableCollection<MenuItemViewModel> MenuItems { get; } = new();

        public Dictionary<KeyGesture, ITool> ToolGestures { get; } = new();

        public IGetProp<object?> Dialog { get; }

        public IGetProp<bool> HasDialog { get; }

        public IGetProp<bool> HasNotDialog { get; }

        public IGetProp<bool> IsExecuting => _dispatcher.IsExecuting;

        public IGetProp<bool> RootEnable { get; }

        public double ViewSize { get; set; }

        public double Height { get => _setting.Height; set => _setting.Height = value; }

        public double Width { get => _setting.Width; set => _setting.Width = value; }

        public double Left { get => _setting.Left; set => _setting.Left = value; }

        public double Top { get => _setting.Top; set => _setting.Top = value; }

        public ICommand CloseDialogCommand { get; }

        private readonly ValueProp<object?> _dialog;

        public WorkspaceViewModel(IWorkspace workspace)
        {
            _workspace = workspace;
            _systemEffects = Array.Empty<IEffect>();

            _dialog = new ValueProp<object?>(null);
            Dialog = _dialog.AsGet();
            HasDialog = _dialog.Select(d => d is not null);
            HasNotDialog = HasDialog.Not();
            RootEnable = HasDialog.Zip(IsExecuting, (a, b) => !a && !b);

            CloseDialogCommand = HasDialog.ToCommand(_ => CloseDialog());
        }

        private ObservableCollection<object>? GetViewModelCollection(Type viewModelType)
        {
            if (!_initialized) return _preViewModels;
            if (!ViewDefinitions.TryGetValue(viewModelType, out var tuple)) return null;

            return tuple.pane switch
            {
                ViewPane.Main => MainViewModels,
                ViewPane.Top => TopViewModels,
                ViewPane.Side => SideViewModels,
                _ => null,
            };
        }

        public void SelectTool(ITool tool) => _workspace.CurrentTool.OnNext(tool);

        public void LoadSetting()
        {
            _setting = Settings.Load<GlobalSetting>(SettingName.Global) ?? new GlobalSetting();
            ViewSize = _setting.ViewSize * 2.0;

            _systemEffects = new IEffect[]
            {
                new GlobalSettingEffect(_setting),
                new PluginSettingEffect(this, _dispatcher),
            };
        }

        public void SaveSetting()
        {
            Settings.Save(SettingName.Global, _setting);
        }

        public void Initialize()
        {
            var pointConverter = new ViewPointConverter(_setting.ViewSize, -_setting.ViewSize, _setting.ViewSize * 0.5, _setting.ViewSize * 1.5);

            var viewArgs = PluginExecutor.ExecutePlugins(new ViewPluginArgs(
                _workspace,
                this,
                _dispatcher,
                pointConverter));

            foreach (var (viewType, att) in viewArgs)
                ViewDefinitions[att.ViewModelType] = (att.Pane, viewType);

            foreach (var effect in _workspace.Effects.Concat(_systemEffects))
                _effectCommands[effect] = EffectCommand.Create(effect, _dispatcher, this);

            foreach (var tool in _workspace.Tools)
            {
                if (Internal.ConvertToKeyGesture(tool.ShortcutKey) is { } gesture)
                    ToolGestures[gesture] = tool;
            }

            _workspace.Initialize();
            _initialized = true;
        }

        private void CreateMenu()
        {
            foreach (var effect in _workspace.Effects.Concat(_systemEffects))
            {
                var targetCollection = MenuItems;
                foreach (var name in effect.MenuHieralchy)
                {
                    var parent = targetCollection.FirstOrDefault(x => x.Name == name);
                    if (parent is null)
                    {
                        parent = new MenuItemViewModel(name);
                        targetCollection.Add(parent);
                    }

                    targetCollection = parent.Children;
                }

                targetCollection.Add(new MenuItemViewModel(effect, this));
            }
        }

        public void LoadViewModels()
        {
            CreateMenu();

            foreach (var item in _preViewModels) AddViewModel(item);
            _preViewModels.Clear();
        }

        public void AddViewModel(object viewModel)
        {
            if (viewModel is null) throw new ArgumentNullException(nameof(viewModel));
            GetViewModelCollection(viewModel.GetType())?.Add(viewModel);
        }

        public void RemoveViewModel(Type viewModelType)
        {
            if (viewModelType is null) throw new ArgumentNullException(nameof(viewModelType));
            var collection = GetViewModelCollection(viewModelType);
            if (collection is not null)
            {
                foreach (var vm in collection.Where(viewModelType.IsInstanceOfType).ToArray())
                    collection.Remove(vm);
            }
        }

        public void RemoveViewModel(object viewModel)
        {
            if (viewModel is null) throw new ArgumentNullException(nameof(viewModel));
            GetViewModelCollection(viewModel.GetType())?.Add(viewModel);
        }

        public void SetEffectParameterViewModel<T>(Func<T, object> mapping)
        {
            _effectParameterCreator[typeof(T)] = p => mapping((T)p);
        }

        public object GetEffectParameterViewModel(object parameter)
        {
            if (_effectParameterCreator.TryGetValue(parameter.GetType(), out var creator))
            {
                return creator(parameter);
            }
            else
            {
                return new SettingViewModel(parameter);
            }
        }

        public void OpenDialog(object viewModel)
        {
            if (!_initialized) throw new InvalidOperationException("初期化完了前にダイアログを表示することはできません。");
            _dialog.Value = viewModel;
        }

        public void CloseDialog() => _dialog.Value = null;

        public ICommand GetEffectCommand(IEffect effect)
        {
            return _effectCommands.TryGetValue(effect, out var command)
                ? command
                : null!;
        }
    }
}
