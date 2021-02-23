namespace Orimath.Core

type CreaseType =
    | Draft = 0
    | Crease = 1
    | MountainFold = 2
    | ValleyFold = 3


type Crease = { Segment: LineSegment; Type: CreaseType } with
    member this.Line = this.Segment.Line
    member this.Point1 = this.Segment.Point1
    member this.Point2 = this.Segment.Point2
    member this.Length = this.Segment.Length
    override this.ToString() = this.Segment.ToString()

module Crease =
    let ofSeg creaseType seg = { Segment = seg; Type = creaseType }

    let ofSegs creaseType segs =
        seq { for seg in segs -> { Segment = seg; Type = creaseType } }

    /// 現在の線分を、指定した直線で反転させます。
    let reflectBy line (crease: Crease) =
        let p1 = Point.reflectBy line crease.Segment.Point1
        let p2 = Point.reflectBy line crease.Segment.Point2
        { crease with Segment = LineSegment(Line.FromPoints(p1, p2).Value, p1, p2) }
