namespace Orimath.Core
open System.Diagnostics.CodeAnalysis
open NearlyEquatable

type LineSegment internal (line: Line, p1: Point, p2: Point) =
    member __.Line = line
    member __.Point1 = p1
    member __.Point2 = p2
    
    interface INearlyEquatable<LineSegment> with
        member this.NearlyEquals(other, margin) =
            nearlyEquals margin this.Line other.Line &&
            nearlyEquals margin this.Point1 other.Point1 &&
            nearlyEquals margin this.Point2 other.Point2

    [<CompiledName("FromPointsOption")>]
    static member FromPoints(p1, p2) =
        Line.FromPoints(p1, p2)
        |> Option.map(fun line -> LineSegment(line, p1, p2))

    [<CompiledName("FromPoints")>]
    static member FromPointsNullable(p1, p2) : [<MaybeNull>] LineSegment =
        match LineSegment.FromPoints(p1, p2) with
        | Some(line) -> line
        | None -> Unchecked.defaultof<_>
