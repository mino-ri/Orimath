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
        public static ThemeBrush CloseButton { get; }
        public static ThemeBrush Workspace { get; }
        public static ThemeBrush Input { get; }
        public static ThemeBrush Selector { get; }
        public static ThemeBrush ScrollBar { get; }

        static ThemeBrushes()
        {
            var border = Solid(26, 81, 53);
            var normal = Solid(40, 121, 80);
            var hover = Solid(53, 161, 107);
            var highlight = Solid(255, 153, 0);
            var highlightLight = Solid(255, 215, 155);
            var highlightBorder = Solid(168, 100, 0);
            var disabledForeground = Solid(166, 166, 166);
            var back = Solid(243, 243, 243);
            var hoveredGray = Solid(230, 230, 230);

            Control = new ThemeBrush
            {
                Normal = new BrushSet(normal, Brushes.White, border),
                Hovered = new BrushSet(hover, Brushes.White, border),
                Highlight = new BrushSet(highlight, Brushes.White, highlightBorder),
                Disabled = new BrushSet(disabledForeground, Solid(217, 217, 217), Solid(127, 127, 127)),
            };

            CloseButton = new ThemeBrush
            {
                Normal = Control.Normal,
                Hovered = new BrushSet(Solid(232, 17, 35), Brushes.White, border),
                Highlight = Control.Highlight,
                Disabled = Control.Disabled,
            };

            Workspace = new ThemeBrush
            {
                Normal = new BrushSet(back, Brushes.Black, normal),
                Hovered = new BrushSet(hoveredGray, Brushes.Black, hover),
                Highlight = new BrushSet(highlightLight, Brushes.Black, highlightBorder),
                Disabled = new BrushSet(back, disabledForeground, disabledForeground),
            };

            Input = new ThemeBrush
            {
                Normal = new BrushSet(Brushes.White, Brushes.Black, normal),
                Hovered = new BrushSet(Brushes.White, Brushes.Black, hover),
                Highlight = new BrushSet(Brushes.White, Brushes.Black, highlight),
                Disabled = new BrushSet(back, disabledForeground, disabledForeground),
            };

            Selector = new ThemeBrush
            {
                Normal = new BrushSet(Brushes.White, Brushes.Black, normal),
                Hovered = new BrushSet(hoveredGray, Brushes.Black, hover),
                Highlight = new BrushSet(highlightLight, Brushes.Black, highlightBorder),
                Disabled = new BrushSet(back, disabledForeground, disabledForeground),
            };

            ScrollBar = new ThemeBrush
            {
                Normal = new BrushSet(back, Solid(217, 217, 217), null),
                Hovered = new BrushSet(back, Solid(190, 190, 190), null),
                Highlight = new BrushSet(back, Solid(190, 190, 190), null),
                Disabled = new BrushSet(back, hoveredGray, null),
            };
        }

        private static Brush Solid(byte r, byte g, byte b)
        {
            var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
            brush.Freeze();
            return brush;
        }

        internal static ThemeBrush? ResolveThemeBrushPath(string path)
        {
            return path switch
            {
                nameof(Control) => Control,
                nameof(CloseButton) => CloseButton,
                nameof(Workspace) => Workspace,
                nameof(Input) => Input,
                nameof(Selector) => Selector,
                nameof(ScrollBar) => ScrollBar,
                _ => null,
            };
        }

        internal static BrushSet? ResolveBrushSetPath(this ThemeBrush themeBrush, string path)
        {
            return path switch
            {
                nameof(ThemeBrush.Normal) => themeBrush.Normal,
                nameof(ThemeBrush.Hovered) => themeBrush.Hovered,
                nameof(ThemeBrush.Highlight) => themeBrush.Highlight,
                nameof(ThemeBrush.Disabled) => themeBrush.Disabled,
                _ => null,
            };
        }

        internal static Brush? ResolveBrushPath(this BrushSet brushSet, string path)
        {
            return path switch
            {
                nameof(BrushSet.Background) => brushSet.Background,
                nameof(BrushSet.Foreground) => brushSet.Foreground,
                nameof(BrushSet.Border) => brushSet.Border,
                _ => null,
            };
        }
    }
}
