namespace Orimath.Core
open NearlyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Line = private { A: float; B: float; C: float } with
    member this.XFactor = this.A
    member this.YFactor = this.B
    member this.Intercept = this.C
    member this.Slope = !-(this.A / this.B)

    member this.Deconstruct(xFactor: outref<float>, yFactor: outref<float>, intercept: outref<float>) =
        xFactor <- this.A
        yFactor <- this.B
        intercept <- this.C
    
    /// 直線と点の距離を取得します。直線の右側か左側かによって符号が変わります。
    member this.GetSignedDistance(point: Point) = this.A * point.X + this.B * point.Y + this.C

    /// 直線と点の距離を取得します。
    member this.GetDistance(point: Point) = abs (this.GetSignedDistance(point))

    /// 2つの直線が交差する点を求めます。
    member this.GetCrossPoint(other: Line) =
        let divider = this.A * other.B - other.A * this.B
        if divider =~~ 0.0 then
            None
        else
            Some {
                X = !+((this.B * other.C - other.B * this.C) / divider)
                Y = !-((this.A * other.C - other.A * this.C) / divider)
            }

    /// 直線が指定した点を含んでいるか判定します。
    member line.Contains(point: Point) = line.GetDistance(point) =~~ 0.0
            
    /// 直線上の指定したY値におけるX値を取得します。
    member line.GetX(y) = !-((line.B * y + line.C) / line.A)
            
    /// 直線上の指定したX値におけるY値を取得します。
    member line.GetY(x) = !-((line.A * x + line.C) / line.B)
            
    /// 直線上の指定したX値における点を取得します。
    member line.XOf(x) = { X = x; Y = line.GetY(x) }
            
    /// 直線上の指定したY値における点を取得します。
    member line.YOf(y) = { X = line.GetX(y); Y = y }
            
    /// 点から直線に下した垂線の足を取得します。
    member line.GetPerpendicularFoot(point: Point) =
        let sDist = line.GetSignedDistance(point)
        {
            X = point.X - line.A * sDist
            Y = point.Y - line.B * sDist
        }
            
    /// 現在の直線を鏡として、直線を反転させます。
    member this.Reflect(target: Line) =
        let e = 2.0 * (this.A * target.A + this.B * target.B)
        Line.Create(target.A - this.A * e, target.B - this.B * e, target.C - this.C * e)

    /// 現在の直線を鏡として、点を反転させます。
    member this.Reflect(point: Point) =
        let sDist = 2.0 * this.GetSignedDistance(point)
        {
            X = point.X - this.A * sDist
            Y = point.Y - this.B * sDist
        }

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

    [<CompiledName("FromPointsFSharp")>]
    static member FromPoints(p1, p2) =
        if p1 = p2 then None
        else Some(Line.Create(p1.Y - p2.Y, p2.X - p1.X, p1.X * p2.Y - p2.X * p1.Y))

    [<CompiledName("FromPoints")>]
    static member FromPointsCSharp(p1, p2) =
        Option.toNullable(Line.FromPoints(p1, p2))
