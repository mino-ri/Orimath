namespace Orimath.Basics.View.Export
open System
open System.Windows
open System.Windows.Media
open Orimath.Basics.View
open Orimath.Basics.Folds
open Orimath.Core
open Orimath.Plugins
open Orimath.Combination

type ExportContext(exporter: IShapeExporter, pointConverter: IViewPointConverter) =
    static let sin60 = sin(Math.PI / 3.0)
    static let cos60 = cos(Math.PI / 3.0)
    static let pointMargin = 8.0
    static let arrowSize = 12.0

    member _.DrawCrease(crease: Crease) =
        exporter.AddLine(
            pointConverter.ModelToView(crease.Point1), 
            pointConverter.ModelToView(crease.Point2),
            ExportPen.Solid(Colors.Black, 2.0))

    member _.DrawEdge(edge: Edge) =
        exporter.AddLine(
            pointConverter.ModelToView(edge.Point1), 
            pointConverter.ModelToView(edge.Point2),
            ExportPen.Solid(Colors.Black, 3.0))

    member _.DrawPoint(point: Point) =
        exporter.AddEllipse(
            pointConverter.ModelToView(point),
            Size(2.0, 2.0),
            { Stroke = ExportPen.Solid(Colors.Black, 1.0)
              Fill = Colors.White })

    member _.DrawLayerBack(layer: ILayer) =
        exporter.AddPolygon(
            layer.Edges |> Seq.map (fun e -> pointConverter.ModelToView(e.Point1)),
            { Stroke = ExportPen.Solid(Colors.Transparent, 0.0)
              Fill = if layer.LayerType = LayerType.FrontSide then Colors.Bisque else Colors.White })

    member this.DrawLayer(layer: ILayer) =
        this.DrawLayerBack(layer)
        layer.Edges |> Seq.iter this.DrawEdge
        layer.Creases |> Seq.iter this.DrawCrease
        layer.Points |> Seq.iter this.DrawPoint

    member this.DrawPaper(paper: IPaper) =
        paper.Layers |> Seq.iter this.DrawLayer

    member _.DrawInstructionPoint(point: InstructionPoint) =
        exporter.AddEllipse(
            pointConverter.ModelToView(point.Point),
            Size(12.0, 12.0),
            { Stroke = ExportPen.Solid(Colors.Transparent, 0.0)
              Fill = UniversalColor.getColor point.Color })

    member _.DrawInstructionLine(line: InstructionLine) =
        exporter.AddLine(
            pointConverter.ModelToView(line.Line.Point1), 
            pointConverter.ModelToView(line.Line.Point2),
            { Color = UniversalColor.getColor line.Color
              Thickness = 3.0
              DashArray = [ 4.0; 2.0 ] })

    member _.DrawInstructionArrow(arrow: InstructionArrow) =
        let inline vector p = ScreenPoint.op_Explicit(p) : Vector
        let inline point v = Vector.op_Explicit(v) : ScreenPoint
        let v1 = vector (pointConverter.ModelToView(arrow.Line.Point1))
        let v2 = vector (pointConverter.ModelToView(arrow.Line.Point2))
        let mutable normal = v2 - v1
        normal.Normalize()
        let direction =
            match arrow.Direction with
            | ArrowDirection.Clockwise -> SweepDirection.Clockwise
            | ArrowDirection.Counterclockwise -> SweepDirection.Counterclockwise
            | _ ->
                if normal.X > 0.0
                then SweepDirection.Clockwise
                else SweepDirection.Counterclockwise
        let vertical =
            if direction = SweepDirection.Clockwise
            then Vector(normal.Y, -normal.X)
            else Vector(-normal.Y, normal.X)
        let point1 = v1 + normal * (pointMargin * sin60) + vertical * (pointMargin * cos60)
        let point2 = v2 - normal * (pointMargin * sin60) + vertical * (pointMargin * cos60)
        let distance = (point2 - point1).Length
        let pen =
            { Color = UniversalColor.getColor arrow.Color
              Thickness = 4.0
              DashArray = [] }
        let drawArrow ty (basePoint: Vector) (normal: Vector) (vertical: Vector) =
            match ty with
            | ArrowType.Normal
            | ArrowType.ValleyFold ->
                exporter.AddPolygon(
                    [
                        point basePoint
                        point (basePoint + normal * arrowSize)
                        point (basePoint + normal * (arrowSize * cos60) + vertical * (arrowSize * sin60))
                    ],
                    { Stroke = pen; Fill = pen.Color })
            | ArrowType.MountainFold ->
                exporter.AddPath(
                    point (basePoint + normal * (arrowSize * sin60) + vertical * (arrowSize * cos60)),
                    [
                        ExportPathSegment.Line(point
                            (basePoint + normal * (arrowSize * 1.25 * cos60) + vertical * (arrowSize * 1.25 * sin60)))
                        ExportPathSegment.Line(point basePoint)
                    ],
                    pen)
            | _ -> ()
        drawArrow arrow.StartType point1 normal vertical
        exporter.AddPath(
            point point1,
            [ ExportPathSegment.Arc(point point2, Size(distance, distance), 0.0, false, direction) ],
            pen)
        drawArrow arrow.EndType point2 (-normal) vertical

    member this.DrawFoldOperation(paper: IPaper, opr: FoldOperation) =
        let lines, arrows, points = Instruction.getLineAndArrow paper opr false false
        lines |> Array.iter this.DrawInstructionLine
        points |> Array.iter this.DrawInstructionPoint
        arrows |> Array.iter this.DrawInstructionArrow
