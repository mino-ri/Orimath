namespace Orimath.Plugins
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

type IDispatcher =
    abstract member OnBackgroundAsync : action: Action -> Task
    abstract member OnBackgroundAsync : action: Func<Task> -> Task
    abstract member OnUIAsync : action: Action -> Task

[<Struct>]
type DispatcherUISwitcher(dispatcher: IDispatcher) =
    member this.GetAwaiter() = this
    member __.IsCompleted = false
    member __.OnCompleted(action: Action) = ignore (dispatcher.OnUIAsync(action))
    member __.GetResult() = ()

    interface INotifyCompletion with
        member __.OnCompleted(action: Action) = ignore (dispatcher.OnUIAsync(action))

[<Struct>]
type DispatcherBackgroundSwitcher(dispatcher: IDispatcher) =
    member this.GetAwaiter() = this
    member __.IsCompleted = false
    member __.OnCompleted(action: Action) = ignore (dispatcher.OnBackgroundAsync(action))
    member __.GetResult() = ()

    interface INotifyCompletion with
        member __.OnCompleted(action: Action) = ignore (dispatcher.OnBackgroundAsync(action))

[<Extension>]
type DispatcherExtensions =
    [<Extension>]
    static member SwitchToUI(dispatcher: IDispatcher) = DispatcherUISwitcher(dispatcher)

    [<Extension>]
    static member SwitchToBackground(dispatcher: IDispatcher) = DispatcherBackgroundSwitcher(dispatcher)
