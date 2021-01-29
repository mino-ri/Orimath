namespace Orimath.Plugins
open Orimath.Core
open ApplicativeProperty

type ILayerModel =
    inherit ILayer
    abstract member Lines : IReactiveCollection<LineSegment>
    abstract member Points : IReactiveCollection<Point>

    abstract member GetSnapShot : unit -> ILayer
    abstract member AddLines : lines: seq<Line> -> unit
    abstract member AddLines : lines: seq<LineSegment> -> unit
    abstract member AddLinesRaw : lines: seq<Line> -> unit
    abstract member AddLinesRaw : lines: seq<LineSegment> -> unit
    abstract member RemoveLines : count: int -> unit
    abstract member AddPoints : points: seq<Point> -> unit
    abstract member RemovePoints : count: int -> unit
