namespace Orimath.ViewModels
open System
open System.Threading
open System.Windows.Threading
open Orimath.Plugins
open ApplicativeProperty
open ApplicativeProperty.PropOperators

type OrimathDispatcher() =
    let processCount = Prop.value 0
    let syncContext = DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher)
    do SynchronizationContext.SetSynchronizationContext(syncContext)
    member val UIDispatcher = Dispatcher.CurrentDispatcher
    member val IsExecuting = processCount .> 0

    member _.SynchronizationContext = syncContext

    member private _.BeginBackground() = Prop.incr processCount

    member private _.EndBackground() = Prop.decr processCount
        
    member this.OnUI(action: Action) = ignore (this.UIDispatcher.InvokeAsync(action))

    member this.OnBackground(action: Action) =
        this.BeginBackground()
        Async.Start(async {
            try
                action.Invoke()
            finally
                this.OnUI(Action(this.EndBackground)) })

    interface IDispatcher with
        member this.SynchronizationContext = upcast this.SynchronizationContext
        member this.OnBackgroundAsync(action) = this.OnBackground(action)
        member this.OnUIAsync(action) = this.OnUI(action)
