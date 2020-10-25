[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Runtime.CompilerServices.Extension>]
module internal Orimath.Core.Layer
open System.Runtime.CompilerServices
open NearlyEquatable

let private ( @@ ) (s : seq<'a>) (lst: 'a list) =
    let mutable r = lst
    for item in s do r <- item :: r
    r

[<CompiledName("Add"); Extension>]
let add (layer: Layer) (lines: seq<LineSegment>) (points: seq<Point>) =
    Layer.Create(layer.Edges, lines @@ layer.Lines, points @@ layer.Points, layer.LayerType)

[<CompiledName("AddPoints"); Extension>]
let addPoints layer points = add layer [] points

[<CompiledName("AddLines"); Extension>]
let addLines layer lines = add layer lines []
