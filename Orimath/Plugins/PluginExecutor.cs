using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Orimath.Plugins
{
    internal static class PluginExecutor
    {
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

        private static IEnumerable<T> GetInstances<T>(Type[] types)
            where T : class
        {
            return types
                .Where(t => t.IsClass && !t.IsAbstract && typeof(T).IsAssignableFrom(t))
                .Select(t => (T)Activator.CreateInstance(t)!)
                .Where(obj => obj is { });
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
            foreach (var plugin in GetInstances<IPlugin>(types)) plugin.Execute(args);
            foreach (var plugin in GetInstances<IViewPlugin>(types)) plugin.Execute(viewArgs);

            return GetViewTypes(types);
        }
    }
}
