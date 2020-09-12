using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Orimath.Themes
{
    // todo: 最終的には外部読み込みにする
    public static class ThemeBrushes
    {
        public static ThemeBrush Control { get; }
        public static ThemeBrush Workspace { get; }
        public static ThemeBrush Input { get; }
        public static ThemeBrush ScrollBar { get; }

        static ThemeBrushes()
        {
            var border = Solid(26, 81, 53);
            var normal = Solid(40, 121, 80);
            var hover = Solid(53, 161, 107);
            var highlight = Solid(255, 153, 0);
            var highlightBorder = Solid(168, 100, 0);
            var disabledBack = Solid(242, 242, 242);
            var disabledFore = Solid(166, 166, 166);
            var hoverBack = Solid(210, 240, 225);

            Control = new ThemeBrush
            {
                Normal = new BrushSet(normal, Brushes.White, border),
                Hovered = new BrushSet(hover, Brushes.White, border),
                Highlight = new BrushSet(highlight, Brushes.White, highlightBorder),
                Disabled = new BrushSet(disabledFore, disabledBack, Solid(127, 127, 127)),
            };

            Workspace = new ThemeBrush
            {
                Normal = new BrushSet(Solid(240, 250, 245), Brushes.Black, normal),
                Hovered = new BrushSet(hoverBack, Brushes.Black, hover),
                Highlight = new BrushSet(Solid(255, 215, 155), Brushes.Black, highlightBorder),
                Disabled = new BrushSet(disabledBack, disabledFore, disabledFore),
            };

            Input = new ThemeBrush
            {
                Normal = new BrushSet(Brushes.White, normal, Brushes.Black),
                Hovered = new BrushSet(Brushes.White, hover, Brushes.Black),
                Highlight = new BrushSet(Brushes.White, highlight, Brushes.Black),
                Disabled = new BrushSet(disabledBack, disabledFore, disabledFore),
            };

            ScrollBar = new ThemeBrush
            {
                Normal = new BrushSet(hoverBack, normal, null),
                Hovered = new BrushSet(hoverBack, hoverBack, null),
                Highlight = new BrushSet(hoverBack, hoverBack, null),
                Disabled = new BrushSet(disabledBack, disabledFore, null),
            };
        }

        private static Brush Solid(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }
    }
}
