module internal Orimath.Basics.View.Export.Drawing
open System.Windows.Media

let toMediaPen (pen: ExportPen) =
    let mediaPen = Pen(SolidColorBrush(pen.Color), pen.Thickness)
    mediaPen.DashStyle <- DashStyle(pen.DashArray, 0.0)
    mediaPen.StartLineCap <- PenLineCap.Flat
    mediaPen.EndLineCap <- PenLineCap.Flat
    mediaPen.DashCap <- PenLineCap.Flat
    mediaPen.LineJoin <- PenLineJoin.Miter
    mediaPen.Freeze()
    mediaPen

let toMediaDecoration (decoration: ExportDecoration) =
    SolidColorBrush(decoration.Fill), toMediaPen decoration.Stroke
