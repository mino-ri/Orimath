module internal Orimath.Basics.View.Export.Drawing
open System.Windows.Media

let toMediaPen (pen: ExportPen) =
    let mediaPen = Pen(SolidColorBrush(pen.Color), pen.Thickness)
    mediaPen.DashStyle <- DashStyle(pen.DashArray, 0.0)
    mediaPen

let toMediaDecoration (decoration: ExportDecoration) =
    SolidColorBrush(decoration.Fill), toMediaPen decoration.Stroke

let solidPen color thickness =
    { Color = color; Thickness = thickness; DashArray = [] }
