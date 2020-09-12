using System;
using System.Windows.Markup;

namespace Orimath.Controls
{
    public class DoubleExtension : MarkupExtension
    {
        public double Value { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Value;
        }
    }
}
