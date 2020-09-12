using Orimath.Core;
using ScreenPoint = System.Windows.Point;

namespace Orimath.Plugins
{
    public interface IViewPointConverter
    {
        ScreenPoint ModelToView(Point point);
        Point ViewToModel(ScreenPoint point);
    }
}
