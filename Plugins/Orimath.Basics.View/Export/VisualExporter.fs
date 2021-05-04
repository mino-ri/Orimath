namespace Orimath.Basics.View.Export
open System
open System.Globalization
open System.Windows
open System.Windows.Media
open System.Windows.Media.Imaging

type VisualExporter(visual: DrawingVisual) =
    let dc = visual.RenderOpen()

    static member ExportPngToStream(stream, width, height, action) =
        let bitmap = VisualExporter.ExportToBitmap(width, height, action)
        let encoder = PngBitmapEncoder()
        encoder.Frames.Add(BitmapFrame.Create(bitmap :> BitmapSource))
        encoder.Save(stream)

    static member ExportToBitmap(width, height, action) =
        let visual = DrawingVisual()
        using (new VisualExporter(visual)) action
        let bitmap = RenderTargetBitmap(width, height, 96.0, 96.0, PixelFormats.Pbgra32)
        bitmap.Render(visual)
        bitmap.Freeze()
        bitmap

    interface IShapeExporter with
        member _.AddLine(point1, point2, pen) =
            dc.DrawLine(Drawing.toMediaPen pen, point1, point2)

        member _.AddPolygon(points, decoration) =
            match Seq.toList points with
            | head :: tail when tail.Length >= 2 ->
                let geometry = PathGeometry([ PathFigure(head, [ PolyLineSegment(tail, true) ], true) ])
                let brush, pen = Drawing.toMediaDecoration decoration
                dc.DrawGeometry(brush, pen, geometry)
            | _ -> ()
            
        member _.AddPath(startPoint, segments, pen) =
            let geometry = StreamGeometry()
            using (geometry.Open()) <| fun context ->
                context.BeginFigure(startPoint, false, false)
                for seg in segments do
                    match seg with
                    | ExportPathSegment.Arc(arcTo, size, rotationAngle, isLarge, direction) ->
                        context.ArcTo(arcTo, size, rotationAngle, isLarge, direction, true, false)
                    | ExportPathSegment.Line(lineTo) ->
                        context.LineTo(lineTo, true, false)
            dc.DrawGeometry(null, Drawing.toMediaPen pen, geometry)

        member _.AddEllipse(center, size, decoration) =
            let brush, pen = Drawing.toMediaDecoration decoration
            dc.DrawEllipse(brush, pen, center, size.Width, size.Height)

        member _.AddText(text, point, size, color) =
            let text =
                FormattedText(
                    text,
                    CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    Typeface("Meiryo UI"),
                    size,
                    SolidColorBrush(color),
                    1.0)
            dc.DrawText(text, point)

    interface IDisposable with
        member _.Dispose() = (dc :> IDisposable).Dispose()
