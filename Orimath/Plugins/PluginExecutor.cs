using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Orimath.IO;
using Orimath.Reflection;
using Sssl;

namespace Orimath.Plugins
{
    internal static class PluginExecutor
    {
        public static Type[] LoadedPluginTypes { get; private set; } = Type.EmptyTypes;

        public static Type[] LoadedViewPluginTypes { get; private set; } = Type.EmptyTypes;

        public static PluginSetting Setting { get; private set; } = new PluginSetting();

        public static List<IConfigurablePlugin> ConfigurablePlugins { get; private set; } = new List<IConfigurablePlugin>();

        private static Type[] LoadPluginTypes()
        {
            var pluginDirectory = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly()!.Location)!, "Plugins");

            if (Directory.Exists(pluginDirectory))
            {
                return Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(Assembly.LoadFrom)
                    .SelectMany(asm => asm.GetExportedTypes())
                    .ToArray();
            }
            else
            {
                return Type.EmptyTypes;
            }
        }

        private static PluginSetting LoadSetting(Type[] types)
        {
            Setting = Settings.Load<PluginSetting>(SettingName.Plugin)!;
            if (Setting is null)
            {
                Setting = new PluginSetting
                {
                    PluginOrder = GetFullNames<IPlugin>(types)
                        .Concat(GetFullNames<IViewPlugin>(types))
                        .ToArray(),
                };
                SaveSetting();
            }
            
            return Setting;
        }

        public static void SaveSetting()
        {
            Settings.Save(SettingName.Plugin, Setting);
        }

        private static string[] GetFullNames<T>(Type[] types)
              where T : class
        {
            return types
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t))
                    .Select(t => t.FullName!)
                    .ToArray();
        }

        private static void ExecuteCore(PluginSetting setting, PluginArgs args, ViewPluginArgs viewArgs)
        {
            var pluginTypes = LoadedPluginTypes.ToDictionary(t => t.FullName);
            var viewPluginTypes = LoadedViewPluginTypes.ToDictionary(t => t.FullName);

            foreach (var fullName in setting.PluginOrder)
            {
                if (pluginTypes.TryGetValue(fullName, out var type) &&
                    Activator.CreateInstance(type) is IPlugin plugin)
                {
                    SetSetting(plugin, fullName, setting);
                    plugin.Execute(args);
                }

                if (viewPluginTypes.TryGetValue(fullName, out type) &&
                    Activator.CreateInstance(type) is IViewPlugin viewPlugin)
                {
                    SetSetting(viewPlugin, fullName, setting);
                    viewPlugin.Execute(viewArgs);
                }
            }
        }

        private static void SetSetting(object plugin, string fullName, PluginSetting setting)
        {
            if (plugin is IConfigurablePlugin configurable)
            {
                ConfigurablePlugins.Add(configurable);

                object? targetSetting;
                if (!setting.Settings.TryGetValue(fullName, out var sssl) ||
                    !sssl!.TryConvertTo(configurable.SettingType, out targetSetting))
                {
                    try
                    {
                        targetSetting = FastActivator.CreateInstance(configurable.SettingType);
                        setting.Settings[fullName] = SsslObject.ConvertFrom(targetSetting);
                    }
                    catch
                    {
                        targetSetting = null;
                    }

                }

                if (targetSetting is { })
                    configurable.Setting = targetSetting;
                else if (configurable.Setting is { })
                    setting.Settings[fullName] = SsslObject.ConvertFrom(configurable.Setting);
            }
        }

        private static IEnumerable<(Type, ViewAttribute)> GetViewTypes(Type[] types)
        {
            return types
                .Where(t => t.IsClass && !t.IsAbstract && typeof(FrameworkElement).IsAssignableFrom(t))
                .Select(t => (t, t.GetCustomAttribute<ViewAttribute>()!))
                .Where(tuple => tuple.Item2 is { });
        }

        public static IEnumerable<(Type, ViewAttribute)> ExecutePlugins(ViewPluginArgs viewArgs)
        {
            var args = new PluginArgs(viewArgs.Workspace);
            var types = LoadPluginTypes();
            var setting = LoadSetting(types);
            LoadedPluginTypes = types.Where(t => t.IsClass && !t.IsAbstract && typeof(IPlugin).IsAssignableFrom(t)).ToArray();
            LoadedViewPluginTypes = types.Where(t => t.IsClass && !t.IsAbstract && typeof(IViewPlugin).IsAssignableFrom(t)).ToArray();
            ExecuteCore(setting, args, viewArgs);

            return GetViewTypes(types);
        }
    }
}
