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
            let addTail lst target = target :: lst |> List.sortBy (fun (s: Crease) -> getD s.Point1)
            let rec recSelf lst =
                match lst with
                | [] -> ()
                | s :: tail ->
                    let target =
                        if result.Count = 0
                        then Unchecked.defaultof<_>
                        else result[result.Count - 1]
                    if result.Count = 0 || not (LineSegment.hasIntersection s.Segment target.Segment) then
                        // 2つの線分に共通部分がない場合
                        result.Add(s)
                        recSelf tail
                    elif getD target.Point2 >= getD s.Point2 then
                        // 一方の線分が他方を完全に含んでいる場合
                        if s.Type <= target.Type then
                            recSelf tail
                        else
                            result[result.Count - 1] <-
                                { target with Segment = LineSegment(line.Value, target.Point1, s.Point1) }
                            result.Add(s)
                            { target with Segment = LineSegment(line.Value, s.Point2, target.Point2) }
                            |> addTail tail
                            |> recSelf
                    elif s.Type = target.Type then
                        // 2つの線分が同じタイプである場合
                        result[result.Count - 1] <-
                            { target with Segment = LineSegment(line.Value, target.Point1, s.Point2) }
                        recSelf tail
                    elif target.Point2 =~ s.Point1 then
                        // 2つの線分が1点で接している場合
                        result.Add(s)
                        recSelf tail
                    elif s.Type < target.Type then
                        // もとの線分のほうが優先度が高い場合
                        { s with Segment = LineSegment(line.Value, target.Point2, s.Point2) }
                        |> addTail tail
                        |> recSelf
                    else
                        result[result.Count - 1] <-
                            { target with Segment = LineSegment(line.Value, target.Point1, s.Point1) }
                        result.Add(s)
                        recSelf tail
            segs
            |> Seq.map (fun c ->
                if getD c.Point1 > getD c.Point2
                then { c with Segment = LineSegment(line.Value, c.Point2, c.Point1) }
                else c)
            |> Seq.sortBy (fun s -> getD s.Point1)
            |> Seq.toList
            |> recSelf
        result :> seq<_>
