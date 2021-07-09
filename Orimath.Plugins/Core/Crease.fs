namespace Orimath.Core
open NearlyEquatable

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

    let merge (creases: seq<Crease>) =
        let grouped = creases |> Seq.groupBy (fun s -> Nearly(s.Line))
        let result = ResizeArray<Crease>()
        for line, segs in grouped do
            let getD = if line.Value.YFactor = 0.0 then (fun s -> s.Y) else (fun s -> s.X)
            segs
            |> Seq.map (fun c ->
                if getD c.Point1 > getD c.Point2
                then {c with Segment = LineSegment(line.Value, c.Point2, c.Point1) }
                else c)
            |> Seq.sortBy (fun s -> getD s.Point1)
            |> Seq.iter (fun s ->
                if result.Count > 0 && LineSegment.hasIntersection s.Segment result.[result.Count - 1].Segment then
                    let target = result.[result.Count - 1]
                    if getD s.Point2 > getD target.Point2 then
                        let creaseType =
                            match s.Type, target.Type with
                            | a, b when a = b -> a
                            | CreaseType.Crease, _
                            | _, CreaseType.Crease -> CreaseType.Crease
                            | c, _ -> c
                        result.[result.Count - 1] <-
                            { Type = creaseType
                              Segment = LineSegment(line.Value, target.Point1, s.Point2) }
                else
                    result.Add(s))
        result :> seq<_>
