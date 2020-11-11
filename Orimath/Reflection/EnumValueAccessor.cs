using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Orimath.ViewModels;

namespace Orimath.Reflection
{
    public static class EnumValueAccessor
    {
        private static readonly ConcurrentDictionary<Type, EnumValueViewModel[]> _cache =
            new ConcurrentDictionary<Type, EnumValueViewModel[]>();

        public static EnumValueViewModel[] GetValues(Type type)
        {
            return _cache.GetOrAdd(type, t =>
                t.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Select(field => new EnumValueViewModel(
                    (Enum)Enum.Parse(t, field.Name),
                    field.GetCustomAttribute<DisplayAttribute>()?.Name ?? field.Name))
                .ToArray());
        }
    }
}
