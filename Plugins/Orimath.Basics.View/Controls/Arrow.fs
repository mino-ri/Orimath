namespace Orimath.Basics.View.Controls
open System
open System.Windows
open System.Windows.Media
open System.Windows.Shapes
open Orimath.FoldingInstruction

type FpmOptions = FrameworkPropertyMetadataOptions

type Arrow() =
    inherit Shape()
    static let sin60 = sin(Math.PI / 3.0)
    static let cos60 = cos(Math.PI / 3.0)
    static let isDoubleFinite = ValidateValueCallback(fun (o: obj) ->
        match o with
        | :? float as d -> not (Double.IsInfinity(d) || Double.IsNaN(d))
        | _ -> false)

    member this.X1 with get() = this.GetValue(Arrow.X1Property) :?> float and set (v: float) = this.SetValue(Arrow.X1Property, box v)
    static member val X1Property =
        DependencyProperty.Register("X1", typeof<float>, typeof<Arrow>,
            FrameworkPropertyMetadata(0.0, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender), isDoubleFinite)

    member this.X2 with get() = this.GetValue(Arrow.X2Property) :?> float and set (v: float) = this.SetValue(Arrow.X2Property, box v)
    static member val X2Property =
        DependencyProperty.Register("X2", typeof<float>, typeof<Arrow>,
            FrameworkPropertyMetadata(0.0, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender), isDoubleFinite)

    member this.Y1 with get() = this.GetValue(Arrow.Y1Property) :?> float and set (v: float) = this.SetValue(Arrow.Y1Property, box v)
    static member val Y1Property =
        DependencyProperty.Register("Y1", typeof<float>, typeof<Arrow>,
            FrameworkPropertyMetadata(0.0, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender), isDoubleFinite)

    member this.Y2 with get() = this.GetValue(Arrow.Y2Property) :?> float and set (v: float) = this.SetValue(Arrow.Y2Property, box v)
    static member val Y2Property =
        DependencyProperty.Register("Y2", typeof<float>, typeof<Arrow>,
            FrameworkPropertyMetadata(0.0, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender), isDoubleFinite)

    member this.PointMargin with get() = this.GetValue(Arrow.PointMarginProperty) :?> float and set (v: float) = this.SetValue(Arrow.PointMarginProperty, box v)
    static member val PointMarginProperty =
        DependencyProperty.Register("PointMargin", typeof<float>, typeof<Arrow>,
            FrameworkPropertyMetadata(8.0, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender), isDoubleFinite)

    member this.ArrowSize with get() = this.GetValue(Arrow.ArrowSizeProperty) :?> float and set (v: float) = this.SetValue(Arrow.ArrowSizeProperty, box v)
    static member val ArrowSizeProperty =
        DependencyProperty.Register("ArrowSize", typeof<float>, typeof<Arrow>,
            FrameworkPropertyMetadata(12.0, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender), isDoubleFinite)

    member this.StartType with get() = this.GetValue(Arrow.BeginTypeProperty) :?> ArrowType and set (v: ArrowType) = this.SetValue(Arrow.BeginTypeProperty, box v)
    static member val BeginTypeProperty =
        DependencyProperty.Register("StartType", typeof<ArrowType>, typeof<Arrow>,
            FrameworkPropertyMetadata(ArrowType.None, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender))

    member this.EndType with get() = this.GetValue(Arrow.EndTypeProperty) :?> ArrowType and set (v: ArrowType) = this.SetValue(Arrow.EndTypeProperty, box v)
    static member val EndTypeProperty =
        DependencyProperty.Register("EndType", typeof<ArrowType>, typeof<Arrow>,
            FrameworkPropertyMetadata(ArrowType.None, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender))

    member this.Direction with get() = this.GetValue(Arrow.DirectionProperty) :?> ArrowDirection and set (v: ArrowDirection) = this.SetValue(Arrow.DirectionProperty, box v)
    static member val DirectionProperty =
        DependencyProperty.Register("Direction", typeof<ArrowDirection>, typeof<Arrow>,
            FrameworkPropertyMetadata(ArrowDirection.Auto, FpmOptions.AffectsMeasure ||| FpmOptions.AffectsRender))

    override this.DefiningGeometry =
        let inline point v = Vector.op_Explicit(v): Point
        let v1 = Vector(this.X1, this.Y1)
        let v2 = Vector(this.X2, this.Y2)
        let mutable normal = v2 - v1
        normal.Normalize()
        let direction =
            match this.Direction with
            | ArrowDirection.Clockwise -> SweepDirection.Clockwise
            | ArrowDirection.Counterclockwise -> SweepDirection.Counterclockwise
            | _ ->
                if normal.X > 0.0
                then SweepDirection.Clockwise
                else SweepDirection.Counterclockwise
        let vertical =
            if direction = SweepDirection.Clockwise
            then Vector(normal.Y, -normal.X)
            else Vector(-normal.Y, normal.X)
        let pointMargin = this.PointMargin
        let arrowSize = this.ArrowSize
        let geometry = StreamGeometry()
        use context = geometry.Open()
        let point1 = v1 + normal * (pointMargin * sin60) + vertical * (pointMargin * cos60)
        let point2 = v2 - normal * (pointMargin * sin60) + vertical * (pointMargin * cos60)
        let distance = (point2 - point1).Length
        let drawArrow isBegin ty (basePoint: Vector) (normal: Vector) (vertical: Vector) =
            match ty with
            | ArrowType.Normal
            | ArrowType.ValleyFold ->
                let isValey = ty = ArrowType.ValleyFold
                context.BeginFigure(point(basePoint + normal * (arrowSize * cos60) + vertical * (arrowSize * sin60)), isValey, isValey)
                context.LineTo(point basePoint, true, false)
                context.LineTo(point(basePoint + normal * arrowSize), true, false)
                if isBegin then
                    context.BeginFigure(point(basePoint), false, false)
            | ArrowType.MountainFold ->
                if isBegin then
                    context.BeginFigure(point(basePoint + normal * (arrowSize * sin60) + vertical * (arrowSize * cos60)), false, false)
                    context.LineTo(point(basePoint + normal * (arrowSize * 1.25 * cos60) + vertical * (arrowSize * 1.25 * sin60)), true, false)
                    context.LineTo(point basePoint, true, false)
                else
                    context.LineTo(point(basePoint + normal * (arrowSize * 1.25 * cos60) + vertical * (arrowSize * 1.25 * sin60)), true, false)
                    context.LineTo(point(basePoint + normal * (arrowSize * sin60) + vertical * (arrowSize * cos60)), true, false)
            | _ ->
                    if isBegin then
                        context.BeginFigure(point basePoint, false, false)
        drawArrow true this.StartType point1 normal vertical
        context.ArcTo(point point2, Size(distance, distance), 0.0, false, direction, true, false)
        drawArrow false this.EndType point2 (-normal) vertical
        upcast geometry