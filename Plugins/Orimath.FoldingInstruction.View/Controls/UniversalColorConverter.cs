using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Orimath.FoldingInstruction.View.Controls
{
    [ValueConversion(typeof(InstructionColor), typeof(Brush))]
    public class UniversalColorConverter : IValueConverter
    {
        private Brush[] _brushes;

        public UniversalColorConverter()
        {
            _brushes = new Brush[20];
            _brushes[(int)InstructionColor.Black] = CreateBrush(0x000000);
            _brushes[(int)InstructionColor.White] = CreateBrush(0xFFFFFF);
            _brushes[(int)InstructionColor.LightGray] = CreateBrush(0xC8C8CB);
            _brushes[(int)InstructionColor.Gray] = CreateBrush(0x7F878F);
            _brushes[(int)InstructionColor.Red] = CreateBrush(0xFF2800);
            _brushes[(int)InstructionColor.Yellow] = CreateBrush(0xFAF500);
            _brushes[(int)InstructionColor.Green] = CreateBrush(0x35A16B);
            _brushes[(int)InstructionColor.Blue] = CreateBrush(0x0041FF);
            _brushes[(int)InstructionColor.Skyblue] = CreateBrush(0x66CCFF);
            _brushes[(int)InstructionColor.Pink] = CreateBrush(0xFF99A0);
            _brushes[(int)InstructionColor.Orange] = CreateBrush(0xFF9900);
            _brushes[(int)InstructionColor.Purple] = CreateBrush(0x9A0079);
            _brushes[(int)InstructionColor.Brown] = CreateBrush(0x663300);
            _brushes[(int)InstructionColor.LightPink] = CreateBrush(0xFFD1D1);
            _brushes[(int)InstructionColor.Cream] = CreateBrush(0xFFFF99);
            _brushes[(int)InstructionColor.YellowGreen] = CreateBrush(0xCBF266);
            _brushes[(int)InstructionColor.LightSkyblue] = CreateBrush(0xB4EBFA);
            _brushes[(int)InstructionColor.Beige] = CreateBrush(0xEDC58F);
            _brushes[(int)InstructionColor.LightGreen] = CreateBrush(0x87E7B0);
            _brushes[(int)InstructionColor.LightPurple] = CreateBrush(0xC7B2DE);
        }

        private Brush CreateBrush(uint value)
        {
            var brush = new SolidColorBrush(Color.FromRgb((byte)(value >> 16), (byte)(value >> 8), (byte)value));
            brush.Freeze();
            return brush;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var c = (InstructionColor)value;
            if (c < InstructionColor.Black || InstructionColor.LightPurple < c)
                c = InstructionColor.Black;

            return _brushes[(int)c];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
