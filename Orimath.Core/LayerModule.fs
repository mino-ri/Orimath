[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Runtime.CompilerServices.Extension>]
module Orimath.Core.Layer
open System.Runtime.CompilerServices
open NearlyEquatable

let private ( @@ ) (s : seq<'a>) (lst: 'a list) =
    let mutable r = lst
    for item in s do r <- item :: r
    r

[<CompiledName("AppendCrosses")>]
let private appendCross (layer: Layer) (line: LineSegment) (points: Point list) =
    let mutable points = points
    for edge in layer.Edges do
        match edge.Line.GetCrossPoint(line) with
        | Some(p) when not (points |> List.exists((=~) p)) && not (layer.HasPoint(p)) ->
            points <- p :: points
        | _ -> ()
    points

[<CompiledName("GetCrossesFSharpList"); Extension>]
let cross (layer: Layer) (line: LineSegment) = appendCross layer line []

[<CompiledName("GetCrosses"); Extension>]
let crossArray layer line = cross layer line |> List.toArray

[<CompiledName("AddCore"); Extension>]
let add (layer: Layer) (lines: seq<LineSegment>) (points: seq<Point>) =
    Layer.Create(layer.Edges, lines @@ layer.Lines, points @@ layer.Points)

[<CompiledName("AddPoints"); Extension>]
let addPoints layer points = add layer [] points

[<CompiledName("AddLinesRaw"); Extension>]
let addLinesRaw layer lines = add layer lines []

[<CompiledName("AddLine"); Extension>]
let addLine layer (line: LineSegment) = add layer [line] (cross layer line)
