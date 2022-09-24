namespace Orimath.Core
open Orimath
open Orimath.ApproximatelyEquatable

[<NoEquality; NoComparison>]
type LineSegment = private LineSegment of line: Line * point1: Point * point2: Point with
    member this.Line = match this with LineSegment(line, _, _) -> line
    member this.Point1 = match this with LineSegment(_, point1, _) -> point1
    member this.Point2 = match this with LineSegment(_, _, point2) -> point2

    override this.ToString() = System.String.Format("{0}, {1}", this.Point1, this.Point2)

    interface IApproximatelyEquatable<LineSegment> with
        member this.ApproximatelyEquals(other, margin) =
            aprxEqual margin this.Line other.Line &&
            (aprxEqual margin this.Point1 other.Point1 &&
             aprxEqual margin this.Point2 other.Point2 ||
             aprxEqual margin this.Point1 other.Point2 &&
             aprxEqual margin this.Point2 other.Point1)


[<AutoOpen>]
module LineSegmentOperator =
    let (|LineSegment|) (LineSegment(line, point1, point2)) = struct (line, point1, point2)


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module LineSegment =
    let internal createUnchecked line point1 point2 = LineSegment(line, point1, point2)

    /// 傾きと通る点から、1点に縮退した線分を生成します。
    let fromFactorsAndPoint factors p =
        LineSegment(Line.fromFactorsAndPoint factors p, p, p)

    /// 2点を両端とする線分を生成します。
    let fromPoints (NotEqual(p1, p2) as points) =
        LineSegment(Line.fromPoints points, p1, p2)

    let tryFromPoints (p1: Point) (p2: Point) =
        result {
            let! points = NotEqual.create p1 p2
            return fromPoints points
        }

    /// 線分の長さを取得します。
    let length (seg: LineSegment) = Point.dist seg.Point1 seg.Point2

    /// 線分の終点と視点を指定した点に変更します。
    let withPoints p1 p2 (seg: LineSegment) = LineSegment(seg.Line, p1, p2)

    /// 線分の始点と終点を入れ替えます。
    let reverse (seg: LineSegment) = LineSegment(seg.Line, seg.Point2, seg.Point1)

    let private containsX x (seg: LineSegment) =
        if seg.Point1.X < seg.Point2.X
        then seg.Point1.X <=~ x && x <=~ seg.Point2.X
        else seg.Point2.X <=~ x && x <=~ seg.Point1.X
        
    let private containsY y (seg: LineSegment) =
        if seg.Point1.Y < seg.Point2.Y
        then seg.Point1.Y <=~ y && y <=~ seg.Point2.Y
        else seg.Point2.Y <=~ y && y <=~ seg.Point1.Y

    let private containsCore (point: Point) seg = containsX point.X seg && containsY point.Y seg
    
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
        |> ValueOption.filter (fun p -> containsCore p seg1 && containsCore p seg2)
    
    /// 直線上の指定したY値におけるX値を取得します。
    let getX y seg = if containsY y seg then ValueSome(Line.getX y seg.Line) else ValueNone
    
    /// 直線上の指定したX値におけるY値を取得します。
    let getY x seg = if containsX x seg then ValueSome(Line.getY x seg.Line) else ValueNone
    
    /// 直線上の指定したX値における点を取得します。
    let xOf x seg = if containsX x seg then ValueSome(Line.xOf x seg.Line) else ValueNone
    
    /// 直線上の指定したY値における点を取得します。
    let yOf y seg = if containsY y seg then ValueSome(Line.yOf y seg.Line) else ValueNone

    /// 現在の線分を、指定した直線で反転させます。
    let reflectBy line (seg: LineSegment) =
        let line = Line.reflectBy line seg.Line
        let p1 = Point.reflectBy line seg.Point1
        let p2 = Point.reflectBy line seg.Point2
        LineSegment(line, p1, p2)

    let private mergeCore (Nearly(line: Line), segs: seq<LineSegment>) =
        let inline getD (s: Point) = if line.YFactor = 0.0 then s.Y else s.X
        segs
        |> Seq.map (fun s ->
            if getD s.Point1 > getD s.Point2
            then LineSegment(line, s.Point2, s.Point1)
            else s)
        |> Seq.sortBy (fun s -> getD s.Point1)
        |> Seq.fold (fun acm s ->
            match acm with
            | head :: tail when hasIntersection s head ->
                if getD s.Point2 > getD head.Point2 then
                    // s と head の合成が head よりも長いので更新する
                    LineSegment(line, head.Point1, s.Point2) :: tail
                else
                    // s が head に完全に含まれているので更新しない
                    acm
            | _ ->
                // s と head が交わりを持たないため先頭に追加する
                s :: acm) []

    /// 指定した線分を可能な限り統合します。
    let merge (lineSegments: seq<LineSegment>) =
        lineSegments
        |> Seq.groupBy (fun s -> Nearly(s.Line))
        |> Seq.collect mergeCore
