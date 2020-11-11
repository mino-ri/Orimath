using System.ComponentModel;
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

        public static ThemeBrush Default { get; } = new();

        public ThemeBrush? BasedOn { get; set; }

        private BrushSet? _normal;
        public BrushSet Normal { get => _normal ?? BasedOn?.Normal ?? BrushSet.Default; set => _normal = value; }

        private BrushSet? _hovered;
        public BrushSet Hovered { get => _hovered ?? BasedOn?.Hovered ?? BrushSet.Default; set => _hovered = value; }

        private BrushSet? _highlight;
        public BrushSet Highlight { get => _highlight ?? BasedOn?.Highlight ?? BrushSet.Default; set => _highlight = value; }

        private BrushSet? _disabled;
        public BrushSet Disabled { get => _disabled ?? BasedOn?.Disabled ?? BrushSet.Default; set => _disabled = value; }

        public ThemeBrush() { }

        public ThemeBrush(BrushSet? normal, BrushSet? hovered, BrushSet? highlight, BrushSet? disabled)
        {
            _normal = normal;
            _hovered = hovered;
            _highlight = highlight;
            _disabled = disabled;
        }
    }

    [TypeConverter(typeof(BrushSetConverter))]
    public class BrushSet
    {
        public static BrushSet Default { get; } = new();

        public BrushSet? BasedOn { get; set; }

        private Brush? _background;
        public Brush? Background { get => _background ?? BasedOn?.Background; set => _background = value; }

        private Brush? _foreGround;
        public Brush? Foreground { get => _foreGround ?? BasedOn?.Foreground; set => _foreGround = value; }

        private Brush? _border;
        public Brush? Border { get => _border ?? BasedOn?.Border; set => _border = value; }

        public BrushSet() { }

        public BrushSet(Brush? background, Brush? foreground, Brush? border)
        {
            Background = background;
            Foreground = foreground;
            Border = border;

            if (Background is not null && Background.CanFreeze && !Background.IsFrozen)
                Background.Freeze();

            if (Foreground is not null && Foreground.CanFreeze && !Foreground.IsFrozen)
                Foreground.Freeze();

            if (Border is not null && Border.CanFreeze && !Border.IsFrozen)
                Border.Freeze();
        }

        public BrushSet(Color background, Color foreground, Color border)
            : this(new SolidColorBrush(background), new SolidColorBrush(foreground), new SolidColorBrush(border)) { }
    }
}
