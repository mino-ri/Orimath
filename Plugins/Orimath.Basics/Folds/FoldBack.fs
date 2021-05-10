module internal Orimath.Basics.Folds.FoldBack
open Orimath.Core
open Orimath.Plugins
open Orimath.Basics.Internal
open Orimath.Core.NearlyEquatable


[<ReferenceEquality; NoComparison>]
type private LayerSource =
    { Edges: Edge list
      OriginalEdges: Edge list
      Points: Point list
      Creases: Crease list }


[<ReferenceEquality; NoComparison>]
type private SplittedLayer =
    { Index: int
      Original: ILayer
      Static: LayerSource option
      Dynamic: LayerSource option
      mutable IsTarget: bool }


let private center = { X = 0.5; Y = 0.5 }

let private chooseUnzip (f: 'T -> 'U1 option * 'U2 option) (source: seq<'T>) =
    let leftResult = ResizeArray()
    let rightResult = ResizeArray()
    for item in source do
        let left, right = f item
        Option.iter leftResult.Add left
        Option.iter rightResult.Add right
    leftResult, rightResult

let private splitPoints foldLine layer =
    let positivePoints = ResizeArray()
    let negativePoints = ResizeArray()
    for point in Layer.crossesAll (Layer.clip foldLine layer) layer |> Seq.append layer.Points do
        match Line.distSign point foldLine with
        | 1 -> positivePoints.Add(point)
        | -1 -> negativePoints.Add(point)
        | _ ->
            positivePoints.Add(point)
            negativePoints.Add(point)
    positivePoints, negativePoints

let private splitLine foldLine (target: LineSegment) =
    match Line.distSign target.Point1 foldLine, Line.distSign target.Point2 foldLine with
    | 1, 1 | 1, 0 | 0, 1 -> Some(target), None
    | -1, -1 | -1, 0 | 0, -1 -> None, Some(target)
    | 1, -1 ->
        let cross = Line.cross foldLine target.Line |> Option.get
        LineSegment.FromPoints(target.Point1, cross),
        LineSegment.FromPoints(cross, target.Point2)
    | -1, 1 ->
        let cross = Line.cross foldLine target.Line |> Option.get
        LineSegment.FromPoints(cross, target.Point2),
        LineSegment.FromPoints(target.Point1, cross)
    // 0, 0
    | _ -> None, None

let private splitCrease foldLine (target: Crease) =
    let positive, negative = splitLine foldLine target.Segment
    positive |> Option.map (fun l -> { target with Segment = l }),
    negative |> Option.map (fun l -> { target with Segment = l })

let private splitCreases foldLine (layer: ILayer) =
    layer.Creases |> chooseUnzip (splitCrease foldLine)

let private splitEdge foldLine (target: Edge) =
    match splitLine foldLine target.Segment with
    | None, None -> Some(target), Some(target)
    | positive, negative ->
        positive |> Option.map (fun l -> { target with Segment = l }),
        negative |> Option.map (fun l -> { target with Segment = l })

let private splitEdges foldLine (layer: ILayer) =
    let rec recSelf positiveEdges negativeEdges index =
        let positive, negative = splitEdge foldLine layer.Edges.[index]
        let addEdge (edge: Edge option) (edges: Edge list) =
            match edge, edges with
            | Some(e), [] -> [ e ]
            | Some(e), head :: _ ->
                if e.Point2 =~ head.Point1 then e :: edges
                else
                    let newEdge = { Segment = LineSegment.FromPoints(e.Point2, head.Point1).Value; Inner = true }
                    e :: newEdge :: edges
            | None, _ -> edges
        let positiveEdges = addEdge positive positiveEdges
        let negativeEdges = addEdge negative negativeEdges
        if index <= 0 then
            let addFirstEdge (edges: Edge list) =
                if edges.Length < 2 then edges
                else
                    let head, last = edges.[0], List.last edges
                    if head.Point1 =~ last.Point2 then edges
                    else
                        let newEdge = { Segment = LineSegment.FromPoints(last.Point2, head.Point1).Value; Inner = true }
                        newEdge :: edges
            addFirstEdge positiveEdges, addFirstEdge negativeEdges
        else
            recSelf positiveEdges negativeEdges (index - 1)
    recSelf [] [] (layer.Edges.Count - 1)

