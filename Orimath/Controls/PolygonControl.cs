using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Orimath.Controls
{
    public class PolygonControl : Control
    {
        public double Points { get => (double)GetValue(PointsProperty); set => SetValue(PointsProperty, value); }
        public static readonly DependencyProperty PointsProperty = Polygon.PointsProperty.AddOwner(typeof(PolygonControl));

        static PolygonControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PolygonControl),
                new FrameworkPropertyMetadata(typeof(PolygonControl)));
        }
    }
}
