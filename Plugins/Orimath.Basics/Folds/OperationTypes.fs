namespace Orimath.Basics.Folds
open Orimath.Core

type FoldDirection =
    | PointToLine = 0
    | LineToPoint = 1


type OprPoint = OprPoint of point: Point * layerIndex: int with
    member this.Point = match this with OprPoint(p, _) -> p
    member this.LayerIndex = match this with OprPoint(_, layerIndex) -> layerIndex


type OprLine = OprLine of segment: LineSegment * hintPoint: Point * isEdge: bool * layerIndex: int with
    member this.Segment = match this with OprLine(seg, _, _, _) -> seg
    member this.Line = this.Segment.Line
    member this.HintPoint = match this with OprLine(_, hint, _, _) -> hint
    member this.IsEdge = match this with OprLine(_, _, isEdge, _) -> isEdge
    member this.LayerIndex = match this with OprLine(_, _, _, layerIndex) -> layerIndex


type FoldMethod =
    | NoOperation
    | Axiom1 of hint: OprPoint option * point1: OprPoint * point2: OprPoint
    | Axiom2 of point1: OprPoint * point2: OprPoint
    | Axiom3 of line1: OprLine * line2: OprLine
    | Axiom4 of hint: OprPoint option * line: OprLine * point: OprPoint
    | Axiom5 of pass: OprPoint * line: OprLine * point: OprPoint * direction: FoldDirection
    | Axiom6 of line1: OprLine * point1: OprPoint * line2: OprLine * point2: OprPoint * direction: FoldDirection
    | Axiom7 of pass: OprLine * line: OprLine * point: OprPoint * direction: FoldDirection
    | Axiom2P of line: OprLine * point: OprPoint * direction: FoldDirection
    | Axiom3P of line: OprLine * point: OprPoint * direction: FoldDirection
    | Axiom5M of passCandidates: OprPoint list * line: OprLine * point: OprPoint * direction: FoldDirection

    
type FoldOperation =
    { Method: FoldMethod
      CreaseType: CreaseType
      IsFrontOnly: bool }


type DivideOperation =
    { Method: FoldMethod
      CreaseType: CreaseType
      DivisionNumber: int }
