module Orimath.Basics.Folds.Fold
open Orimath.Core

let axiom1 p1 p2 =
    Line.FromPoints(p1, p2)

let axiom2 p1 p2 =
    if p1 = p2 then None
    else
        Line.Create(
            p1.X - p2.X,
            p1.Y - p2.Y,
            (p2.X * p2.X + p2.Y * p2.Y - p1.X * p1.X - p1.Y * p1.Y) / 2.0)
        |> Some

let axiom3 (line1: Line) (line2: Line) =
    let isParallel = line1.Slope = line2.Slope
    if isParallel && line1.Intercept = line2.Intercept then []
    else
        let result1 =
            Line.Create(
                line1.XFactor + line2.XFactor,
                line1.YFactor + line2.YFactor,
                line1.Intercept + line2.Intercept)
        if isParallel then
            [ result1 ]
        else
            let result2 =
                Line.Create(
                    line1.XFactor - line2.XFactor,
                    line1.YFactor - line2.YFactor,
                    line1.Intercept - line2.Intercept)
            [ result1; result2 ]

let axiom4 (line: Line) point =
    Line.Create(-line.YFactor, line.XFactor, line.YFactor * point.X - line.XFactor * point.Y)

let axiom5 pass (ontoLine: Line) ontoPoint =
    if  Line.contains ontoPoint ontoLine then []
    else
        let n = (ontoLine.XFactor * pass.X + ontoLine.YFactor * pass.Y + ontoLine.Intercept)
        let dist = (pass.X - ontoPoint.X) * (pass.X - ontoPoint.X) +
                   (pass.Y - ontoPoint.Y) * (pass.Y - ontoPoint.Y)
        let delta = dist - n * n
        if delta < 0.0 then
            []
        elif delta = 0.0 then
            [
                axiom2 ontoPoint { 
                    X = pass.X - ontoLine.XFactor * n
                    Y = pass.Y - ontoLine.YFactor * n
                }
            ]
            |> List.choose id
        else
            let s = (sqrt delta)
            [
                axiom2 ontoPoint {
                    X = pass.X - (ontoLine.XFactor * n + ontoLine.YFactor * s)
                    Y = pass.Y - (ontoLine.YFactor * n - ontoLine.XFactor * s)
                }
                axiom2 ontoPoint {
                    X = pass.X - (ontoLine.XFactor * n - ontoLine.YFactor * s)
                    Y = pass.Y - (ontoLine.YFactor * n + ontoLine.XFactor * s)
                }
            ]
            |> List.choose id

let axiom6 (line1: Line) (point1: Point) (line2: Line) (point2: Point) =
    let cbrt x = if x < 0.0 then -(-x ** (1.0 / 3.0)) else x ** (1.0 / 3.0)
    let solveEquation a b c d =
        if a = 0.0 then
            let delta = c * c - 4.0 * b * d
            if delta = 0.0 then
                [ -c / (2.0 * b) ]
            elif delta < 0.0 then
                []
            else
                let c2 = -c / (2.0 * b)
                let s = sqrt delta / (2.0 * b)
                [ c2 + s; c2 - s ]
        else
            let p = (3.0 * a * c - b * b) / (3.0 * a * a)
            let q = (2.0 * b * b * b - 9.0 * a * b * c + 27.0 * a * a * d) / (27.0 * a * a * a)
            let ys =
                if p = 0.0 && q = 0.0 then
                    [ 0.0 ]
                else
                    let delta = (27.0 * q * q + 4.0 * p * p * p) / 108.0
                    if delta > 0.0 then
                        let s = sqrt delta
                        [ cbrt (-q / 2.0 + s) + cbrt (-q / 2.0 - s) ]
                    elif delta = 0.0 then
                        let s = cbrt (q / 2.0)
                        [ -2.0 * s; s ]
                    else
                        let α = -q / 2.0
                        let β = sqrt -delta
                        let s = 2.0 * sqrt (-p / 3.0)
                        let t = atan2 β α / 3.0
                        let u = System.Math.PI / 1.5
                        [ s * cos t; s * cos (t + u); s * cos (t - u) ]
            ys |> List.map(fun y -> y - b / (3.0 * a))
    let getFactors a1 b1 c1 x1 y1 a2 b2 c2 x2 y2 =
        let d = -x2 * a1 - y2 * b1 - c1
        let e = -x2 * a2 - y2 * b2 - c2
        let a = a1 * a2 + b1 * b2
        let b = b1 * a2 - a1 * b2
        let x = (x1 - x2) * a2 + (y1 - y2) * b2
        let y = -(x1 - x2) * b2 + (y1 - y2) * a2
        let α = e * b
        let β = x * b + y * a
        let γ = e * a - d
        let δ = x * a - y * b
        solveEquation α (γ + δ) (α - 2.0 * β) (γ - δ)
        |> List.map(fun t -> 
            let a = a2 - t * b2
            let b = b2 + t * a2
            let c = t * (x2 * b2 - y2 * a2) + 
                    ((x2 * a2 + y2 * b2) * (t * t - 1.0) + c2 * (t * t + 1.0)) / 2.0
            Line.Create(a, b, c))
    if Line.contains point1 line1 || Line.contains point2 line2 then []
    else
        getFactors
            line1.XFactor line1.YFactor line1.Intercept point1.X point1.Y
            line2.XFactor line2.YFactor line2.Intercept point2.X point2.Y

let axiom7 (passLine: Line) (ontoLine: Line) ontoPoint =
    if Line.contains ontoPoint ontoLine then None
    else
        let c =
            passLine.YFactor * ontoPoint.X - passLine.XFactor * ontoPoint.Y -
            (ontoLine.XFactor * ontoPoint.X + ontoLine.YFactor * ontoPoint.Y + ontoLine.Intercept) /
            (2.0 * (ontoLine.XFactor * passLine.YFactor - passLine.XFactor * ontoLine.YFactor))
        Some(Line.Create(-passLine.YFactor, passLine.XFactor, c))

let axiomP (line: Line) point = axiom7 (Line.Create(line.YFactor, -line.XFactor, 0.0)) line point
