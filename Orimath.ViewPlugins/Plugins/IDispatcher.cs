using System;
using System.Threading.Tasks;
using System.Threading;

namespace Orimath.Plugins
{
    public interface IDispatcher
    {
        SynchronizationContext SynchronizationContext { get; }
        Task OnBackgroundAsync(Action action);
        Task OnUIAsync(Action action);
    }
}
