using System.Windows.Input;
using Orimath.Controls;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.ViewModels
{
    public class ParametricEffectDialogViewModel : NotifyPropertyChanged
    {
        private readonly IParametricEffect _effect;
        private readonly IDispatcher _dispatcher;
        private readonly IMessenger _messenger;

        public string Header { get; }

        public object Parameter { get; }

        public ParametricEffectDialogViewModel(IParametricEffect effect, IDispatcher dispatcher, WorkspaceViewModel parent)
        {
            _effect = effect;
            _dispatcher = dispatcher;
            _messenger = parent;

            Header = effect.Name;
            Parameter = parent.GetEffectParameterViewModel(_effect.GetParameter());
            ExecuteCommand = Prop.True.ToCommand(Execute);
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
