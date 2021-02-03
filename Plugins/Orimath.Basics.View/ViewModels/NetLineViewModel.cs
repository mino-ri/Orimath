using Mvvm;
using Orimath.Core;
using Orimath.FoldingInstruction;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public class NetLineViewModel : NotifyPropertyChanged
    {
        public double X1 { get; }
        public double Y1 { get; }
        public double X2 { get; }
        public double Y2 { get; }
        public InstructionColor Color { get; }

        public NetLineViewModel(LineSegment line, IViewPointConverter pointConverter, InstructionColor color)
        {
            (X1, Y1) = pointConverter.ModelToView(line.Point1);
            (X2, Y2) = pointConverter.ModelToView(line.Point2);
            Color = color;
        }
    }
}
