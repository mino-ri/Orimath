using Sssl;
using System;
using System.Collections.Generic;

namespace Orimath.IO
{
    public sealed class PluginSetting
    {
        public string[] PluginOrder { get; set; } = Array.Empty<string>();

        public Dictionary<string, SsslObject> Settings { get; set; } = new Dictionary<string, SsslObject>();
    }
}
