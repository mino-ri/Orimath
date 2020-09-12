using System.Windows;
using System.Windows.Media;

namespace Orimath.Themes
{
    public class ThemeBrush
    {
        public static ThemeBrush GetTheme(DependencyObject obj) => (ThemeBrush)obj.GetValue(ThemeProperty);
        public static void SetTheme(DependencyObject obj, ThemeBrush value) => obj.SetValue(ThemeProperty, value);
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.RegisterAttached("Theme", typeof(ThemeBrush), typeof(ThemeBrush),
                new FrameworkPropertyMetadata(new ThemeBrush(), FrameworkPropertyMetadataOptions.AffectsRender));

        public static ThemeBrush Default { get; } = new ThemeBrush();

        private BrushSet _normal;
        public BrushSet Normal { get => _normal; set => _normal = value ?? BrushSet.Default; }

        private BrushSet _hovered;
        public BrushSet Hovered { get => _hovered; set => _hovered = value ?? BrushSet.Default; }

        private BrushSet _highlight;
        public BrushSet Highlight { get => _highlight; set => _highlight = value ?? BrushSet.Default; }

        private BrushSet _disabled;
        public BrushSet Disabled { get => _disabled; set => _disabled = value ?? BrushSet.Default; }

        public ThemeBrush()
        {
            _normal = BrushSet.Default;
            _hovered = BrushSet.Default;
            _highlight = BrushSet.Default;
            _disabled = BrushSet.Default;
        }

        public ThemeBrush(BrushSet normal, BrushSet hovered, BrushSet highlight, BrushSet disabled)
        {
            _normal = normal ?? BrushSet.Default;
            _hovered = hovered ?? BrushSet.Default;
            _highlight = highlight ?? BrushSet.Default;
            _disabled = disabled ?? BrushSet.Default;
        }
    }

    public class BrushSet
    {
        public static BrushSet Default { get; } = new BrushSet();

        public Brush? Background { get; set; }

        public Brush? Foreground { get; set; }

        public Brush? Border { get; set; }

        public BrushSet() { }

        public BrushSet(Brush? background, Brush? foreground, Brush? border)
        {
            Background = background;
            Foreground = foreground;
            Border = border;

            if (Background is { } && Background.CanFreeze && !Background.IsFrozen)
                Background.Freeze();

            if (Foreground is { } && Foreground.CanFreeze && !Foreground.IsFrozen)
                Foreground.Freeze();

            if (Border is { } && Border.CanFreeze && !Border.IsFrozen)
                Border.Freeze();
        }

        public BrushSet(Color background, Color foreground, Color border)
            : this(new SolidColorBrush(background), new SolidColorBrush(foreground), new SolidColorBrush(border)) { }
    }
}
