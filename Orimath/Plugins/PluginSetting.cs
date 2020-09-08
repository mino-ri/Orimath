using System;
using System.Collections.Generic;
using System.Text;

namespace Orimath.Plugins
{
    public class PluginSetting
    {
        public string[] PluginOrder { get; set; } = Array.Empty<string>();

        public string[] ViewPluginOrder { get; set; } = Array.Empty<string>();
    }
}
