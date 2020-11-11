using System;
using System.Windows.Input;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class ParametricEffectCommand : ICommand
    {
        private readonly IParametricEffect _effect;
        private readonly IDispatcher _dispatcher;
        private readonly WorkspaceViewModel _parent;

        public ParametricEffectCommand(IParametricEffect effect, IDispatcher dispatcher, WorkspaceViewModel parent)
        {
            _effect = effect;
            _dispatcher = dispatcher;
            _parent = parent;
            _effect.CanExecuteChanged += (sender, e) =>
                _dispatcher.OnUIAsync(() => CanExecuteChanged?.Invoke(this, e));
            _parent.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(WorkspaceViewModel.RootEnable))
                    CanExecuteChanged?.Invoke(this, e);
            };
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter) => _parent.RootEnable && _effect.CanExecute();

        public void Execute(object? parameter) =>
            _parent.OpenDialog(new ParametricEffectDialogViewModel(_effect, _dispatcher, _parent));
    }
}
