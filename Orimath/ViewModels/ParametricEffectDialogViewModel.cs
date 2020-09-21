using System.Windows.Input;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class ParametricEffectDialogViewModel : NotifyPropertyChanged
    {
        private readonly IParametricEffect _effect;
        private readonly IDispatcher _dispatcher;
        private readonly IMessenger _messenger;

        public string Header { get; }

        public SettingViewModel Parameter { get; }

        public ParametricEffectDialogViewModel(IParametricEffect effect, IDispatcher dispatcher, IMessenger messenger)
        {
            _effect = effect;
            _dispatcher = dispatcher;
            _messenger = messenger;

            Header = effect.Name;
            Parameter = new SettingViewModel(_effect.Parameter);
            ExecuteCommand = new ActionCommand(Execute);
        }

        public async void Execute(object? dummy)
        {
            await _dispatcher.OnBackgroundAsync(_effect.Execute);
            _messenger.CloseDialog();
        }

        public ICommand ExecuteCommand { get; }

        public ICommand CloseCommand => _messenger.CloseDialogCommand;
    }
}
