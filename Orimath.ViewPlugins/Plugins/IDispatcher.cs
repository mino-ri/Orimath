using System;
using System.Threading.Tasks;
using System.Threading;

namespace Orimath.Plugins
{
    public interface IDispatcher
    {
        SynchronizationContext SynchronizationContext { get; }
        void OnBackgroundAsync(Action action);
        void OnUIAsync(Action action);
    }
}
