namespace Orimath.Core
open System
open Orimath.Plugins
open ApplicativeProperty

type internal PaperOpr =
    | Clear of before: Layer list * after: Layer list
    | LayerAddition of index: int * layers: Layer list
    | LayerRemoving of index: int * layers: Layer list
    | LayerReplace of layerIndex: int * before: Layer * after: Layer
    | LineAddition of layerIndex: int * index: int * lines: Crease list
    | LineRemoving of layerIndex: int * index: int * lines: Crease list
    | PointAddition of layerIndex: int * index: int * points: Point list
    | PointRemoving of layerIndex: int * index: int * points: Point list


type internal IInternalPaperModel =
    inherit IPaperModel
    abstract member PushUndoOpr : opr: PaperOpr -> unit
    abstract member TryBeginChange : tag: obj -> IDisposable

and LayerModel internal (parent: IInternalPaperModel, layerIndex: int, init: ILayer) =
    let edges = asList init.Edges
    let layerCreases = ReactiveCollection<Crease>(init.Creases)
    let layerPoints = ReactiveCollection<Point>(init.Points)
    
    do layerCreases.Add(function
        | CollectionChange.Add(index, lines) ->
            parent.PushUndoOpr(LineAddition(layerIndex, index, asList lines))
        | CollectionChange.Remove(index, lines) ->
            parent.PushUndoOpr(LineRemoving(layerIndex, index, asList lines))
        | _ -> ())
    do layerPoints.Add(function
        | CollectionChange.Add(index, points) ->
            parent.PushUndoOpr(PointAddition(layerIndex, index, asList points))
        | CollectionChange.Remove(index, points) ->
            parent.PushUndoOpr(PointRemoving(layerIndex, index, asList points))
        | _ -> ())

    member _.Index = layerIndex
    member _.Edges = edges
    member _.Creases = layerCreases :> IReactiveCollection<_>
    member _.Points = layerPoints :> IReactiveCollection<_>
    member _.LayerType = init.LayerType

    member _.GetSnapShot() =
        Layer.create edges layerCreases layerPoints
            init.LayerType init.OriginalEdges init.Matrix

    member this.AddLineCore(segs: seq<Crease>, addCross) =
        let creases = [ for l in segs do if not (Layer.hasSeg l.Segment this) then l ]
        if creases <> [] then
            let points = if addCross then Layer.crossesAllCrease creases this else []
            use __ = parent.TryBeginChange(null)
            layerCreases.AddRange(creases)
            layerPoints.AddRange(points)

    member this.AddCreasesRaw(lines) =
        this.AddLineCore(Seq.collect (flip Layer.clip this >> Crease.ofSegs CreaseType.Crease) lines, false)

    member this.AddCreasesRaw(segs) =
        this.AddLineCore(Seq.collect (flip Layer.clipSeg this >> Crease.ofSegs CreaseType.Crease) segs, false)

    member this.AddCreasesRaw(creases) =
        this.AddLineCore(Seq.collect (flip Layer.clipCrease this) creases, false)

    member this.AddCreases(lines) =
        this.AddLineCore(Seq.collect (flip Layer.clip this >> Crease.ofSegs CreaseType.Crease) lines, true)

    member this.AddCreases(segs) =
        this.AddLineCore(Seq.collect (flip Layer.clipSeg this >> Crease.ofSegs CreaseType.Crease) segs, true)

    member this.AddCreases(creases) =
        this.AddLineCore(Seq.collect (flip Layer.clipCrease this) creases, true)

    member _.RemoveCreases(count) =
        if count > 0 then
            use __ = parent.TryBeginChange(null)
            layerCreases.RemoveTail(count)

    member this.AddPoints(points) =
        let points = [
            for p in points do
            if Layer.containsPoint p this && not (Layer.hasPoint p this) then p
        ]
        if points <> [] then
            use __ = parent.TryBeginChange(null)
            layerPoints.AddRange(points)

    member _.RemovePoints(count) =
        if count > 0 then
            use __ = parent.TryBeginChange(null)
            layerPoints.RemoveTail(count)

    interface ILayer with
        member _.Edges = upcast edges
        member this.Creases = upcast this.Creases
        member this.Points = upcast this.Points
        member _.LayerType = init.LayerType
        member _.OriginalEdges = init.OriginalEdges
        member _.Matrix = init.Matrix

    interface ILayerModel with
        member this.Index = this.Index
        member this.Creases = this.Creases
        member this.Points = this.Points
        member this.AddCreases(lines: seq<Line>) = this.AddCreases(lines)
        member this.AddCreases(segs: seq<LineSegment>) = this.AddCreases(segs)
        member this.AddCreases(creases: seq<Crease>) = this.AddCreases(creases)
        member this.AddCreasesRaw(lines: seq<Line>) = this.AddCreasesRaw(lines)
        member this.AddCreasesRaw(segs: seq<LineSegment>) = this.AddCreasesRaw(segs)
        member this.AddCreasesRaw(creases: seq<Crease>) = this.AddCreasesRaw(creases)
        member this.RemoveCreases(count) = this.RemoveCreases(count)
        member this.AddPoints(points) = this.AddPoints(points)
        member this.RemovePoints(count) = this.RemovePoints(count)
