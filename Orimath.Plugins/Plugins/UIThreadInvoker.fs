namespace Orimath.Plugins
open System
open System.Runtime.CompilerServices
open System.Threading.Tasks

type IUIThreadInvoker =
    abstract member Invoke : Action -> unit
    abstract member InvokeAsync : Action -> Task

[<Extension>]
module ThreadController =
    [<CompiledName("RunAsync")>]
    let runAsync action = Async.Start(async { action() })
    
    [<CompiledName("AwaitOnUI"); Extension>]
    let awaitOnUI (invoker: IUIThreadInvoker) action = invoker.Invoke(Action(action))

    [<CompiledName("AsyncOnUI"); Extension>]
    let asyncOnUI (invoker: IUIThreadInvoker) action = Async.AwaitTask(invoker.InvokeAsync(Action(action)))

    [<CompiledName("OnUI"); Extension>]
    let onUI (invoker: IUIThreadInvoker) action = ignore (invoker.InvokeAsync(Action(action)))
