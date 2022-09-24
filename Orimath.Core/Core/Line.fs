namespace Orimath.Core
open Orimath
open Orimath.ApproximatelyEquatable

[<Struct; NoComparison; StructuralEquality>]
type Line = private Line of xFactor: float * yFactor: float * intercept: float with
    member this.XFactor = match this with Line(v, _, _) -> v
    member this.YFactor = match this with Line(_, v, _) -> v
    member this.Intercept = match this with Line(_, _, v) -> v
    
    override this.ToString() =
        System.String.Format("[{0:0.#####}, {1:0.#####}, {2:0.#####}]", this.XFactor, this.YFactor, this.Intercept)

    interface IApproximatelyEquatable<Line> with
        member this.ApproximatelyEquals(other, margin) =
            aprxEqualf margin this.XFactor other.XFactor &&
            aprxEqualf margin this.YFactor other.YFactor &&
            aprxEqualf margin this.Intercept other.Intercept ||
            aprxEqualf margin this.XFactor -other.XFactor &&
            aprxEqualf margin this.YFactor -other.YFactor &&
            aprxEqualf margin this.Intercept -other.Intercept


[<AutoOpen>]
module LineOperator =
    let (|Line|) (line: Line) = struct (line.XFactor, line.YFactor, line.Intercept)


[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module Line =
    /// 傾きを求めます。
    let slope (line: Line) = !-(line.XFactor / line.YFactor)
    
    /// 傾きをラジアンで求めます。
    let angle (line: Line) = atan2 !-line.YFactor line.XFactor
    
    let create (NonZeroPoint(xFactor, yFactor) as factors) intercept =
        let d = sqrt (xFactor * xFactor + yFactor * yFactor)
        let a = xFactor / d
        let b = yFactor / d
        let c = intercept / d
        match a =~~ 0.0, b =~~ 0.0 with
        | true, true
        | false, true -> Line(1.0, 0.0, if a < 0.0 then !-c else !+c)
        | true, false -> Line(0.0, 1.0, if b < 0.0 then !-c else !+c)
        | false, false ->
            if a < 0.0
            then Line(!-a, !-b, !-c)
            else Line(!+a, !+b, !+c)
    
    let tryCreate xFactor yFactor intercept =
        match NonZeroPoint.create xFactor yFactor with
        | Ok(factors) -> Ok(create factors intercept)
        | Error _ -> Error(Error.invalidLine)

    /// 傾きと通る点から直線を生成します。
    let fromFactorsAndPoint (NonZeroPoint(xFactor, yFactor) as factors) (p: Point) =
        create factors (-p.X * xFactor - p.Y * yFactor)
    
    /// 2点を結ぶ直線を生成します。
    let fromPoints (NotEqual(p1: Point, p2) as points) =
        Line(p1.Y - p2.Y, p2.X - p1.X, p1.X * p2.Y - p2.X * p1.Y)
    
    let tryFromPoints (p1: Point) (p2: Point) =
        match NotEqual.create p1 p2 with
        | Ok(points) -> Ok(fromPoints points)
        | Error _ -> Error(Error.invalidLine)

    /// 直線と点の距離を取得します。直線の右側か左側かによって符号が変わります。
    let signedDist (point: Point) (line: Line) = line.XFactor * point.X + line.YFactor * point.Y + line.Intercept
    
    /// 直線と点の距離を取得します。
    let dist point line = abs (signedDist point line)
    
    /// 直線から点への符号付き距離の符号を取得します。
    let distSign point line =
        let s = signedDist point line
        if s =~~ 0.0 then 0 else sign s
    
    /// 直線から点への符号付き距離の符号が正の範囲にあるか判断します。
    let isPositiveSide point line = distSign point line > 0
    
    /// 直線から点への符号付き距離の符号が負の範囲にあるか判断します。
    let isNegativeSide point line = distSign point line < 0
    
    /// 2つの直線が交差する点を求めます。
    let cross (line1: Line) (line2: Line) =
        let divider = line1.XFactor * line2.YFactor - line2.XFactor * line1.YFactor
        if divider =~~ 0.0 then
            ValueNone
        else
            ValueSome(Point(
                !+((line1.YFactor * line2.Intercept - line2.YFactor * line1.Intercept) / divider),
                !-((line1.XFactor * line2.Intercept - line2.XFactor * line1.Intercept) / divider)))
    
    /// 直線が指定した点を含んでいるか判定します。
    let contains point line = dist point line =~~ 0.0
            
    /// 直線上の指定したY値におけるX値を取得します。
    let getX y (line: Line) = !-((line.YFactor * y + line.Intercept) / line.XFactor)
            
    /// 直線上の指定したX値におけるY値を取得します。
    let getY x (line: Line) = !-((line.XFactor * x + line.Intercept) / line.YFactor)
            
    /// 直線上の指定したX値における点を取得します。
    let xOf x (line: Line) = Point(x, getY x line)
            
    /// 直線上の指定したY値における点を取得します。
    let yOf y (line: Line) = Point(getX y line, y)
            
    /// 点から直線に下した垂線の足を取得します。
    let perpFoot point (line: Line) =
        let sDist = signedDist point line
        Point(point.X - line.XFactor * sDist, point.Y - line.YFactor * sDist)
    
    /// 直線を鏡として、直線を反転させます。
    let reflectBy (mirror: Line) (line: Line) =
        let e = 2.0 * (mirror.XFactor * line.XFactor + mirror.YFactor * line.YFactor)
        Line(line.XFactor - mirror.XFactor * e, line.YFactor - mirror.YFactor * e, line.Intercept - mirror.Intercept * e)
