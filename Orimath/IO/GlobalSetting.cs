using System;
using System.ComponentModel.DataAnnotations;

namespace Orimath.IO
{
    public sealed class GlobalSetting : ICloneable
    {
        [Display(Name = "表示サイズ")]
        [Range(1, 5000)]
        public int ViewSize { get; set; } = 512;

        [Editable(false)]
        public double Height { get; set; } = 600.0;

        [Editable(false)]
        public double Width { get; set; } = 800.0;

        [Editable(false)]
        public double Left { get; set; } = double.NaN;

        [Editable(false)]
        public double Top { get; set; } = double.NaN;

        public object Clone() => MemberwiseClone();
    }
}
