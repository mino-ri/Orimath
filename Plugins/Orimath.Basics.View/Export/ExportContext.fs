namespace Orimath.Basics.View.Export
open System.Windows
open System.Windows.Media
open Orimath.Core
open Orimath.Plugins

type ExportContext(exporter: IShapeExporter, pointConverter: IViewPointConverter) =
    member _.DrawCrease(crease: Crease) =
        exporter.AddLine(
            pointConverter.ModelToView(crease.Point1), 
            pointConverter.ModelToView(crease.Point2),
            Drawing.solidPen Colors.Black 2.0)

    member _.DrawEdge(edge: Edge) =
        exporter.AddLine(
            pointConverter.ModelToView(edge.Point1), 
            pointConverter.ModelToView(edge.Point2),
            Drawing.solidPen Colors.Black 3.0)

    member _.DrawPoint(point: Point) =
        exporter.AddEllipse(
            pointConverter.ModelToView(point),
            Size(2.0, 2.0),
            { Stroke = Drawing.solidPen Colors.Black 1.0
              Fill = Colors.White })

    member _.DrawLayerBack(layer: ILayer) =
        exporter.AddPolygon(
            layer.Edges |> Seq.map (fun e -> pointConverter.ModelToView(e.Point1)),
            { Stroke = Drawing.solidPen Colors.Transparent 0.0
              Fill = if layer.LayerType = LayerType.FrontSide then Colors.Bisque else Colors.White })

    member this.DrawLayer(layer: ILayer) =
        this.DrawLayerBack(layer)
        layer.Edges |> Seq.iter this.DrawEdge
        layer.Creases |> Seq.iter this.DrawCrease
        layer.Points |> Seq.iter this.DrawPoint

    member this.DrawPaper(paper: IPaper) =
        paper.Layers |> Seq.iter this.DrawLayer