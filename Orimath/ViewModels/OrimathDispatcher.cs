using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Orimath.Plugins;

namespace Orimath.ViewModels
{
    public class OrimathDispatcher : IDispatcher
    {
        private int _processCount;
        private readonly Dispatcher _dispatcher = Dispatcher.CurrentDispatcher;

        public bool IsExecuting => _processCount > 0;

        public event EventHandler? IsExecutingChanged;

        private void BeginBackground()
        {
            _processCount++;
            if (_processCount == 1) IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        }

        private void EndBackground()
        {
            _processCount--;
            if (_processCount == 0) IsExecutingChanged?.Invoke(this, EventArgs.Empty);
        }

        public async Task OnBackgroundAsync(Action action)
        {
            BeginBackground();
            try { await Task.Run(action); }
            finally { EndBackground(); }
        }

        public async Task OnBackgroundAsync(Func<Task> action)
        {
            BeginBackground();
            try { await action(); }
            finally { EndBackground(); }
        }

        public Task OnUIAsync(Action action) => _dispatcher.InvokeAsync(action).Task;
    }
}
