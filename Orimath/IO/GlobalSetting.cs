using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Orimath.IO
{
    public sealed class GlobalSetting
    {
        [Range(1.0, 10000.0)]
        public double ViewSize { get; set; } = 512.0;

        [Browsable(false)]
        public double Height { get; set; } = 600.0;

        [Browsable(false)]
        public double Width { get; set; } = 800.0;

        [Browsable(false)]
        public double Left { get; set; } = double.NaN;

        [Browsable(false)]
        public double Top { get; set; } = double.NaN;
    }
}
