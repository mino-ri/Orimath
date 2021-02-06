namespace Orimath.Core
open NearlyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Line = private { A: float; B: float; C: float } with
    member this.XFactor = this.A
    member this.YFactor = this.B
    member this.Intercept = this.C
    member this.Slope = !-(this.A / this.B)
    
    override this.ToString() = System.String.Format("[{0:0.#####}, {1:0.#####}, {2:0.#####}]", this.A, this.B, this.C)

    interface INearlyEquatable<Line> with
        member this.NearlyEquals(other, margin) =
            nearlyEqualsf margin this.A other.A &&
            nearlyEqualsf margin this.B other.B &&
            nearlyEqualsf margin this.C other.C

    static member Create(xFactor, yFactor, intercept) =
        let d = sqrt (xFactor * xFactor + yFactor * yFactor)
        let a = xFactor / d
        let b = yFactor / d
        let c = intercept / d
        let aZero = a =~~ 0.0
        let bZero = b =~~ 0.0
        match aZero, bZero with
        | true, true -> failwith "直線の傾きを定義できません。"
        | false, true -> { A = 1.0; B = 0.0; C = if a < 0.0 then !-c else !+c }
        | true, false -> { A = 0.0; B = 1.0; C = if b < 0.0 then !-c else !+c }
        | false, false ->
            if a < 0.0
            then { A = !-a; B = !-b; C = !-c }
            else { A = !+a; B = !+b; C = !+c }

    static member FromFactorsAndPoint(xFactor, yFactor, p) =
        Line.Create(xFactor, yFactor, -p.X * xFactor - p.Y * yFactor)

    static member FromPoints(p1, p2) =
        if p1 = p2 then None
        else Some(Line.Create(p1.Y - p2.Y, p2.X - p1.X, p1.X * p2.Y - p2.X * p1.Y))

    static member FromPointsCSharp(p1, p2) =
        Option.toNullable(Line.FromPoints(p1, p2))


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Line =
    /// 直線と点の距離を取得します。直線の右側か左側かによって符号が変わります。
    let signedDist point line = line.A * point.X + line.B * point.Y + line.C

    /// 直線と点の距離を取得します。
    let dist point line = abs (signedDist point line)

    /// 直線から点への符号付き距離の符号を取得します。
    let distSign point line =
        let s = signedDist point line
        if s =~~ 0.0 then 0 else sign s

    /// 直線から点への符号付き距離の符号が正の範囲にあるか判断します。
    let isPositiveSide point line = 0 < distSign point line

    /// 直線から点への符号付き距離の符号が負の範囲にあるか判断します。
    let isNegativeSide point line = distSign point line < 0

    /// 2つの直線が交差する点を求めます。
    let cross line1 line2 =
        let divider = line1.A * line2.B - line2.A * line1.B
        if divider =~~ 0.0 then
            None
        else
            Some { X = !+((line1.B * line2.C - line2.B * line1.C) / divider)
                   Y = !-((line1.A * line2.C - line2.A * line1.C) / divider) }

    /// 直線が指定した点を含んでいるか判定します。
    let contains point line = dist point line =~~ 0.0
        
    /// 直線上の指定したY値におけるX値を取得します。
    let getX y line = !-((line.B * y + line.C) / line.A)
        
    /// 直線上の指定したX値におけるY値を取得します。
    let getY x line = !-((line.A * x + line.C) / line.B)
        
    /// 直線上の指定したX値における点を取得します。
    let xOf x line = { X = x; Y = getY x line }
        
    /// 直線上の指定したY値における点を取得します。
    let yOf y line = { X = getX y line; Y = y }
        
    /// 点から直線に下した垂線の足を取得します。
    let perpFoot point line =
        let sDist = signedDist point line
        { X = point.X - line.A * sDist
          Y = point.Y - line.B * sDist }

    /// 直線を鏡として、直線を反転させます。
    let reflectBy mirror line =
        let e = 2.0 * (mirror.A * line.A + mirror.B * line.B)
        Line.Create(line.A - mirror.A * e, line.B - mirror.B * e, line.C - mirror.C * e)


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Point =
    /// 点と点の距離を求めます。
    let dist (p1: Point) p2 = p1.GetDistance(p2)

    /// 点のノルムを求めます。
    let norm (p: Point) = p.Norm

    /// 直線を鏡として、点を反転させます。
    let reflectBy mirror point =
        let sDist = 2.0 * Line.signedDist point mirror
        { X = point.X - mirror.A * sDist
          Y = point.Y - mirror.B * sDist }
