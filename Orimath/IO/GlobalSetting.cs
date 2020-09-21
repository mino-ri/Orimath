using System;
using System.ComponentModel.DataAnnotations;

namespace Orimath.IO
{
    public sealed class GlobalSetting
    {
        [Display(Name = "表示サイズ")]
        [Range(1.0, 10000.0)]
        public double ViewSize { get; set; } = 512.0;

        [Editable(false)]
        public double Height { get; set; } = 600.0;

        [Editable(false)]
        public double Width { get; set; } = 800.0;

        [Editable(false)]
        public double Left { get; set; } = double.NaN;

        [Editable(false)]
        public double Top { get; set; } = double.NaN;
    }
}
