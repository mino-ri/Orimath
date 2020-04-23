namespace Orimath.Core

type Edge internal (line: LineSegment, layer: Layer option) =
    member __.Line = line
    member __.Layer = layer

and Layer internal (edges: Edge list, lines: LineSegment list, points: Point list) =
    member __.Edges = edges
    member __.Lines = lines
    member __.Points = points

type Paper internal (layers: Layer list) =
    member __.Layers = layers
