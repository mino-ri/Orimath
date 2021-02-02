module internal Orimath.Basics.Folds.FoldBack
open Orimath.Core
open Orimath.Plugins
open Orimath.Core.NearlyEquatable

let private center = { X = 0.5; Y = 0.5 }

type private LayerSource =
    { Edges: Edge list
      OriginalEdges: Edge list
      Points: Point list
      Lines: LineSegment list }

type private SplittedLayer =
    { Original: ILayerModel
      Static: LayerSource option
      Dynamic: LayerSource option
      mutable IsTarget: bool }

let chooseUnzip (f: 'T -> 'U1 option * 'U2 option) (source: seq<'T>) =
    let leftResult = ResizeArray()
    let rightResult = ResizeArray()
    for item in source do
        let left, right = f item
        Option.iter leftResult.Add left
        Option.iter rightResult.Add right
    leftResult, rightResult

let private splitPoints (foldLine: Line) (layer: ILayer) =
    let positivePoints = ResizeArray()
    let negativePoints = ResizeArray()
    for point in Seq.append layer.Points (layer.GetCrosses(layer.Clip(foldLine))) do
        match foldLine.GetDistanceSign(point) with
        | 1 -> positivePoints.Add(point)
        | -1 -> negativePoints.Add(point)
        | _ ->
            positivePoints.Add(point)
            negativePoints.Add(point)
    positivePoints, negativePoints

let private splitLine (line: Line) (target: LineSegment) =
    match line.GetDistanceSign(target.Point1), line.GetDistanceSign(target.Point2) with
    | 1, 1 | 1, 0 | 0, 1 -> Some(target), None
    | -1, -1 | -1, 0 | 0, -1 -> None, Some(target)
    | 1, -1 ->
        let cross = line.GetCrossPoint(target.Line).Value
        Some(LineSegment.FromPoints(target.Point1, cross).Value),
        Some(LineSegment.FromPoints(cross, target.Point2).Value)
    | -1, 1 ->
        let cross = line.GetCrossPoint(target.Line).Value
        Some(LineSegment.FromPoints(cross, target.Point2).Value),
        Some(LineSegment.FromPoints(target.Point1, cross).Value)
    | _ -> None, None

let private splitLines foldLine (layer: ILayer) =
    layer.Lines |> chooseUnzip (splitLine foldLine)

let private splitEdge line (target: Edge) =
    let positive, negative = splitLine line target.Line
    positive |> Option.map (fun l -> Edge(l, target.Inner)),
    negative |> Option.map (fun l -> Edge(l, target.Inner))

let private splitEdges (foldLine: Line) (layer: ILayer) =
    let mutable crossed = false
    let mutable isPositive = None
    let positiveEdges = ResizeArray()
    let negativeEdges = ResizeArray()
    let cross (edge: Edge) positiveToNegative =
        let positiveEdge, negativeEdge =
            match layer.ClipBound(foldLine) with
            | Some(a, b) ->
                if edge.Line.Contains(a) = positiveToNegative
                then LineSegment.FromPoints(a, b), LineSegment.FromPoints(b, a)
                else LineSegment.FromPoints(b, a), LineSegment.FromPoints(a, b)
            | None -> None, None
        positiveEdge |> Option.iter(fun e -> positiveEdges.Add(Edge(e, true)))
        negativeEdge |> Option.iter(fun e -> negativeEdges.Add(Edge(e, true)))
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
                if positive.Line.Point2 = negative.Line.Point1 then
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
    let edges1, edges2 = splitEdges foldLineInOrigin (layer.GetOriginal())
    match positiveEdgesCount >= 3, edges1.Count >= 3 with
    | false, false -> edges1, edges2
    | true, false | false, true -> edges2, edges1
    | true, true ->
        let edgeSign =
            edges1
            |> Seq.map(fun e -> foldLine.GetDistanceSign(e.Line.Point1 * layer.Matrix))
            |> Seq.find(fun d -> d <> 0)
        if edgeSign > 0 then edges1, edges2 else edges2, edges1

// returns positive-side, negative-side
let private splitLayerCore foldLine layer =
    let positiveEdges, negativeEdges = splitEdges foldLine layer
    let originalPositiveEdges, originalNegativeEdges = splitOriginalEdges foldLine positiveEdges.Count layer
    let positivePoints, negativePoints = splitPoints foldLine layer
    let positiveLines, negativeLines = splitLines foldLine layer
    let positive =
        if positiveEdges.Count >= 3 then
            Some { Edges = Seq.toList positiveEdges
                   OriginalEdges = Seq.toList originalPositiveEdges
                   Points = Seq.toList positivePoints
                   Lines = Seq.toList positiveLines }
        else
            None
    let negative =
        if negativeEdges.Count >= 3 then
            Some { Edges = Seq.toList negativeEdges
                   OriginalEdges = Seq.toList originalNegativeEdges
                   Points = Seq.toList negativePoints
                   Lines = Seq.toList negativeLines }
        else
            None
    positive, negative

let private createLayer (workspace: IWorkspace) (foldLine: Line) (layer: ILayer) turnOver source =
    if not turnOver then
        workspace.CreateLayer(
            source.Edges,
            source.Lines,
            source.Points,
            layer.LayerType,
            source.OriginalEdges,
            layer.Matrix)
    else
        workspace.CreateLayer(
            source.Edges |> Seq.map(fun e -> Edge(e.Line.ReflectBy(foldLine), e.Inner)),
            source.Lines |> Seq.map(fun ls -> ls.ReflectBy(foldLine)),
            source.Points |> Seq.map foldLine.Reflect,
            layer.LayerType.TurnOver(),
            source.OriginalEdges,
            layer.Matrix * Matrix.OfReflection(foldLine))

