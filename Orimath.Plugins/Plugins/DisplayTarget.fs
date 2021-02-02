namespace Orimath.Plugins
open Orimath.Core

/// ドラッグおよびクリック操作の対象オブジェクトを表します。
[<RequireQualifiedAccess>]
type DisplayTarget =
    /// レイヤー。
    | Layer of ILayerModel
    /// 紙の端や折りたたまれた紙の境界線。
    | Edge of Edge
    /// 折線。
    | Line of LineSegment
    /// 折線の交差などに現れる選択可能な点。
    | Point of Point

/// ドラッグおよびクリック操作の対象と位置を表します。
type OperationTarget =
    {
        /// 操作対象の位置。
        Point: Point
        /// 操作対称が存在するレイヤー。
        Layer: ILayerModel
        /// 操作対象のオブジェクト。
        Target: DisplayTarget
    }
