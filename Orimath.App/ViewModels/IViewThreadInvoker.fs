namespace Orimath.ViewModels
open System
open System.Threading.Tasks

type IUIThreadInvoker =
    abstract member Invoke : Action -> unit
    abstract member InvokeAsync : Action -> Task

[<AutoOpen>]
module internal UIThreadInvoker =
    let runAsync action = Async.Start(async { action() })
    
    let awaitOnUI (invoker: IUIThreadInvoker) action = invoker.Invoke(Action(action))

    let asyncOnUI (invoker: IUIThreadInvoker) action = Async.AwaitTask(invoker.InvokeAsync(Action(action)))

    let onUI (invoker: IUIThreadInvoker) action = ignore (invoker.InvokeAsync(Action(action)))
