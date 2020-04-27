using System;

namespace Orimath.Plugins
{
    internal static class FastActivator
    {
        public static object? CreateInstance(Type type) => Activator.CreateInstance(type);
    }
}
