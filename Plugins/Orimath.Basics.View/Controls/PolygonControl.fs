namespace Orimath.Basics.View.Controls
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes

type PolygonControl() =
    inherit Control()
    static let mutable init = false
    do
        if not init then
            init <- true
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof<PolygonControl>, FrameworkPropertyMetadata(typeof<PolygonControl>))

    member this.Points
        with get() = this.GetValue(PolygonControl.PointsProperty) :?> float
        and set (v: float) = this.SetValue(PolygonControl.PointsProperty, box v)
    static member val PointsProperty = Polygon.PointsProperty.AddOwner(typeof<PolygonControl>)
