﻿using System;
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
            var disabledBackground = Solid(242, 242, 242);
            var disabledForeground = Solid(166, 166, 166);
            var back = Solid(240, 250, 245);

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
                Hovered = new BrushSet(Solid(210, 240, 225), Brushes.Black, hover),
                Highlight = new BrushSet(highlightLight, Brushes.Black, highlightBorder),
                Disabled = new BrushSet(disabledBackground, disabledForeground, disabledForeground),
            };

            Input = new ThemeBrush
            {
                Normal = new BrushSet(Brushes.White, Brushes.Black, normal),
                Hovered = new BrushSet(Brushes.White, Brushes.Black, hover),
                Highlight = new BrushSet(Brushes.White, Brushes.Black, highlight),
                Disabled = new BrushSet(disabledBackground, disabledForeground, disabledForeground),
            };

            Selector = new ThemeBrush
            {
                Normal = new BrushSet(Brushes.White, Brushes.Black, normal),
                Hovered = new BrushSet(back, Brushes.Black, hover),
                Highlight = new BrushSet(highlightLight, Brushes.Black, highlightBorder),
                Disabled = new BrushSet(disabledBackground, disabledForeground, disabledForeground),
            };

            ScrollBar = new ThemeBrush
            {
                Normal = new BrushSet(back, Solid(191, 219, 205), null),
                Hovered = new BrushSet(back, hover, null),
                Highlight = new BrushSet(back, normal, null),
                Disabled = new BrushSet(disabledBackground, Solid(200, 200, 200), null),
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