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
    | _ -> None, None

let private splitCrease foldLine (target: Crease) =
    let positive, negative = splitLine foldLine target.Segment
    positive |> Option.map (fun l -> { target with Segment = l }),
    negative |> Option.map (fun l -> { target with Segment = l })

let private splitCreases foldLine (layer: ILayer) =
    layer.Creases |> chooseUnzip (splitCrease foldLine)

let private splitEdge foldLine (target: Edge) =
    let positive, negative = splitLine foldLine target.Segment
    positive |> Option.map (fun l -> { target with Segment = l }),
    negative |> Option.map (fun l -> { target with Segment = l })

let private splitEdges foldLine layer =
    let mutable crossed = false
    let mutable isPositive = None
    let positiveEdges = ResizeArray()
    let negativeEdges = ResizeArray()
    let cross (edge: Edge) positiveToNegative =
        let positiveEdge, negativeEdge =
            match Layer.clipBound foldLine layer with
            | Some(a, b) ->
                swapWhen
                    (LineSegment.containsPoint a edge.Segment = positiveToNegative)
                    (LineSegment.FromPoints(b, a))
                    (LineSegment.FromPoints(a, b))
            | None -> None, None
        iter { let! e = positiveEdge in positiveEdges.Add({ Segment = e; Inner = true }) }
        iter { let! e = negativeEdge in negativeEdges.Add({ Segment = e; Inner = true }) }
    for edge in layer.Edges do
        match splitEdge foldLine edge with
        | Some(positive), None ->
            if not crossed && isPositive = Some(false) then
                crossed <- true
                cross edge false
            positiveEdges.Add(positive)
            isPositive <- Some(true)
        | None, Some(negative) ->
            if not crossed && isPositive = Some(true) then
                crossed <- true
                cross edge true
            negativeEdges.Add(negative)
            isPositive <- Some(false)
        | Some(positive), Some(negative) ->
            if crossed then
                positiveEdges.Add(positive)
                negativeEdges.Add(negative)
            else
                crossed <- true
                // 正 → 負に突入
                if positive.Segment.Point2 = negative.Segment.Point1 then
                    positiveEdges.Add(positive)
                    cross edge true
                    negativeEdges.Add(negative)
                    isPositive <- Some(false)
                // 負 → 正に突入
                else
                    negativeEdges.Add(negative)
                    cross edge false
                    positiveEdges.Add(positive)
                    isPositive <- Some(true)
        | None, None ->
            crossed <- true
            positiveEdges.Add(edge)
            negativeEdges.Add(edge)
    positiveEdges, negativeEdges

let private splitOriginalEdges foldLine positiveEdgesCount (layer: ILayer) =
    let foldLineInOrigin = layer.Matrix.MultiplyInv(foldLine)
    let edges1, edges2 = splitEdges foldLineInOrigin (Layer.original layer)
    match positiveEdgesCount >= 3, edges1.Count >= 3 with
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
    let originalPositiveEdges, originalNegativeEdges = splitOriginalEdges foldLine positiveEdges.Count layer
    let positivePoints, negativePoints = splitPoints foldLine layer
    let positiveCreases, negativeCreases = splitCreases foldLine layer
    let positive =
        if positiveEdges.Count >= 3 then
            Some {
                Edges = Seq.toList positiveEdges
                OriginalEdges = Seq.toList originalPositiveEdges
                Points = Seq.toList positivePoints
                Creases = Seq.toList positiveCreases
            }
        else
            None
    let negative =
        if negativeEdges.Count >= 3 then
            Some {
                Edges = Seq.toList negativeEdges
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
    iter {
        let! layer = layers
        let! dl = layer.Dynamic
        clusteredLayers
        |> Seq.tryFindIndex (fun cluster ->
            exists {
                let! cl = cluster
                let! c = cl.Dynamic
                return isContacted dl.OriginalEdges c.OriginalEdges
            })
        |> function
        | Some(i) -> clusteredLayers.[i] <- layer :: clusteredLayers.[i]
        | None -> clusteredLayers.Add([ layer ])
    }
    clusteredLayers

let foldBack (workspace: IWorkspace) line dynamicPoint =
    let positiveStatic = isPositiveStatic line dynamicPoint
    let staticLayers, dynamicLayers =
        workspace.Paper.Layers |> chooseUnzip (splitLayer line positiveStatic)
    workspace.ClearPaper(Seq.append staticLayers (Seq.rev dynamicLayers))

let private getTargetLayersCore (paper: IPaper) foldLine method =
    let hintPoints = FoldOperation.getSourcePoint method
    let layers =
        paper.Layers
        |> Seq.mapi (fun index layer ->
            let positive, negative = splitLayerCore foldLine layer
            if isPositiveStatic foldLine (List.tryHead hintPoints)
            then { Index = index; Original = layer; Static = positive; Dynamic = negative; IsTarget = false }
            else { Index = index; Original = layer; Static = negative; Dynamic = positive; IsTarget = false })
        |> Seq.rev
        |> Seq.toArray
    let firstLayers =
        [
            for OprPoint(_, index) in hintPoints do
            let l = layers |> Seq.find (fun sl -> sl.Index = index)
            if l.Dynamic.IsSome then l
        ]
        |> function
        | [] ->
            layers
            |> Array.tryFind (fun item -> item.Static.IsSome && item.Dynamic.IsSome)
            |> Option.toList
        | [ t ] -> [ t ]
        | ts ->
            match ts |> Seq.tryFind (fun t -> FoldOperation.isContained t.Original method) with
            | Some(t) -> [ t ]
            | None -> ts
    if firstLayers.IsEmpty then
        None
    else
        let clusters = clusterLayers layers
        let rec setDynamicLayers firstLayers =
            for layer in firstLayers do
                for l in Seq.find (List.contains layer) clusters do
                    l.IsTarget <- true
            let lastIndex = Array.findIndexBack (fun l -> l.IsTarget) layers
            let targetLayers, otherLayers =
                layers
                |> Array.take (lastIndex + 1)
                |> Array.partition (fun layer -> layer.IsTarget)
            otherLayers
            |> Array.tryFind (fun layer ->
                exists {
                    let! otherLayer = layer.Dynamic
                    let! targetLayer = targetLayers
                    let! targetLayer = targetLayer.Dynamic
                    return Edge.areOverlap otherLayer.Edges targetLayer.Edges
                })
            // 末尾最適化のため、Option.iter を使わない
            |> function
            | None -> ()
            | Some(layer) -> setDynamicLayers [ layer ]
        setDynamicLayers firstLayers
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
