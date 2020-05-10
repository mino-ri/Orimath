using Orimath.Plugins;
using System;
using System.Windows.Input;

namespace Orimath.ViewModels
{
    public class EffectCommand : ICommand
    {
        private readonly IEffect _effect;
        private readonly IDispatcher _dispatcher;

        public EffectCommand(IEffect effect, IDispatcher dispatcher)
        {
            _effect = effect;
            _dispatcher = dispatcher;
            _effect.CanExecuteChanged += (sender, e) =>
                _dispatcher.OnUIAsync(() => CanExecuteChanged?.Invoke(this, e));
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object parameter) => _effect.CanExecute();

        public void Execute(object parameter) => _dispatcher.OnBackgroundAsync(_effect.Execute);
    }
}
