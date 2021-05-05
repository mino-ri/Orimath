namespace Orimath.Basics.View.Export
open System.IO
open System.Xml.Linq
open System.Windows.Media
open Orimath.Basics.View

type SvgExporter(root: XElement) =
    static let svgns = XNamespace.Get("http://www.w3.org/2000/svg")
    static let xmlns = XAttribute(XName.Get("xmlns"), svgns.NamespaceName)
    static let xelem name (values: obj[]) = XElement(svgns + name, values)
    static let xattr name value = XAttribute(XName.Get(name), value)
    static let convertColor (color: Color) =
        if color.A = 0uy then "transparent" else $"#%02x{color.R}%02x{color.G}%02x{color.B}"
    static let getPenAttrs (pen: ExportPen) =
        [|
            xattr "stroke" (convertColor pen.Color)
            xattr "stroke-width" pen.Thickness
            xattr "stroke-linecap" <|
                match pen.LineCap with
                | PenLineCap.Square -> "square"
                | PenLineCap.Round -> "round"
                | _ -> "butt"
            xattr "stroke-linejoin" <|
                match pen.LineJoin with
                | PenLineJoin.Round -> "round"
                | PenLineJoin.Bevel -> "bevel"
                | _ -> "miter"
            if pen.DashArray.Length > 0 then
                xattr "stroke-dasharray" <|
                    String.concat "," (Seq.map (fun i -> string (i * pen.Thickness)) pen.DashArray)
        |]

    member _.Root = root

    static member ExportToStream(stream: Stream, width, height, action) =
        let exporter = SvgExporter(width, height)
        action exporter
        exporter.Root.Save(stream)

    new(width: int, height: int) =
        SvgExporter(xelem "svg" [|
            xmlns
            xattr "width" width
            xattr "height" height
        |])

    interface IShapeExporter with
        member _.AddLine(point1, point2, pen) =
            root.Add(xelem "line" [|
                xattr "x1" point1.X
                xattr "y1" point1.Y
                xattr "x2" point2.X
                xattr "y2" point2.Y
                getPenAttrs pen
            |])

        member _.AddPolygon(points, decoration) =
            root.Add(xelem "polygon" [|
                xattr "points" <| String.concat " " (points |> Seq.map (fun p -> $"{p.X},{p.Y}"))
                xattr "fill" (convertColor decoration.Fill)
                getPenAttrs decoration.Stroke
            |])
            
        member _.AddPath(startPoint, segments, pen) =
            root.Add(xelem "path" [|
                xattr "d" <| String.concat " " (seq {
                    $"M {startPoint.X} {startPoint.Y}"
                    for seg in segments do
                    match seg with
                    | ExportPathSegment.Line(ScreenPoint(x, y)) -> $"L {x} {y}"
                    | ExportPathSegment.Arc(ScreenPoint(x, y), size, rotationAngle, isLarge, direction) ->
                        let large = if isLarge then 1 else 0
                        let sweep = if direction = SweepDirection.Clockwise then 1 else 0
                        $"A {size.Width} {size.Height} {rotationAngle} {large} {sweep} {x} {y}"
                })
                xattr "fill" "transparent"
                getPenAttrs pen
            |])

        member _.AddEllipse(center, size, decoration) =
            root.Add(xelem "ellipse" [|
                xattr "cx" center.X
                xattr "cy" center.Y
                xattr "rx" size.Width
                xattr "ry" size.Height
                xattr "fill" (convertColor decoration.Fill)
                getPenAttrs decoration.Stroke
            |])

        member _.AddText(text, point, size, color) =
            root.Add(xelem "text" [|
                xattr "x" point.X
                xattr "y" (point.Y + size)
                xattr "fill" (convertColor color)
                xattr "font-family" "Meiryo UI"
                xattr "font-size" size
                text
            |])
