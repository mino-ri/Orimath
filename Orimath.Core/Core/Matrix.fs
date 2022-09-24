﻿namespace Orimath.Core
open Orimath

[<NoComparison>]
type Matrix =
    {
        M11: float; M12: float // 0
        M21: float; M22: float // 0
        OffsetX: float; OffsetY: float // 1
    }
    override this.ToString() =
        System.String.Format(
            "[ [{0} {1}] [{2} {3}] [{4} {5}] ]",
            this.M11, this.M12,
            this.M21, this.M22,
            this.OffsetX, this.OffsetY);

module Matrix =
    let identity = 
        { M11 = 1.0; M12 = 0.0
          M21 = 0.0; M22 = 1.0
          OffsetX = 0.0; OffsetY = 0.0 }

    let ofReflection (line: Line) =
        let cosL = line.YFactor * line.YFactor - line.XFactor * line.XFactor
        let sinL = -2.0 * line.XFactor * line.YFactor
        { M11 = cosL
          M12 = sinL
          M21 = sinL
          M22 = -cosL
          OffsetX = -2.0 * line.Intercept * line.XFactor
          OffsetY = -2.0 * line.Intercept * line.YFactor }

    let invert matrix =
        let invDet = 1.0 / (matrix.M11 * matrix.M22 - matrix.M21 * matrix.M12)
        { M11 = matrix.M22 * invDet
          M12 = -matrix.M12 * invDet
          M21 = -matrix.M21 * invDet
          M22 = matrix.M11 * invDet
          OffsetX = (matrix.M21 * matrix.OffsetY - matrix.M22 * matrix.OffsetX) * invDet
          OffsetY = (matrix.M12 * matrix.OffsetX - matrix.M11 * matrix.OffsetY) * invDet }

    let multiplyInv (line: Line) matrix =
        Line(
            matrix.M11 * line.XFactor + matrix.M12 * line.YFactor,
            matrix.M21 * line.XFactor + matrix.M22 * line.YFactor,
            matrix.OffsetX * line.XFactor + matrix.OffsetY * line.YFactor + line.Intercept)

type Matrix with
    static member ( * ) (point: Point, matrix) =
        Point(
            point.X * matrix.M11 + point.Y * matrix.M21 + matrix.OffsetX,
            point.X * matrix.M12 + point.Y * matrix.M22 + matrix.OffsetY)

    static member ( * ) (matxi1, matrix2) =
        { M11 = matxi1.M11 * matrix2.M11 + matxi1.M12 * matrix2.M21
          M12 = matxi1.M11 * matrix2.M12 + matxi1.M12 * matrix2.M22
          M21 = matxi1.M21 * matrix2.M11 + matxi1.M22 * matrix2.M21
          M22 = matxi1.M21 * matrix2.M12 + matxi1.M22 * matrix2.M22
          OffsetX = matxi1.OffsetX * matrix2.M11 + matxi1.OffsetY * matrix2.M21 + matrix2.OffsetX
          OffsetY = matxi1.OffsetX * matrix2.M12 + matxi1.OffsetY * matrix2.M22 + matrix2.OffsetY }
        
    static member ( * ) (lineSegment: LineSegment, matrix: Matrix) =
        let points = NotEqual.create (lineSegment.Point1 * matrix) (lineSegment.Point2 * matrix) |> Result.forceOk
        LineSegment.fromPoints points

    static member ( * ) (crease: Crease, matrix: Matrix) =
        Crease(crease.Type, crease.Segment * matrix)

    static member ( * ) (edge: Edge, matrix: Matrix) =
        Edge(edge.Type, edge.Segment * matrix)
