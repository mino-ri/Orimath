using Orimath.Plugins;
using System;
using System.Windows.Input;

namespace Orimath.ViewModels
{
    public class EffectCommand : ICommand
    {
        private readonly IEffect _effect;
        private readonly IDispatcher _dispatcher;
        private readonly WorkspaceViewModel _parent;

        public EffectCommand(IEffect effect, IDispatcher dispatcher, WorkspaceViewModel parent)
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

        public bool CanExecute(object parameter) => _parent.RootEnable && _effect.CanExecute();

        public void Execute(object parameter) => _dispatcher.OnBackgroundAsync(_effect.Execute);
    }
}
