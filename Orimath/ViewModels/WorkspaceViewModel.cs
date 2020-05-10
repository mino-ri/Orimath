using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class WorkspaceViewModel : NotifyPropertyChanged, IMessenger
    {
        private readonly IWorkspace _workspace;
        private readonly IViewPointConverter _pointConverter = new ViewPointConverter(512.0, 16.0, 16.0);
        private readonly OrimathDispatcher _dispatcher = new OrimathDispatcher();
        private readonly Dictionary<IEffect, EffectCommand> _effectCommands = new Dictionary<IEffect, EffectCommand>();

        private readonly ActionCommand _closeDialogCommand;
        private readonly ObservableCollection<object> _preViewModels = new ObservableCollection<object>();

        private bool _initialized;
        private object? _dialog;

        public Dictionary<Type, (ViewPane pane, Type type)> ViewDefinitions { get; } = new Dictionary<Type, (ViewPane pane, Type type)>();

        public ObservableCollection<object> MainViewModels { get; } = new ObservableCollection<object>();

        public ObservableCollection<object> TopViewModels { get; } = new ObservableCollection<object>();

        public ObservableCollection<object> SideViewModels { get; } = new ObservableCollection<object>();

        public object? Dialog
        {
            get => _dialog;
            private set
            {
                if (SetValue(ref _dialog, value))
                {
                    OnPropertyChanged(nameof(HasDialog));
                    OnPropertyChanged(nameof(HasNotDialog));
                    OnPropertyChanged(nameof(RootEnable));
                    _closeDialogCommand.OnCanExecuteChanged();
                }
            }
        }

        public bool HasDialog => _dialog is { };

        public bool HasNotDialog => !HasDialog;

        public bool IsExecuting => _dispatcher.IsExecuting;

        public bool RootEnable => !HasDialog && !IsExecuting;

        public ICommand CloseDialogCommand => _closeDialogCommand;

        public WorkspaceViewModel(IWorkspace workspace)
        {
            _workspace = workspace;
            _closeDialogCommand = new ActionCommand(_ => CloseDialog(), _ => HasDialog);
            _dispatcher.IsExecutingChanged += (_, __) =>
            {
                OnPropertyChanged(nameof(IsExecuting));
                OnPropertyChanged(nameof(RootEnable));
            };
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

        public void Initialize()
        {
            var viewArgs = PluginExecutor.ExecutePlugins(new ViewPluginArgs(
                _workspace,
                this,
                _dispatcher,
                _pointConverter));

            foreach (var (viewType, att) in viewArgs)
                ViewDefinitions[att.ViewModelType] = (att.Pane, viewType);

            foreach (var effect in _workspace.Effects)
                _effectCommands[effect] = new EffectCommand(effect, _dispatcher);

            _workspace.Initialize();
            _initialized = true;
        }

        public void LoadViewModels()
        {
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
            if (collection is { })
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

        public void OpenDialog(object viewModel)
        {
            if (!_initialized) throw new InvalidOperationException("初期化完了前にダイアログを表示することはできません。");
            Dialog = viewModel;
        }

        public void CloseDialog() => Dialog = null;

        public ICommand? GetEffectCommand(IEffect effect)
        {
            return _effectCommands.TryGetValue(effect, out var command)
                ? command
                : null;
        }
    }
}