let private splitOriginalEdges foldLine positiveEdgesCount (layer: ILayer) =
    let foldLineInOrigin = layer.Matrix.MultiplyInv(foldLine)
    let edges1, edges2 = splitEdges foldLineInOrigin (Layer.original layer)
    match positiveEdgesCount >= 3, edges1.Length >= 3 with
    | false, false -> edges1, edges2
    | true, false | false, true -> edges2, edges1
    | true, true ->
        let edgeSign =
            edges1
            |> Seq.map (fun e -> Line.distSign (e.Segment.Point1 * layer.Matrix) foldLine)
            |> Seq.find ((<>) 0)
        if edgeSign > 0 then edges1, edges2 else edges2, edges1

// returns positive-side, negative-side
let private splitLayerCore foldLine layer =
    let positiveEdges, negativeEdges = splitEdges foldLine layer
    let originalPositiveEdges, originalNegativeEdges = splitOriginalEdges foldLine positiveEdges.Length layer
    let positivePoints, negativePoints = splitPoints foldLine layer
    let positiveCreases, negativeCreases = splitCreases foldLine layer
    let positive =
        if positiveEdges.Length >= 3 then
            Some {
                Edges = positiveEdges
                OriginalEdges = Seq.toList originalPositiveEdges
                Points = Seq.toList positivePoints
                Creases = Seq.toList positiveCreases
            }
        else
            None
    let negative =
        if negativeEdges.Length >= 3 then
            Some {
                Edges = negativeEdges
                OriginalEdges = Seq.toList originalNegativeEdges
                Points = Seq.toList negativePoints
                Creases = Seq.toList negativeCreases
            }
        else
            None
    positive, negative

let private createLayer foldLine (layer: ILayer) turnOver source =
    if not turnOver then
        Layer.create
            source.Edges
            source.Creases
            source.Points
            layer.LayerType
            source.OriginalEdges
            layer.Matrix
        :> ILayer
    else
        Layer.create
            (source.Edges |> Seq.map (Edge.reflectBy foldLine))
            (source.Creases |> Seq.map (Crease.reflectBy foldLine))
            (source.Points |> Seq.map (Point.reflectBy foldLine))
            (LayerType.turnOver layer.LayerType)
            source.OriginalEdges
            (layer.Matrix * Matrix.OfReflection(foldLine))
        :> ILayer

// returns static-side, dynamic-side
let private splitLayer foldLine isPositiveStatic layer =
    let positive, negative = splitLayerCore foldLine layer
    if isPositiveStatic
    then Option.map (createLayer foldLine layer false) positive,
         Option.map (createLayer foldLine layer true) negative
    else Option.map (createLayer foldLine layer false) negative,
         Option.map (createLayer foldLine layer true) positive

let private isContacted originalEdges1 originalEdges2 =
    exists {
        let! e1 = originalEdges1 |> Seq.filter (fun e -> e.Inner)
        let! e2 = originalEdges2 |> Seq.filter (fun e -> e.Inner)
        return e1.Segment =~ e2.Segment
    }

let isPositiveStatic foldLine dynamicPoint =
    match dynamicPoint with
    | Some(OprPoint(point, _)) -> not (Line.isPositiveSide point foldLine)
    | None -> Line.isPositiveSide center foldLine

