using System;
using System.Windows.Markup;
using System.Windows.Media;

namespace Orimath.Themes
{
    [MarkupExtensionReturnType(typeof(Brush))]
    public class ThemeBrushExtension : MarkupExtension
    {
        public string? Path { get; set; }

        public ThemeBrushExtension() { }

        public ThemeBrushExtension(string path) => Path = path;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Path is null) throw new InvalidOperationException("Invalid Path.");
            var parts = Path.Split('.');
            if (parts.Length != 3) throw new InvalidOperationException("Invalid Path.");

            return ThemeBrushes.ResolveThemeBrushPath(parts[0])!
                    .ResolveBrushSetPath(parts[1])!
                    .ResolveBrushPath(parts[2])!;
        }
    }
}
