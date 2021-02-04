using System;
using System.Windows;
using System.Windows.Input;

namespace Orimath.Plugins
{
    public enum ViewPane
    {
        Main = 0,
        Top = 1,
        Side = 2,
        Dialog = 3,
    }

    public interface IMessenger
    {
        void AddViewModel(object viewModel);
        void RemoveViewModel(Type viewModelType);
        void RemoveViewModel(object viewModel);
        void RegisterView(ViewPane viewPane, Type viewModelType, Type viewType);
        void RegisterView(ViewPane viewPane, Type viewModelType, string viewUri);
        void SetEffectParameterViewModel<TViewModel>(Func<TViewModel, object> mapping);
        void OpenDialog(object viewModel);
        void CloseDialog();
        ICommand CloseDialogCommand { get; }
        ICommand GetEffectCommand(IEffect effect);
    }

    public static class MessengerExtensions
    {
        public static void RegisterView<TViewModel, TView>(this IMessenger messenger, ViewPane viewPane)
            where TView : FrameworkElement
        {
            messenger.RegisterView(viewPane, typeof(TViewModel), typeof(TView));
        }
    }
}
