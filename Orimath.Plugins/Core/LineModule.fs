[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Runtime.CompilerServices.Extension>]
module Orimath.Core.Line
open System.Runtime.CompilerServices
open NearlyEquatable

/// 直線と点の距離を取得します。直線の右側か左側かによって符号が変わります。
[<CompiledName("GetSignedDistance"); Extension>]
let signedDist (line: Line) (point: Point) = line.A * point.X + line.B * point.Y + line.C

/// 直線と点の距離を取得します。
[<CompiledName("GetDistance"); Extension>]
let dist (line: Line) (point: Point) = abs (signedDist line point)

/// 2つの直線が交差する点を求めます。
[<CompiledName("GetCrossPoint"); Extension>]
let cross (line1: Line) (line2: Line) =
    let divider = line1.A * line2.B - line2.A * line1.B
    if divider =~~ 0.0 then
        None
    else
        Some {
            X = !+((line1.B * line2.C - line2.B * line1.C) / divider)
            Y = !-((line1.A * line2.C - line2.A * line1.C) / divider)
        }

/// 直線が指定した点を含んでいるか判定します。
[<CompiledName("Contains"); Extension>]
let contains (line: Line) (point: Point) = dist line point =~~ 0.0

/// 直線上の指定したY値におけるX値を取得します。
[<CompiledName("GetX"); Extension>]
let getX line y = !-((line.B * y + line.C) / line.A)

/// 直線上の指定したX値におけるY値を取得します。
[<CompiledName("GetY"); Extension>]
let getY line x = !-((line.A * x + line.C) / line.B)

/// 直線上の指定したX値における点を取得します。
[<CompiledName("XOf"); Extension>]
let xOf line x = { X = x; Y = getY line x }

/// 直線上の指定したY値における点を取得します。
[<CompiledName("YOf"); Extension>]
let yOf line y = { X = getX line y; Y = y }

/// 点から直線に下した垂線の足を取得します。
[<CompiledName("GetPerpendicularFoot"); Extension>]
let perpFoot (line: Line) (point: Point) =
    let sDist = signedDist line point
    {
        X = point.X - line.A * sDist
        Y = point.Y - line.B * sDist
    }

/// 直線を直線で反転させます。
[<CompiledName("Reflect"); Extension>]
let refl (targetLine: Line)(reflector: Line) =
    let e = 2.0 * (reflector.A * targetLine.A + reflector.B * targetLine.B)
    Line.Create(targetLine.A - reflector.A * e, targetLine.B - reflector.B * e, targetLine.C - reflector.C * e)
