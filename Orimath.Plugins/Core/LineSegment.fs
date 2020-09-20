namespace Orimath.Core
open System.Diagnostics.CodeAnalysis
open NearlyEquatable

type LineSegment internal (line: Line, p1: Point, p2: Point) =
    member _.Line = line
    member _.Point1 = p1
    member _.Point2 = p2
    
    member _.Length = (p1 - p2).Norm

    interface INearlyEquatable<LineSegment> with
        member this.NearlyEquals(other, margin) =
            nearlyEquals margin this.Line other.Line &&
            nearlyEquals margin this.Point1 other.Point1 &&
            nearlyEquals margin this.Point2 other.Point2

    member private this.ContainsX(x) =
        if this.Point1.X < this.Point2.X
        then this.Point1.X <=~ x && x <=~ this.Point2.X
        else this.Point2.X <=~ x && x <=~ this.Point1.X
            
    member private this.ContainsY(y) =
        if this.Point1.Y < this.Point2.Y
        then this.Point1.Y <=~ y && y <=~ this.Point2.Y
        else this.Point2.Y <=~ y && y <=~ this.Point1.Y
            
    member private this.ContainsCore(point) = this.ContainsX(point.X) && this.ContainsY(point.Y)
            
    /// 直線が指定した点を含んでいるか判定します。
    member this.Contains(point) = this.ContainsCore(point) && this.Line.Contains(point)
            
    /// 線分が指定した線分を含んでいるか判定します。
    member this.Contains(target: LineSegment) =
        this.Line =~ target.Line &&
        this.ContainsCore(target.Point1) &&
        this.ContainsCore(target.Point2)
            
    /// 2つの線分が交差する点を求めます。
    [<CompiledName("GetCrossPointOption")>]
    member this.GetCrossPoint(other: LineSegment) =
        this.Line.GetCrossPoint(other.Line)
        |> Option.filter(fun p -> this.ContainsCore(p) && other.ContainsCore(p))
            
    /// 2つの線分が交差する点を求めます。
    [<CompiledName("GetCrossPoint")>]
    member this.GetCrossPointNullable(other) = this.GetCrossPoint(other) |> Option.toNullable
            
    /// 直線上の指定したY値におけるX値を取得します。
    [<CompiledName("GetXFSharp")>]
    member this.GetX(y) = if this.ContainsY(y) then Some(this.Line.GetX(y)) else None
            
    /// 直線上の指定したX値におけるY値を取得します。
    [<CompiledName("GetYFSharp")>]
    member this.GetY(x) = if this.ContainsX(x) then Some(this.Line.GetY(x)) else None
            
    /// 直線上の指定したX値における点を取得します。
    [<CompiledName("XOfFSharp")>]
    member this.XOf(x) = if this.ContainsX(x) then Some(this.Line.XOf(x)) else None
            
    /// 直線上の指定したY値における点を取得します。
    [<CompiledName("YOfFSharp")>]
    member this.YOf(y) = if this.ContainsY(y) then Some(this.Line.YOf(y)) else None
            
    /// 直線上の指定したY値におけるX値を取得します。
    [<CompiledName("GetX")>]
    member this.GetXCSharp(y) = this.GetX(y) |> Option.toNullable
            
    /// 直線上の指定したX値におけるY値を取得します。
    [<CompiledName("GetY")>]
    member this.GetYCSharp(x) = this.GetY(x) |> Option.toNullable
            
    /// 直線上の指定したX値における点を取得します。
    [<CompiledName("XOf")>]
    member this.XOfCSharp(x) = this.XOf(x) |> Option.toNullable
            
    /// 直線上の指定したY値における点を取得します。
    [<CompiledName("YOf")>]
    member this.YOfCSharp(y) = this.YOf(y) |> Option.toNullable

    static member FromFactorsAndPoint(xFactor, yFactor, p) =
        LineSegment(Line.FromFactorsAndPoint(xFactor, yFactor, p), p, p)

    [<CompiledName("FromPointsFSharp")>]
    static member FromPoints(p1, p2) =
        Line.FromPoints(p1, p2)
        |> Option.map(fun line -> LineSegment(line, p1, p2))

    [<CompiledName("FromPoints")>]
    static member FromPointsCSharp(p1, p2) : [<MaybeNull>] LineSegment =
        match LineSegment.FromPoints(p1, p2) with
        | Some(line) -> line
        | None -> Unchecked.defaultof<_>
