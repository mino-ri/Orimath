[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
[<System.Runtime.CompilerServices.Extension>]
module Orimath.Core.Point
open System.Runtime.CompilerServices

/// 新しいPointオブジェクトを生成します。
[<CompiledName("Create")>]
let create x y = { X = x; Y = y }

/// 2点の距離を取得します。
[<CompiledName("GetDistance"); Extension>]
let dist (p1: Point) (p2: Point) = sqrt ((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y))

/// 点を直線で反転させます。
[<CompiledName("Reflect"); Extension>]
let refl (point: Point) (reflector: Line) =
    let sDist = 2.0 * Line.signedDist reflector point
    {
        X = point.X - reflector.A * sDist
        Y = point.Y - reflector.B * sDist
    }
