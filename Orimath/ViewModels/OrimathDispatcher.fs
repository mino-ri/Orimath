﻿namespace Orimath.ViewModels
open System.Threading
open System.Windows.Threading
open Orimath.Plugins
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type OrimathDispatcher(messenger: IMessenger) =
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
                    try
                        try action()
                        with ex ->
#if DEBUG
                            messenger.ShowMessage($"[%s{ex.GetType().Name}]\n%s{ex.Message}\n{ex.StackTrace}")
#else
                            messenger.ShowMessage($"[%s{ex.GetType().Name}]\n%s{ex.Message}")
#endif
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
