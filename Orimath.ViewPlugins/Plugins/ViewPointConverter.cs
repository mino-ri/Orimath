using Orimath.Core;
using ScreenPoint = System.Windows.Point;

namespace Orimath.Plugins
{
    public interface IViewPointConverter
    {
        ScreenPoint ModelToView(Point point);
        Point ViewToModel(ScreenPoint point);
    }

    public class ViewPointConverter : IViewPointConverter
    {
        private readonly double _scaleX;
        private readonly double _scaleY;
        private readonly double _offsetX;
        private readonly double _offsetY;

        public ViewPointConverter(double scaleX, double scaleY, double offsetX, double offsetY)
        {
            _scaleX = scaleX;
            _scaleY = scaleY;
            _offsetX = offsetX;
            _offsetY = offsetY;
        }

        public ScreenPoint ModelToView(Point point) => new ScreenPoint(point.X * _scaleX + _offsetX, point.Y * _scaleY + _offsetY);

        public Point ViewToModel(ScreenPoint point) => new Point((point.X - _offsetX) / _scaleX, (point.Y - _offsetY) / _scaleY);
    }
}
