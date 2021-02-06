namespace Orimath.Plugins
open System.Threading

type IDispatcherInvoker =
    abstract member Invoke : action: (unit -> unit) -> unit


type IDispatcher =
    abstract member SyncContext : SynchronizationContext
    abstract member Background : IDispatcherInvoker
    abstract member UI : IDispatcherInvoker


[<AutoOpen>]
module DispatcherOperator =
    type IDispatcherInvoker with
        member inline _.Bind(m: IDispatcherInvoker, f) = m.Invoke(f)
        member inline _.Zero() = ()
        member inline this.Delay(f: unit -> unit) = this.Invoke(f)
