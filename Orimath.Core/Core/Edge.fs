namespace Orimath.Core

[<Struct>]
type EdgeType = Outside | Inside


[<Struct; NoEquality; NoComparison>]
type Edge = Edge of edgeType: EdgeType * segment: LineSegment with
    member this.Type = match this with Edge(edgeType, _) -> edgeType
    member this.Segment = match this with Edge(_, segment) -> segment
    member this.IsInside = this.Type = EdgeType.Inside
    member this.IsOutside = this.Type = EdgeType.Outside
    member this.Line = this.Segment.Line
    member this.Point1 = this.Segment.Point1
    member this.Point2 = this.Segment.Point2
    override this.ToString() = this.Segment.ToString()


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Edge =
    let length (edge: Edge) = LineSegment.length edge.Segment
    
    /// 現在の線分を、指定した直線で反転させます。
    let reflectBy line (edge: Edge) = Edge(edge.Type, LineSegment.reflectBy line edge.Segment)
    