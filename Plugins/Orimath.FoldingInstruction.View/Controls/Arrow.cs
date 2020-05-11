using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using FpmOptions = System.Windows.FrameworkPropertyMetadataOptions;

namespace Orimath.FoldingInstruction.View.Controls
{
    public class Arrow : Shape
    {
        private static readonly double Sin60 = Math.Sin(Math.PI / 3d);
        private static readonly double Cos60 = Math.Cos(Math.PI / 3d);

        internal static bool IsDoubleFinite(object o)
        {
            var d = (double)o;
            return !(double.IsInfinity(d) || double.IsNaN(d));
        }

        public double X1 { get => (double)GetValue(X1Property); set => SetValue(X1Property, value); }
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register(nameof(X1), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0d, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender), IsDoubleFinite);

        public double X2 { get => (double)GetValue(X2Property); set => SetValue(X2Property, value); }
        public static readonly DependencyProperty X2Property =
            DependencyProperty.Register(nameof(X2), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0d, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender), IsDoubleFinite);

        public double Y1 { get => (double)GetValue(Y1Property); set => SetValue(Y1Property, value); }
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register(nameof(Y1), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0d, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender), IsDoubleFinite);

        public double Y2 { get => (double)GetValue(Y2Property); set => SetValue(Y2Property, value); }
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register(nameof(Y2), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(0d, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender), IsDoubleFinite);

        public double PointMargin { get => (double)GetValue(PointMarginProperty); set => SetValue(PointMarginProperty, value); }
        public static readonly DependencyProperty PointMarginProperty =
            DependencyProperty.Register(nameof(PointMargin), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(8d, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender), IsDoubleFinite);

        public double ArrowSize { get => (double)GetValue(ArrowSizeProperty); set => SetValue(ArrowSizeProperty, value); }
        public static readonly DependencyProperty ArrowSizeProperty =
            DependencyProperty.Register(nameof(ArrowSize), typeof(double), typeof(Arrow),
                new FrameworkPropertyMetadata(12d, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender), IsDoubleFinite);

        public ArrowType StartType { get => (ArrowType)GetValue(BeginTypeProperty); set => SetValue(BeginTypeProperty, value); }
        public static readonly DependencyProperty BeginTypeProperty =
            DependencyProperty.Register(nameof(StartType), typeof(ArrowType), typeof(Arrow),
                new FrameworkPropertyMetadata(ArrowType.None, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender));

        public ArrowType EndType { get => (ArrowType)GetValue(EndTypeProperty); set => SetValue(EndTypeProperty, value); }
        public static readonly DependencyProperty EndTypeProperty =
            DependencyProperty.Register(nameof(EndType), typeof(ArrowType), typeof(Arrow),
                new FrameworkPropertyMetadata(ArrowType.None, FpmOptions.AffectsMeasure | FpmOptions.AffectsRender));

        protected override Geometry DefiningGeometry
        {
            get
            {
                var v1 = new Vector(X1, Y1);
                var v2 = new Vector(X2, Y2);
                var normal = v2 - v1;
                normal.Normalize();
                var vertical = normal.X > 0d
                    ? new Vector(normal.Y, -normal.X)
                    : new Vector(-normal.Y, normal.X);
                var pointMargin = PointMargin;
                var arrowSize = ArrowSize;

                var geometry = new StreamGeometry();
                using (var context = geometry.Open())
                {
                    var point1 = v1 + normal * (pointMargin * Sin60) + vertical * (pointMargin * Cos60);
                    var point2 = v2 - normal * (pointMargin * Sin60) + vertical * (pointMargin * Cos60);
                    var distance = (point2 - point1).Length;

                    DrawArrow(true, StartType, point1, normal, vertical);
                    context.ArcTo((Point)point2, new Size(distance, distance), 0d, false, normal.X > 0d ? SweepDirection.Clockwise : SweepDirection.Counterclockwise, true, false);
                    DrawArrow(false, EndType, point2, -normal, vertical);

                    void DrawArrow(bool begin, ArrowType type, Vector basePoint, Vector normal, Vector vertical)
                    {
                        switch (type)
                        {
                            case ArrowType.Normal:
                            case ArrowType.ValleyFold:
                                var isValey = type == ArrowType.ValleyFold;
                                context.BeginFigure((Point)(basePoint + normal * (arrowSize * Cos60) + vertical * (arrowSize * Sin60)), isValey, isValey);
                                context.LineTo((Point)basePoint, true, false);
                                context.LineTo((Point)(basePoint + normal * arrowSize), true, false);
                                if (begin)
                                    context.BeginFigure((Point)basePoint, false, false);
                                break;

                            case ArrowType.MountainFold:
                                if (begin)
                                {
                                    context.BeginFigure((Point)(basePoint + normal * (arrowSize * Sin60) + vertical * (arrowSize * Cos60)), false, false);
                                    context.LineTo((Point)(basePoint + normal * (arrowSize * 1.25 * Cos60) + vertical * (arrowSize * 1.25 * Sin60)), true, false);
                                    context.LineTo((Point)basePoint, true, false);
                                }
                                else
                                {
                                    context.LineTo((Point)(basePoint + normal * (arrowSize * 1.25 * Cos60) + vertical * (arrowSize * 1.25 * Sin60)), true, false);
                                    context.LineTo((Point)(basePoint + normal * (arrowSize * Sin60) + vertical * (arrowSize * Cos60)), true, false);
                                }
                                break;

                            default:
                                if (begin)
                                    context.BeginFigure((Point)basePoint, false, false);
                                break;
                        }
                    }
                }

                return geometry;

            }
        }
    }
}
