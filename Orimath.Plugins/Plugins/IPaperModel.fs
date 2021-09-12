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
    abstract member RawUndoItemType : Type
    abstract member GetRawUndoItems : unit -> obj
    abstract member SetRawUndoItems : obj -> unit
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

    [<Extension>]
    static member MergeCreases(paper: IPaperModel, layer: ILayerModel, creases: seq<Crease>) =
        let isNewCrease (crease: Crease) =
            layer.Creases
            |> Seq.forall (fun c -> not (LineSegment.hasIntersection crease.Segment c.Segment))
        if creases |> Seq.forall isNewCrease then
            layer.AddCreases(creases)
        else
            let index = layer.Index
            let newLayer = Layer.create layer.Edges [] [] layer.LayerType layer.OriginalEdges layer.Matrix
            paper.ReplaceLayer(index, newLayer)
            paper.Layers.[index].AddCreases(Crease.merge (Seq.append layer.Creases creases))
