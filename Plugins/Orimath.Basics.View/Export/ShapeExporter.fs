﻿namespace Orimath.Basics.View.Export
open System.Windows
open System.Windows.Media
open Orimath.Plugins

[<Struct>]
type ExportPen =
    { Color: Color
      Thickness: float
      DashArray: float list
      LineCap: PenLineCap
      DashCap: PenLineCap
      LineJoin: PenLineJoin
    }

    static member Solid(color, thickness) =
        { Color = color
          LineCap = PenLineCap.Round
          DashCap = PenLineCap.Round
          LineJoin = PenLineJoin.Round
          Thickness = thickness
          DashArray = []
        }

[<Struct>]
type ExportDecoration =
    { Stroke: ExportPen
      Fill: Color }


[<RequireQualifiedAccess>]
type ExportPathSegment =
    | Arc of arcTo: ScreenPoint
           * size: Size
           * rotationAngle: float
           * isLarge: bool
           * direction: SweepDirection
    | Line of lineTo: ScreenPoint


type IShapeExporter =
    abstract member AddLine : point1: ScreenPoint * point2: ScreenPoint * pen: ExportPen -> unit
    abstract member AddPolygon : points: seq<ScreenPoint> * decoration: ExportDecoration -> unit
    abstract member AddPath : startPoint: ScreenPoint * segments: seq<ExportPathSegment> * pen: ExportPen -> unit
    abstract member AddEllipse : center: ScreenPoint * size: Size * decoration: ExportDecoration -> unit
    abstract member AddText : text: string * point: ScreenPoint * size: float * color: Color -> unit
