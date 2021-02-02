using Mvvm;
using Orimath.Plugins;
using Orimath.FoldingInstruction;

namespace Orimath.Basics.View.ViewModels
{
    public class InstructionPointViewModel : NotifyPropertyChanged
    {
        private readonly IViewPointConverter _pointConverter;

        private double _x;
        public double X { get => _x; set => SetValue(ref _x, value); }

        private double _y;
        public double Y { get => _y; set => SetValue(ref _y, value); }

        private InstructionColor _color;
        public InstructionColor Color { get => _color; set => SetValue(ref _color, value); }

        public InstructionPointViewModel(IViewPointConverter pointConverter)
        {
            _pointConverter = pointConverter;
        }

        public InstructionPointViewModel(IViewPointConverter pointConverter, InstructionPoint model)
            : this(pointConverter)
        {
            SetModel(model);
        }

        public void SetModel(InstructionPoint model)
        {
            var point = _pointConverter.ModelToView(model.Point);
            X = point.X;
            Y = point.Y;
            Color = model.Color;
        }
    }
}
