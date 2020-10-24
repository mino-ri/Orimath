﻿using System;
using System.Windows.Input;

namespace Orimath.Plugins
{
    public interface IMessenger
    {
        void AddViewModel(object viewModel);
        void RemoveViewModel(Type viewModelType);
        void RemoveViewModel(object viewModel);
        void SetEffectParameterViewModel<T>(Func<T, object> mapping);
        void OpenDialog(object viewModel);
        void CloseDialog();
        ICommand CloseDialogCommand { get; }
        ICommand GetEffectCommand(IEffect effect);
    }
}
