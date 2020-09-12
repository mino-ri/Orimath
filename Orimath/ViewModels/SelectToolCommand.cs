using Orimath.Plugins;
using System;
using System.Windows.Input;

namespace Orimath.ViewModels
{
    public class SelectToolCommand : ICommand
    {
        private readonly WorkspaceViewModel _parent;

        public event EventHandler? CanExecuteChanged;

        public SelectToolCommand(WorkspaceViewModel parent)
        {
            _parent = parent;
            _parent.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(WorkspaceViewModel.RootEnable))
                    CanExecuteChanged?.Invoke(this, e);
            };
        }

        public bool CanExecute(object? parameter) => _parent.RootEnable;

        public void Execute(object? parameter)
        {
            if (parameter is ITool tool)
                _parent.SelectTool(tool);
        }
    }
}
