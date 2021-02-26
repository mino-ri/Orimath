namespace Orimath.Plugins
open System
open System.Runtime.CompilerServices
open Orimath.Core
open ApplicativeProperty

type IPaperModel =
    inherit IPaper
    abstract member Layers : IReactiveCollection<ILayerModel>
    abstract member CanUndo : IGetProp<bool>
    abstract member CanRedo : IGetProp<bool>
    abstract member SelectedLayers : IProp<ILayerModel[]>
    abstract member SelectedEdges : IProp<Edge[]>
    abstract member SelectedPoints : IProp<Point[]>
    abstract member SelectedCreases : IProp<Crease[]>

    abstract member Undo : unit -> unit
    abstract member Redo : unit -> unit
    abstract member BeginChange : tag: obj -> IDisposable
    abstract member UndoSnapShots : seq<Paper * obj>
    abstract member RedoSnapShots : seq<Paper * obj>
    abstract member Clear : unit -> unit
    abstract member Clear : paper: IPaper -> unit
    abstract member ClearUndoStack : unit -> unit
    abstract member AddLayers : layers: seq<ILayer> -> unit
    abstract member RemoveLayers : count: int -> unit
    abstract member ReplaceLayer : index: int * newLayer: ILayer -> unit


[<Extension>]
type PaperModelExtensions =
    [<Extension>]
    static member IsSelected(paper: IPaperModel, point) =
        Array.contains point paper.SelectedPoints.Value

    [<Extension>]
    static member IsSelected(paper: IPaperModel, crease) =
        Array.contains crease paper.SelectedCreases.Value

    [<Extension>]
    static member IsSelected(paper: IPaperModel, edge) =
        Array.contains edge paper.SelectedEdges.Value

    [<Extension>]
    static member IsSelected(paper: IPaperModel, layer) =
        Array.contains layer paper.SelectedLayers.Value
