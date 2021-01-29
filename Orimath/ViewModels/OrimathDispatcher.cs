using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.ViewModels
{
    public class OrimathDispatcher : IDispatcher
    {
        private ValueProp<int> _processCount = new ValueProp<int>(0);
        public Dispatcher UIDispatcher { get; } = Dispatcher.CurrentDispatcher;
        public SynchronizationContext SynchronizationContext { get; }

        public IGetProp<bool> IsExecuting { get; }

        public OrimathDispatcher()
        {
            SynchronizationContext = new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher);
            SynchronizationContext.SetSynchronizationContext(SynchronizationContext);
            IsExecuting = _processCount.Select(p => p > 0).Cache();
        }

        private void BeginBackground()
        {
            _processCount.Increment();
        }

        private void EndBackground()
        {
            _processCount.Decrement();
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

        public Task OnUIAsync(Action action) => UIDispatcher.InvokeAsync(action).Task;
    }
}
