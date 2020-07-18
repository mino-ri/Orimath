using Orimath.Plugins;
using Point = Orimath.Core.Point;
using ViewPoint = System.Windows.Point;

namespace Orimath.ViewModels
{
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

        public ViewPoint ModelToView(Point point) => new ViewPoint(point.X * _scaleX + _offsetX, point.Y * _scaleY + _offsetY);

        public Point ViewToModel(ViewPoint point) => new Point((point.X - _offsetX) / _scaleX, (point.Y - _offsetY) / _scaleY);
    }
}
