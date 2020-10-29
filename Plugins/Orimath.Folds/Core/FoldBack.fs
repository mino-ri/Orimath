module internal Orimath.Folds.Core.FoldBack
open Orimath.Core
open Orimath.Plugins

let private center = { X = 0.5; Y = 0.5 }

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

let private splitEdge (line: Line) (target: Edge) =
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

let private splitLayer (workspace: IWorkspace) (foldLine: Line) (isPositiveStatic: bool) (layer: ILayer) =
    let turnLayerType isPositive =
        if isPositive = isPositiveStatic then layer.LayerType
        else
            match layer.LayerType with
            | LayerType.BackSide -> LayerType.FrontSide
            | LayerType.FrontSide -> LayerType.BackSide
            | _ -> layer.LayerType
    let positiveEdges, negativeEdges = splitEdges foldLine layer
    let originalPositiveEdges, originalNegativeEdges =
        let foldLineInOrigin = layer.Matrix.MultiplyInv(foldLine)
        let edges1, edges2 = splitEdges foldLineInOrigin (layer.GetOriginal())
        match positiveEdges.Count >= 3, edges1.Count >= 3 with
        | false, false -> edges1, edges2
        | true, false | false, true -> edges2, edges1
        | true, true ->
            let edgeSign =
                edges1
                |> Seq.map(fun e -> foldLine.GetDistanceSign(e.Line.Point1 * layer.Matrix))
                |> Seq.find(fun d -> d <> 0)
            if edgeSign > 0 then edges1, edges2 else edges2, edges1
    let positivePoints = ResizeArray()
    let negativePoints = ResizeArray()
    for point in Seq.append layer.Points (layer.GetCrosses(layer.Clip(foldLine))) do
        match foldLine.GetDistanceSign(point) with
        | 1 -> positivePoints.Add(point)
        | -1 -> negativePoints.Add(point)
        | _ ->
            positivePoints.Add(point)
            negativePoints.Add(point)
    let positiveLines = ResizeArray()
    let negativeLines = ResizeArray()
    for line in layer.Lines do
        let positiveLine, negativeLine = splitLine foldLine line
        positiveLine |> Option.iter positiveLines.Add
        negativeLine |> Option.iter negativeLines.Add
    let createLayer (edges: ResizeArray<Edge>) original lines points isPositive =
        if edges.Count < 3 then None
        else
            if isPositive = isPositiveStatic then
                workspace.CreateLayer(
                    edges,
                    lines,
                    points,
                    turnLayerType isPositive,
                    original,
                    layer.Matrix)
            else
                workspace.CreateLayer(
                    edges |> Seq.map(fun e -> Edge(e.Line.ReflectBy(foldLine), e.Inner)),
                    lines |> Seq.map(fun ls -> ls.ReflectBy(foldLine)),
                    points |> Seq.map foldLine.Reflect,
                    turnLayerType isPositive,
                    original,
                    layer.Matrix * Matrix.OfReflection(foldLine))
            |> Some
    if isPositiveStatic then
        let positiveLayer = createLayer positiveEdges originalPositiveEdges positiveLines positivePoints true
        let negativeLayer = createLayer negativeEdges originalNegativeEdges negativeLines negativePoints false
        positiveLayer, negativeLayer
    else
        let negativeLayer = createLayer negativeEdges originalNegativeEdges negativeLines negativePoints false
        let positiveLayer = createLayer positiveEdges originalPositiveEdges positiveLines positivePoints true
        negativeLayer, positiveLayer

let foldBack (workspace: IWorkspace) (line: Line) =
    let staticLayers = ResizeArray()
    let dynamicLayers = ResizeArray()
    let isPositiveStatic = line.IsPositiveSide(center)
    for layer in workspace.Paper.Layers do
        let staticLayer, dynamicLayer = splitLayer workspace line isPositiveStatic layer
        staticLayer |> Option.iter staticLayers.Add
        dynamicLayer |> Option.iter dynamicLayers.Add
    workspace.Paper.Clear(workspace.CreatePaper(Seq.append staticLayers (Seq.rev dynamicLayers)))