let private clusterLayers (layers: SplittedLayer[]) =
    let clusteredLayers = ResizeArray<SplittedLayer list>()
    let added = Array.zeroCreate<bool> layers.Length
    let rec addLayers index acm =
        added.[index] <- true
        let mutable newCluster = layers.[index] :: acm
        match layers.[index].Dynamic with
        | Some(dynamic) ->
            for i = 0 to layers.Length - 1 do
                let ok =
                    not added.[i] && exists {
                        let! target = layers.[i].Dynamic
                        return isContacted dynamic.OriginalEdges target.OriginalEdges
                    }
                if ok then
                    newCluster <- addLayers i newCluster
        | None -> ()
        newCluster
    for i = 0 to layers.Length - 1 do
        if not added.[i] then
            clusteredLayers.Add(addLayers i [])
    clusteredLayers

let splitPaper (workspace: IWorkspace) line dynamicPoint =
    let positiveStatic = isPositiveStatic line dynamicPoint
    let staticLayers, dynamicLayers =
        workspace.Paper.Layers |> chooseUnzip (splitLayer line positiveStatic)
    dynamicLayers.Reverse()
    staticLayers, dynamicLayers

let foldBack (workspace: IWorkspace) line dynamicPoint =
    let staticLayers, dynamicLayers = splitPaper workspace line dynamicPoint
    workspace.ClearPaper(Seq.append staticLayers dynamicLayers)

let private getTargetLayersCore (paper: IPaper) foldLine method =
    let hintPoint = FoldOperation.getSourcePoint method |> List.tryHead
    let isPositiveStatic = isPositiveStatic foldLine hintPoint
    let layers =
        paper.Layers
        |> Seq.mapi (fun index layer ->
            let positive, negative = splitLayerCore foldLine layer
            if isPositiveStatic
            then { Index = index; Original = layer; Static = positive; Dynamic = negative; IsTarget = false }
            else { Index = index; Original = layer; Static = negative; Dynamic = positive; IsTarget = false })
        |> Seq.rev
        |> Seq.toArray
    let firstLayer =
        option {
            return! option {
                let! OprPoint(_, index) = hintPoint
                let l = layers |> Seq.find (fun sl -> sl.Index = index)
                if l.Dynamic.IsSome then return l
            }
            return! layers |> Array.tryFind (fun item -> item.Static.IsSome && item.Dynamic.IsSome)
        }
    match firstLayer with
    | None -> None
    | Some(firstLayer) ->
        let clusters = clusterLayers layers
        let rec setDynamicLayers firstLayer =
            for l in Seq.find (List.contains firstLayer) clusters do
                l.IsTarget <- true
            let lastIndex = Array.findIndexBack (fun l -> l.IsTarget) layers
            // otherLayers は、自分より下のtargetLayersだけを確認する
            layers.[..lastIndex]
            |> Seq.indexed
            |> Seq.tryFind (fun (i, layer) ->
                not layer.IsTarget && exists {
                    let! otherLayer = layer.Dynamic
                    let! targetLayer = layers.[i + 1..lastIndex]
                    if targetLayer.IsTarget then
                        let! targetLayer = targetLayer.Dynamic
                        return Edge.areOverlap otherLayer.Edges targetLayer.Edges
                })
            // 末尾最適化のため、Option.iter を使わない
            |> function
            | None -> ()
            | Some(_, layer) -> setDynamicLayers layer
        setDynamicLayers firstLayer
        Some(layers)

let foldBackFirst (workspace: IWorkspace) foldLine method =
    match getTargetLayersCore workspace.Paper foldLine method with
    | Some(layers) ->
        let staticLaters =
            layers
            |> Seq.choose (fun l ->
                if l.IsTarget
                then Option.map (createLayer foldLine l.Original false) l.Static
                else Some(l.Original))
        let dynamicLayers =
            layers
            |> Seq.choose (fun l ->
                if l.IsTarget
                then Option.map (createLayer foldLine l.Original true) l.Dynamic
                else None)
        workspace.ClearPaper(Seq.append (Seq.rev staticLaters) dynamicLayers)
    | None -> ()

let getTargetLayers paper foldLine method =
    match getTargetLayersCore paper foldLine method with
    | Some(ls) -> [| for s in ls do if s.IsTarget then s.Original |]
    | None -> array.Empty()
