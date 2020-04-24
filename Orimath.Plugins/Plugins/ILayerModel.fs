namespace Orimath.Plugins
open Orimath.Core

type ILayerModel =
    inherit ILayer
    [<CLIEvent>] abstract member LineChanged : ICollectionChangedEvent<LineSegment>
    [<CLIEvent>] abstract member PointChanged : ICollectionChangedEvent<Point>

    abstract member GetSnapShot : unit -> ILayer
    abstract member AddLines : lines: seq<Line> -> unit
    abstract member AddLines : lines: seq<LineSegment> -> unit
    abstract member AddLinesRaw : lines: seq<Line> -> unit
    abstract member AddLinesRaw : lines: seq<LineSegment> -> unit
    abstract member RemoveLines : count: int -> unit
    abstract member AddPoints : points: seq<Point> -> unit
    abstract member RemovePoints : count: int -> unit
