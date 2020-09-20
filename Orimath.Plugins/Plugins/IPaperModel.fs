namespace Orimath.Plugins
open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open Orimath.Core

type IPaperModel =
    inherit IPaper
    abstract member Layers : IReadOnlyList<ILayerModel>
    abstract member CanUndo : bool
    abstract member CanRedo : bool
    abstract member SelectedLayers : ILayerModel[] with get, set
    abstract member SelectedEdges : Edge[] with get, set
    abstract member SelectedPoints : Point[] with get, set
    abstract member SelectedLines : LineSegment[] with get, set
    abstract member ChangeBlockDeclared : bool

    [<CLIEvent>]
    abstract member SelectedLayersChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member SelectedEdgesChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member SelectedPointsChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member SelectedLinesChanged : IEvent<EventHandler, EventArgs>
    [<CLIEvent>]
    abstract member CanUndoChanged : IEvent<EventHandler, EventArgs>

    [<CLIEvent>]
    abstract member LayerChanged : ICollectionChangedEvent<ILayerModel>

    abstract member GetSnapShot : unit -> IPaper
    abstract member Undo : unit -> unit
    abstract member Redo : unit -> unit
    abstract member BeginChange : unit -> IDisposable
    abstract member Clear : unit -> unit
    abstract member Clear : paper: IPaper -> unit
    abstract member ClearUndoStack : unit -> unit
    abstract member AddLayers : layers: seq<ILayer> -> unit
    abstract member RemoveLayers : count: int -> unit
    abstract member ReplaceLayer : index: int * newLayer: ILayer -> unit

[<Extension>]
type PaperModelExtensions =
    [<Extension>]
    static member TryBeginChange(paper: IPaperModel) =
        if paper.ChangeBlockDeclared then
            { new IDisposable with member _.Dispose() = () }
        else
            paper.BeginChange()

    [<Extension>]
    static member IsSelected(paper: IPaperModel, point) =
        Array.contains point paper.SelectedPoints

    [<Extension>]
    static member IsSelected(paper: IPaperModel, line) =
        Array.contains line paper.SelectedLines

    [<Extension>]
    static member IsSelected(paper: IPaperModel, edge) =
        Array.contains edge paper.SelectedEdges

    [<Extension>]
    static member IsSelected(paper: IPaperModel, layer) =
        Array.contains layer paper.SelectedLayers
