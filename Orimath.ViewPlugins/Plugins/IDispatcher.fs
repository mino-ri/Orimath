namespace Orimath.Plugins
open System
open System.Threading

type IDispatcher =
    abstract member SynchronizationContext : SynchronizationContext
    abstract member OnBackgroundAsync : action: Action -> unit
    abstract member OnUIAsync : action: Action -> unit
