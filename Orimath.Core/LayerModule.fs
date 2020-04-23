﻿[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
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
        | Some(p) when (points |> List.forall((<>~) p)) && not (layer.HasPoint(p)) ->
            points <- p :: points
        | _ -> ()
    points

[<CompiledName("GetCrossesFSharp"); Extension>]
let cross (layer: Layer) (line: LineSegment) = appendCross layer line [line.Point1; line.Point2]

[<CompiledName("GetCrosses"); Extension>]
let crossCSharp layer line = cross layer line |> List.toArray

[<CompiledName("GetCrossesFSharp"); Extension>]
let crossAll (layer: Layer) (lines: seq<LineSegment>) =
    let lines = asList lines
    let mutable points = []
    for line in lines do
        if (points |> List.forall((<>~) line.Point1)) && not (layer.HasPoint(line.Point1)) then points <- line.Point1 :: points
        if (points |> List.forall((<>~) line.Point2)) && not (layer.HasPoint(line.Point2)) then points <- line.Point2 :: points
        points <- appendCross layer line points
    points

[<CompiledName("GetCrosses"); Extension>]
let crossAllCSharp layer lines = crossAll layer lines |> List.toArray

[<CompiledName("Add"); Extension>]
let add (layer: Layer) (lines: seq<LineSegment>) (points: seq<Point>) =
    Layer.Create(layer.Edges, lines @@ layer.Lines, points @@ layer.Points)

[<CompiledName("AddPoints"); Extension>]
let addPoints layer points = add layer [] points

[<CompiledName("AddLines"); Extension>]
let addLines layer lines = add layer lines []
