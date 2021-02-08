namespace Orimath.Core
open System.Runtime.CompilerServices
open NearlyEquatable

type LineSegment internal (line: Line, p1: Point, p2: Point) =
    member _.Line = line
    member _.Point1 = p1
    member _.Point2 = p2
    member _.Length = (p1 - p2).Norm

    override _.ToString() = System.String.Format("{0}, {1}", p1, p2)

    interface INearlyEquatable<LineSegment> with
        member this.NearlyEquals(other, margin) =
            nearlyEquals margin this.Line other.Line &&
            (nearlyEquals margin this.Point1 other.Point1 && nearlyEquals margin this.Point2 other.Point2 ||
             nearlyEquals margin this.Point1 other.Point2 && nearlyEquals margin this.Point2 other.Point1)

    static member FromFactorsAndPoint(xFactor, yFactor, p) =
        LineSegment(Line.FromFactorsAndPoint(xFactor, yFactor, p), p, p)

    static member FromPoints(p1, p2) =
        Line.FromPoints(p1, p2)
        |> Option.map (fun line -> LineSegment(line, p1, p2))
            

[<Extension>]
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module LineSegment =
    let private containsX x (seg: LineSegment) =
        if seg.Point1.X < seg.Point2.X
        then seg.Point1.X <=~ x && x <=~ seg.Point2.X
        else seg.Point2.X <=~ x && x <=~ seg.Point1.X
            
    let private containsY y (seg: LineSegment) =
        if seg.Point1.Y < seg.Point2.Y
        then seg.Point1.Y <=~ y && y <=~ seg.Point2.Y
        else seg.Point2.Y <=~ y && y <=~ seg.Point1.Y

    let private containsCore point seg = containsX point.X seg && containsY point.Y seg
        
    /// 直線が指定した点を含んでいるか判定します。
    let containsPoint point seg = containsCore point seg && Line.contains point seg.Line
        
    /// 線分が指定した線分を含んでいるか判定します。
    let containsSeg (target: LineSegment) (seg: LineSegment) =
        seg.Line =~ target.Line &&
        containsCore target.Point1 seg && containsCore target.Point2 seg
        
    /// 線分が指定した線分と同じ向きで、かつ共有部分を持つか判定します。
    let hasIntersection (target: LineSegment) (seg: LineSegment) =
        seg.Line =~ target.Line &&
        (containsCore target.Point1 seg || containsCore target.Point2 seg ||
         containsCore seg.Point1 target || containsCore seg.Point2 target)

    /// 2つの線分が交差する点を求めます。
    let cross (seg1: LineSegment) (seg2: LineSegment) =
        Line.cross seg1.Line seg2.Line
        |> Option.filter (fun p -> containsCore p seg1 && containsCore p seg2)
        
    /// 直線上の指定したY値におけるX値を取得します。
    let getX y seg = if containsY y seg then Some(Line.getX y seg.Line) else None
        
    /// 直線上の指定したX値におけるY値を取得します。
    let getY x seg = if containsX x seg then Some(Line.getY x seg.Line) else None
        
    /// 直線上の指定したX値における点を取得します。
    let xOf x seg = if containsX x seg then Some(Line.xOf x seg.Line) else None
        
    /// 直線上の指定したY値における点を取得します。
    let yOf y seg = if containsY y seg then Some(Line.yOf y seg.Line) else None
        
    /// 現在の線分を、指定した直線で反転させます。
    let reflectBy line (seg: LineSegment) =
        let p1 = Point.reflectBy line seg.Point1
        let p2 = Point.reflectBy line seg.Point2
        LineSegment(Line.FromPoints(p1, p2).Value, p1, p2)

    let merge (lineSegments: seq<LineSegment>) =
        let grouped = lineSegments |> Seq.groupBy (fun s -> Nearly(s.Line))
        let result = ResizeArray<LineSegment>()
        for line, segs in grouped do
            let getD = if line.Value.YFactor = 0.0 then (fun s -> s.Y) else (fun s -> s.X)
            segs
            |> Seq.map (fun s ->
                if getD s.Point1 > getD s.Point2
                then LineSegment(line.Value, s.Point2, s.Point1)
                else s)
            |> Seq.sortBy (fun s -> getD s.Point1)
            |> Seq.iter (fun s ->
                if result.Count > 0 && hasIntersection s result.[result.Count - 1] then
                    if getD s.Point2 > getD result.[result.Count - 1].Point2 then
                        result.[result.Count - 1] <- LineSegment(line.Value, result.[result.Count - 1].Point1, s.Point2)
                else
                    result.Add(s))
        result :> seq<_>
