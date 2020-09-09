using System;
using System.ComponentModel.DataAnnotations;

namespace Orimath.IO
{
    public sealed class GlobalSetting
    {
        [Range(1.0, 10000.0)]
        public double ViewSize { get; set; } = 512.0;
    }
}
