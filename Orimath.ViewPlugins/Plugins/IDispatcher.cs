using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Threading;

namespace Orimath.Plugins
{
    public interface IDispatcher
    {
        SynchronizationContext SynchronizationContext { get; }
        Task OnBackgroundAsync(Action action);
        Task OnBackgroundAsync(Func<Task> action);
        Task OnUIAsync(Action action);
    }

    public struct DispatcherUISwitcher : INotifyCompletion
    {
        private readonly IDispatcher _dispatcher;

        public DispatcherUISwitcher(IDispatcher dispatcher) => _dispatcher = dispatcher;

        public bool IsCompleted => false;
        public void OnCompleted(Action action) => _dispatcher.OnUIAsync(action);
        public void GetResult() { }
        public DispatcherUISwitcher GetAwaiter() => this;
    }

    public struct DispatcherBackgroundSwitcher : INotifyCompletion
    {
        private readonly IDispatcher _dispatcher;

        public DispatcherBackgroundSwitcher(IDispatcher dispatcher) => _dispatcher = dispatcher;

        public bool IsCompleted => false;
        public void OnCompleted(Action action) => _dispatcher.OnBackgroundAsync(action);
        public void GetResult() { }
        public DispatcherBackgroundSwitcher GetAwaiter() => this;
    }

    public static class DispatcherExtensions
    {
        public static DispatcherUISwitcher SwitchToUI(this IDispatcher dispatcher) =>
            new DispatcherUISwitcher(dispatcher);

        public static DispatcherBackgroundSwitcher SwitchToBackground(this IDispatcher dispatcher) =>
            new DispatcherBackgroundSwitcher(dispatcher);
    }
}
