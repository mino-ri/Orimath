module internal Orimath.Basics.View.Export.Drawing
open System.Windows.Media

let toMediaPen (pen: ExportPen) =
    let mediaPen = Pen(SolidColorBrush(pen.Color), pen.Thickness)
    mediaPen.DashStyle <- DashStyle(pen.DashArray, 0.0)
    mediaPen.StartLineCap <- pen.LineCap
    mediaPen.EndLineCap <- pen.LineCap
    mediaPen.DashCap <- pen.DashCap
    mediaPen.LineJoin <- pen.LineJoin
    mediaPen.Freeze()
    mediaPen

let toMediaDecoration (decoration: ExportDecoration) =
    SolidColorBrush(decoration.Fill), toMediaPen decoration.Stroke
