using System;
using System.ComponentModel;
using System.Globalization;

namespace Orimath.Themes
{
    public class BrushSetConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string str)
            {
                var parts = str.Split('.');
                if (parts.Length != 2) return base.ConvertFrom(context, culture, value);

                return ThemeBrushes.ResolveThemeBrushPath(parts[0])
                    ?.ResolveBrushSetPath(parts[1])
                    ?? base.ConvertFrom(context, culture, value);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
