[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Orimath.Core.Point

/// 点と点の距離を求めます。
let dist (Point(x1, y1)) (Point(x2, y2)) = sqrt ((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2))

/// 点のノルムを求めます。
let norm (p: Point) = sqrt (p.X * p.X + p.Y * p.Y)

/// 直線を鏡として、点を反転させます。
let reflectBy (mirror: Line) (Point(x, y) as point) =
    let sDist = 2.0 * Line.signedDist point mirror
    Point(x - mirror.XFactor * sDist, y - mirror.YFactor * sDist)

let toNonZero p = NonZeroPoint.ofPoint p

let ofNonZero p = NonZeroPoint.toPoint p
