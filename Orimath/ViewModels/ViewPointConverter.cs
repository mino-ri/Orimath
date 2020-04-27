using Orimath.Plugins;
using Point = Orimath.Core.Point;
using ViewPoint = System.Windows.Point;

namespace Orimath.ViewModels
{
    public class ViewPointConverter : IViewPointConverter
    {
        private readonly double _scale;
        private readonly double _offsetX;
        private readonly double _offsetY;

        public ViewPointConverter(double scale, double offsetX, double offsetY)
        {
            _scale = scale;
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        public ViewPoint ModelToView(Point point) => new ViewPoint(point.X * _scale + _offsetX, point.Y * _scale + _offsetY);

        public Point ViewToModel(ViewPoint point) => new Point((point.X - _offsetX) / _scale, (point.Y - _offsetY) / _scale);
    }
}
