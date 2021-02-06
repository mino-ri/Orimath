namespace Orimath.Basics.View.Controls
open System.ComponentModel
open System.Windows
open System.Windows.Controls
open System.Windows.Shapes

type LineControl() =
    inherit Control()
    static let mutable init = false
    do
        if not init then
            init <- true
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(typeof<LineControl>, new FrameworkPropertyMetadata(typeof<LineControl>))

    [<TypeConverter(typeof<DoubleConverter>)>]
    member this.X1 with get() = this.GetValue(LineControl.X1Property) :?> float and set (v: float) = this.SetValue(LineControl.X1Property, box v)
    static member val X1Property = Line.X1Property.AddOwner(typeof<LineControl>)

    [<TypeConverter(typeof<DoubleConverter>)>]
    member this.X2 with get() = this.GetValue(LineControl.X2Property) :?> float and set (v: float) = this.SetValue(LineControl.X2Property, box v)
    static member val X2Property = Line.X2Property.AddOwner(typeof<LineControl>)

    [<TypeConverter(typeof<DoubleConverter>)>]
    member this.Y1 with get() = this.GetValue(LineControl.Y1Property) :?> float and set (v: float) = this.SetValue(LineControl.Y1Property, box v)
    static member val Y1Property = Line.Y1Property.AddOwner(typeof<LineControl>)

    [<TypeConverter(typeof<DoubleConverter>)>]
    member this.Y2 with get() = this.GetValue(LineControl.Y2Property) :?> float and set (v: float) = this.SetValue(LineControl.Y2Property, box v)
    static member val Y2Property = Line.Y2Property.AddOwner(typeof<LineControl>)

    [<TypeConverter(typeof<DoubleConverter>)>]
    member this.StrokeThickness with get() = this.GetValue(LineControl.StrokeThicknessProperty) :?> float and set (v: float) = this.SetValue(LineControl.StrokeThicknessProperty, box v)
    static member val StrokeThicknessProperty = Line.StrokeThicknessProperty.AddOwner(typeof<LineControl>)
