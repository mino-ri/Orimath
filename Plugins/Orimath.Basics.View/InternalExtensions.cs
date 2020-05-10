using System.Windows;

namespace Orimath.Basics.View
{
    internal static class InternalExtensions
    {
        public static void Deconstruct(this Point point, out double x, out double y)
        {
            x = point.X;
            y = point.Y;
        }
    }
}
