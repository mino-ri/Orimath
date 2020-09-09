using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Orimath.IO;

namespace Orimath.Plugins
{
    internal static class PluginExecutor
    {
        public static Type[] LoadedPluginTypes { get; private set; } = Type.EmptyTypes;

        public static Type[] LoadedViewPluginTypes { get; private set; } = Type.EmptyTypes;

        public static PluginSetting Setting { get; private set; } = new PluginSetting();

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
                    PluginOrder = GetFullNames<IPlugin>(types),
                    ViewPluginOrder = GetFullNames<IViewPlugin>(types)
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

        private static IEnumerable<T> GetInstances<T>(Type[] types, string[] order)
            where T : class
        {
            var targetTypes = types
                .Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t))
                .ToDictionary(t => t.FullName);

            foreach (var fullName in order)
            {
                if (targetTypes.TryGetValue(fullName, out var type))
                {
                    var instance = (T?)Activator.CreateInstance(type);
                    if (instance is { }) yield return instance;
                }
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

            foreach (var plugin in GetInstances<IPlugin>(types, setting.PluginOrder)) plugin.Execute(args);
            foreach (var plugin in GetInstances<IViewPlugin>(types, setting.ViewPluginOrder)) plugin.Execute(viewArgs);

            return GetViewTypes(types);
        }
    }
}
