namespace Orimath.Plugins
open Folding

type LineSegment internal (line: Line, p1: Point, p2: Point) =
    member __.Line = line
    member __.Point1 = p1
    member __.Point2 = p2
    
    static member FromPoints(p1, p2) = axiom2 p1 p2 |> Option.map(fun line -> LineSegment(line, p1, p2))
