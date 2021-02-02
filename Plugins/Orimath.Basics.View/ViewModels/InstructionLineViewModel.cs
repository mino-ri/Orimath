using Mvvm;
using Orimath.Plugins;
using Orimath.FoldingInstruction;

namespace Orimath.Basics.View.ViewModels
{
    public class InstructionLineViewModel : NotifyPropertyChanged
    {
        private readonly IViewPointConverter _pointConverter;

        private double _x1;
        public double X1 { get => _x1; set => SetValue(ref _x1, value); }

        private double _x2;
        public double X2 { get => _x2; set => SetValue(ref _x2, value); }

        private double _y1;
        public double Y1 { get => _y1; set => SetValue(ref _y1, value); }

        private double _y2;
        public double Y2 { get => _y2; set => SetValue(ref _y2, value); }

        private InstructionColor _color;
        public InstructionColor Color { get => _color; set => SetValue(ref _color, value); }    

        public InstructionLineViewModel(IViewPointConverter pointConverter)
        {
            _pointConverter = pointConverter;
        }

        public InstructionLineViewModel(IViewPointConverter pointConverter, InstructionLine model)
            : this(pointConverter)
        {
            SetModel(model);
        }

        public void SetModel(InstructionLine model)
        {
            var point1 = _pointConverter.ModelToView(model.Line.Point1);
            var point2 = _pointConverter.ModelToView(model.Line.Point2);
            X1 = point1.X;
            Y1 = point1.Y;
            X2 = point2.X;
            Y2 = point2.Y;
            Color = model.Color;
        }
    }
}