// returns static-side, dynamic-side
let private splitLayer workspace foldLine isPositiveStatic layer =
    let positive, negative = splitLayerCore foldLine layer
    if isPositiveStatic
    then Option.map (createLayer workspace foldLine layer false) positive,
         Option.map (createLayer workspace foldLine layer true) negative
    else Option.map (createLayer workspace foldLine layer false) negative,
         Option.map (createLayer workspace foldLine layer true) positive

let private isContacted originalEdges1 originalEdge2 =
    let innerEdges (edges: seq<Edge>) = edges |> Seq.filter(fun e -> e.Inner)
    Seq.allPairs (innerEdges originalEdges1) (innerEdges originalEdge2)
    |> Seq.exists(fun (e1, e2) -> e1.Line =~ e2.Line)

let isPositiveStatic (line: Line) dynamicPoint =
    match dynamicPoint with
    | Some(point) -> not (line.IsPositiveSide(point))
    | None -> line.IsPositiveSide(center)

let private setContactedLayers (layers: SplittedLayer[]) firstIndices =
    firstIndices |> Seq.iter(fun i -> layers.[i].IsTarget <- true)
    for srcIndex = 0 to layers.Length - 2 do
        if layers.[srcIndex].IsTarget then
            for dstIndex = 0 to layers.Length - 1 do
                if srcIndex <> dstIndex && not layers.[dstIndex].IsTarget then
                    match layers.[srcIndex].Dynamic, layers.[dstIndex].Dynamic with
                    | Some(src), Some(dst) when isContacted src.OriginalEdges dst.OriginalEdges ->
                        layers.[dstIndex].IsTarget <- true
                    | _ -> ()

let foldBack (workspace: IWorkspace) line dynamicPoint =
    let positiveStatic = isPositiveStatic line dynamicPoint
    let staticLayers, dynamicLayers =
        workspace.Paper.Layers |> chooseUnzip (splitLayer workspace line positiveStatic)
    workspace.Paper.Clear(workspace.CreatePaper(Seq.append staticLayers (Seq.rev dynamicLayers)))

let private getTargetLayersCore (paper: IPaperModel) (line: Line) (dynamicPoint: Point option) (targets: OperationTarget list) =
    let layers =
        paper.Layers
        |> Seq.map(fun layer ->
            let positive, negative = splitLayerCore line layer
            if  isPositiveStatic line dynamicPoint
            then { Original = layer; Static = positive; Dynamic = negative; IsTarget = false }
            else { Original = layer; Static = negative; Dynamic = positive; IsTarget = false })
        |> Seq.rev
        |> Seq.toArray
    let getIndex t = paper.Layers.Count - 1 - (paper.Layers |> Seq.findIndex ((=) t.Layer))
    let contains (layer: ILayerModel) (target: OperationTarget) =
        match target.Target with
        | DisplayTarget.Edge(e) -> layer.Contains(e.Line)
        | DisplayTarget.Line(l) -> layer.Contains(l)
        | DisplayTarget.Point(p) -> layer.Contains(p)
        | _ -> false
    let firstIndices =
        match targets with
        | [] ->
            layers
            |> Array.tryFindIndex(fun item -> item.Static.IsSome && item.Dynamic.IsSome)
            |> Option.toList
        | [t] -> [getIndex t]
        | ts ->
            match ts |> List.tryFind(fun t -> List.forall (contains t.Layer) ts) with
            | Some(t) -> [getIndex t]
            | None -> ts |> List.map getIndex
    match firstIndices with
    | [] -> None
    | firstIndices ->
        let rec setDynamicLayers (indices: seq<int>) =
            setContactedLayers layers indices
            let lastIndex = Array.findIndexBack (fun l -> l.IsTarget) layers
            let mergedLine =
                layers
                |> Seq.filter(fun l -> l.IsTarget)
                |> Seq.choose(fun l -> l.Dynamic)
                |> Seq.collect(fun dl -> dl.Edges.Clip(line))
                |> LineSegmentExtensions.Merge
                |> Seq.toList
            seq { 0..lastIndex }
            |> Seq.tryFind(fun i ->
                not layers.[i].IsTarget &&
                layers.[i].Dynamic |> Option.exists(fun dl ->
                    mergedLine |> List.exists(fun l -> not (Seq.isEmpty (dl.Edges.Clip(l))))))
            // 末尾最適化のため、Option.iter を使わない
            |> function
            | None -> ()
            | Some(ix) -> setDynamicLayers [|ix|]
        setDynamicLayers firstIndices
        Some(layers)

let foldBackFirst (workspace: IWorkspace) line dynamicPoint targets =
    match getTargetLayersCore workspace.Paper line dynamicPoint targets with
    | Some(layers) ->
        let staticLaters = layers |> Seq.choose(fun l ->
            if l.IsTarget
            then l.Static |> Option.map(createLayer workspace line l.Original false)
            else Some(upcast l.Original))
        let dynamicLayers = layers |> Seq.choose(fun l ->
            if l.IsTarget
            then l.Dynamic |> Option.map(createLayer workspace line l.Original true)
            else None)
        workspace.Paper.Clear(workspace.CreatePaper(Seq.append (Seq.rev staticLaters) dynamicLayers))
    | None -> ()

let getTargetLayers (workspace: IWorkspace) line dynamicPoint targets =
    match getTargetLayersCore workspace.Paper line dynamicPoint targets with
    | Some(ls) ->
        ls
        |> Seq.filter(fun s -> s.IsTarget)
        |> Seq.map(fun s -> s.Original)
        |> Seq.toArray
    | None -> System.Array.Empty()
