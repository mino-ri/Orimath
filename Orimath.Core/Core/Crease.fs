namespace Orimath.Core
open Orimath
open Orimath.ApproximatelyEquatable

[<Struct>]
type CreaseType = Draft | Crease | MountainFold | ValleyFold


[<Struct; NoEquality; NoComparison>]
type Crease = Crease of creaseType: CreaseType * segment: LineSegment with
    member this.Type = match this with Crease(creaseType, _) -> creaseType
    member this.Segment = match this with Crease(_, segment) -> segment
    member this.Line = this.Segment.Line
    member this.Point1 = this.Segment.Point1
    member this.Point2 = this.Segment.Point2

    override this.ToString() = this.Segment.ToString()


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Crease =
    // let private create creaseType line p1 p2 =
    //     Crease(creaseType, LineSegment.createUnchecked line p1 p2)

    let reverse (crease: Crease) = Crease(crease.Type, LineSegment.reverse crease.Segment)

    let length (crease: Crease) = LineSegment.length crease.Segment
    
    let ofSeg creaseType seg = Crease(creaseType, seg)
    
    let ofSegs creaseType segs = seq { for seg in segs -> Crease(creaseType, seg) }
    
    let fromPoints creaseType points =
        let seg = LineSegment.fromPoints points
        Crease(creaseType, seg)
    
    let withPoints p1 p2 (crease: Crease) = Crease(crease.Type, LineSegment.withPoints p1 p2 crease.Segment)
    
    /// 現在の線分を、指定した直線で反転させます。
    let reflectBy line (crease: Crease) =
        Crease(crease.Type, LineSegment.reflectBy line crease.Segment)
    
    let merge (creases: seq<Crease>) =
        let grouped = creases |> Seq.groupBy (fun s -> Nearly(s.Line))
        let result = ResizeArray<Crease>()
        for line, segs in grouped do
            let getD (s: Point) = if line.Value.YFactor = 0.0 then s.Y else s.X
            let addTail lst target = target :: lst |> List.sortBy (fun (s: Crease) -> getD s.Point1)
            let rec recSelf (lst: Crease list) =
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
                            result[result.Count - 1] <- withPoints target.Point1 s.Point1 target
                            result.Add(s)
                            withPoints s.Point2 target.Point2 target
                            |> addTail tail
                            |> recSelf
                    elif s.Type = target.Type then
                        // 2つの線分が同じタイプである場合
                        result[result.Count - 1] <- withPoints target.Point1 s.Point2 target
                        recSelf tail
                    elif target.Point2 =~ s.Point1 then
                        // 2つの線分が1点で接している場合
                        result.Add(s)
                        recSelf tail
                    elif s.Type < target.Type then
                        // もとの線分のほうが優先度が高い場合
                        withPoints target.Point2 s.Point2 s
                        |> addTail tail
                        |> recSelf
                    else
                        result[result.Count - 1] <- withPoints target.Point1 s.Point1 target
                        result.Add(s)
                        recSelf tail
            segs
            |> Seq.map (fun c -> if getD c.Point1 > getD c.Point2 then reverse c else c)
            |> Seq.sortBy (fun s -> getD s.Point1)
            |> Seq.toList
            |> recSelf
        result :> seq<_>
