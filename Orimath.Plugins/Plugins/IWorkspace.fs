namespace Orimath.Plugins
open System.Collections.Generic
open System.IO
open ApplicativeProperty

type IWorkspace =
    abstract member Paper : IPaperModel
    abstract member Tools : IReadOnlyList<ITool>
    abstract member Effects : IReadOnlyList<IEffect>
    abstract member CurrentTool : IProp<ITool>
    abstract member AddTool : tool: ITool -> unit
    abstract member AddEffect : effect: IEffect -> unit
    abstract member Initialize : unit -> unit


type IFileManager =
    abstract member SavePaper : unit -> Async<unit>
    abstract member LoadPaper : unit -> Async<unit>
    abstract member SaveObject<'T> : fileTypeName: string * filter: string * object: 'T -> Async<unit>
    abstract member LoadObject<'T> : fileTypeName: string * filter: string -> Async<'T option>
    abstract member SaveStream : fileTypeName: string * filter: string -> Async<Stream option>
    abstract member LoadStream : fileTypeName: string * filter: string -> Async<Stream option>
