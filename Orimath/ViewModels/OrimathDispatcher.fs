namespace Orimath.ViewModels
open System.Threading
open System.Windows.Threading
open Orimath.Plugins
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type OrimathDispatcher() =
    let processCount = Prop.value 0
    let uiDispatcher = Dispatcher.CurrentDispatcher
    let syncContext = DispatcherSynchronizationContext(uiDispatcher)
    do SynchronizationContext.SetSynchronizationContext(syncContext)
    let ui =
        { new IDispatcherInvoker with
            member _.Invoke(action) = ignore (uiDispatcher.InvokeAsync(action))
        }
    let background =
        { new IDispatcherInvoker with
            member _.Invoke(action) =
                Prop.incr processCount
                Async.Start(async {
                    try action()
                    finally ui.Invoke(fun () -> Prop.decr processCount)
                })
        }

    member val IsExecuting = processCount .> 0
    member _.SyncContext = syncContext
    member _.UI = ui
    member _.Background = background

    interface IDispatcher with
        member _.SyncContext = upcast syncContext
        member _.Background = background
        member _.UI = ui
