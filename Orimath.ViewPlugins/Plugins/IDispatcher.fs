namespace Orimath.Plugins
open System
open System.Threading.Tasks

type IDispatcher =
    abstract member OnBackgroundAsync : action: Action -> Task
    abstract member OnBackgroundAsync : action: Func<Task> -> Task
    abstract member OnUI : action: Action -> unit
    abstract member OnUIAsync : action: Action -> Task
