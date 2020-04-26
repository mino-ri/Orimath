using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Orimath.Basics.View.Controls
{
    public class LineControl : Control
    {
        [TypeConverter(typeof(DoubleConverter))]
        public double X1 { get => (double)GetValue(X1Property); set => SetValue(X1Property, value); }
        public static readonly DependencyProperty X1Property = Line.X1Property.AddOwner(typeof(LineControl));

        [TypeConverter(typeof(DoubleConverter))]
        public double X2 { get => (double)GetValue(X2Property); set => SetValue(X2Property, value); }
        public static readonly DependencyProperty X2Property = Line.X2Property.AddOwner(typeof(LineControl));

        [TypeConverter(typeof(DoubleConverter))]
        public double Y1 { get => (double)GetValue(Y1Property); set => SetValue(Y1Property, value); }
        public static readonly DependencyProperty Y1Property = Line.Y1Property.AddOwner(typeof(LineControl));

        [TypeConverter(typeof(DoubleConverter))]
        public double Y2 { get => (double)GetValue(Y2Property); set => SetValue(Y2Property, value); }
        public static readonly DependencyProperty Y2Property = Line.Y2Property.AddOwner(typeof(LineControl));

        [TypeConverter(typeof(DoubleConverter))]
        public double StrokeThickness { get => (double)GetValue(StrokeThicknessProperty); set => SetValue(StrokeThicknessProperty, value); }
        public static readonly DependencyProperty StrokeThicknessProperty = Line.StrokeThicknessProperty.AddOwner(typeof(LineControl));

        static LineControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(LineControl),
                new FrameworkPropertyMetadata(typeof(LineControl)));
        }
    }
}
