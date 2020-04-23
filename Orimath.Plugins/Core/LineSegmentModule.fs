[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Runtime.CompilerServices.Extension>]
module Orimath.Core.LineSegment
open System.Runtime.CompilerServices
open NearlyEquatable

let private containsX (line: LineSegment) x =
    if line.Point1.X < line.Point2.X then
        line.Point1.X <=~ x && x <=~ line.Point2.X
     else
        line.Point2.X <=~ x && x <=~ line.Point1.X

let private containsY (line: LineSegment) y =
    if line.Point1.Y < line.Point2.Y then
        line.Point1.Y <=~ y && y <=~ line.Point2.Y
     else
        line.Point2.Y <=~ y && y <=~ line.Point1.Y

let private containsCore (line: LineSegment) (point: Point) =
    containsX line point.X && containsY line point.Y

/// 直線が指定した点を含んでいるか判定します。
[<CompiledName("Contains"); Extension>]
let contains (line: LineSegment) (point: Point) =
    containsCore line point && Line.contains line.Line point

/// 線分が指定した線分を含んでいるか判定します。
[<CompiledName("Contains"); Extension>]
let containsSeg (line: LineSegment) (target: LineSegment) =
    line.Line =~ target.Line &&
    containsCore line target.Point1 &&
    containsCore line target.Point2

/// 2つの線分が交差する点を求めます。
[<CompiledName("GetCrossPointOption"); Extension>]
let cross (line1: LineSegment) (line2: LineSegment) =
    Line.cross line1.Line line2.Line
    |> Option.filter(fun p -> containsCore line1 p && containsCore line2 p)

/// 2つの線分が交差する点を求めます。
[<CompiledName("GetCrossPoint"); Extension>]
let crossNullable line1 line2 = cross line1 line2 |> Option.toNullable

/// 直線上の指定したY値におけるX値を取得します。
[<CompiledName("GetXOption"); Extension>]
let getX line y = if containsY line y then Some(Line.getX line.Line y) else None

/// 直線上の指定したX値におけるY値を取得します。
[<CompiledName("GetYOption"); Extension>]
let getY line x = if containsX line x then Some(Line.getY line.Line x) else None

/// 直線上の指定したX値における点を取得します。
[<CompiledName("XOfOption"); Extension>]
let xOf line x = if containsX line x then Some(Line.xOf line.Line x) else None

/// 直線上の指定したY値における点を取得します。
[<CompiledName("YOfOption"); Extension>]
let yOf line y = if containsY line y then Some(Line.yOf line.Line y) else None

/// 直線上の指定したY値におけるX値を取得します。
[<CompiledName("GetX"); Extension>]
let getXNullable line y = getX line y |> Option.toNullable

/// 直線上の指定したX値におけるY値を取得します。
[<CompiledName("GetY"); Extension>]
let getYNullable line x = getY line x |> Option.toNullable

/// 直線上の指定したX値における点を取得します。
[<CompiledName("XOf"); Extension>]
let xOfNullable line x = xOf line x |> Option.toNullable

/// 直線上の指定したY値における点を取得します。
[<CompiledName("YOf"); Extension>]
let yOfNullable line y = yOf line y |> Option.toNullable
