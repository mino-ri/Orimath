namespace Orimath.Plugins
open Orimath.Core
open ApplicativeProperty

type ILayerModel =
    inherit ILayer
    abstract member Creases : IReactiveCollection<Crease>
    abstract member Points : IReactiveCollection<Point>

    abstract member GetSnapShot : unit -> ILayer
    abstract member AddCreases : lines: seq<Line> -> unit
    abstract member AddCreases : lines: seq<LineSegment> -> unit
    abstract member AddCreases : lines: seq<Crease> -> unit
    abstract member AddCreasesRaw : lines: seq<Line> -> unit
    abstract member AddCreasesRaw : lines: seq<LineSegment> -> unit
    abstract member AddCreasesRaw : lines: seq<Crease> -> unit
    abstract member RemoveCreases : count: int -> unit
    abstract member AddPoints : points: seq<Point> -> unit
    abstract member RemovePoints : count: int -> unit
