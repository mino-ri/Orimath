using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Orimath.Core;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class RootViewModel
    {
        public RootViewModel() { }

        public WorkspaceViewModel Workspace { get; } = 
            new WorkspaceViewModel(new Workspace(), new WpfThreadInvoker());
    }

    internal class WpfThreadInvoker : IUIThreadInvoker
    {
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        public void Invoke(Action value) => _dispatcher.InvokeAsync(value);

        public async Task InvokeAsync(Action value) => await _dispatcher.InvokeAsync(value);
    }
}
