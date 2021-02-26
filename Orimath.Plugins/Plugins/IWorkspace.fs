namespace Orimath.Plugins
open System.Collections.Generic
open ApplicativeProperty

type IWorkspace =
    abstract member Paper : IPaperModel
    abstract member Tools : IReadOnlyList<ITool>
    abstract member Effects : IReadOnlyList<IEffect>
    abstract member CurrentTool : IProp<ITool>
    abstract member AddTool : tool: ITool -> unit
    abstract member AddEffect : effect: IEffect -> unit
    abstract member Initialize : unit -> unit
