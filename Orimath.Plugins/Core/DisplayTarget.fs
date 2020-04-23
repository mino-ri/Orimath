namespace Orimath.Core

/// ドラッグおよびクリック操作の対象を表します。
[<RequireQualifiedAccess>]
type DisplayTarget =
    /// 選択対象なし
    | None
    /// レイヤー。
    | Layer of Layer
    /// 紙の端や折りたたまれた紙の境界線。
    | Edge of Edge
    /// 折線。
    | Line of LineSegment
    /// 折線の交差などに現れる選択可能な点。
    | Point of Point
