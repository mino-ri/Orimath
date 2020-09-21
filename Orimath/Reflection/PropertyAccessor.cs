using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace Orimath.Reflection
{
    internal class PropertyAccessor
    {
        private static readonly ConcurrentDictionary<PropertyInfo, PropertyAccessor> _cache = new ConcurrentDictionary<PropertyInfo, PropertyAccessor>();

        private readonly Func<object, object?> _getValue;
        private readonly Action<object, object?> _setValue;

        public object? GetValue(object instance) => _getValue(instance);

        public void SetValue(object instance, object? value) => _setValue(instance, value);

        private PropertyAccessor(PropertyInfo prop)
        {
            var value = Expression.Parameter(typeof(object), "value");
            var obj = Expression.Parameter(typeof(object), "obj");

            _getValue = obj.Convert(prop.DeclaringType!)
                .Property(prop)
                .Convert(typeof(object))
                .CompileLambda<Func<object, object?>>(obj);

            _setValue = obj.Convert(prop.DeclaringType!)
                .Property(prop)
                .Assign(value.Convert(prop.PropertyType))
                .CompileLambda<Action<object, object?>>(obj, value);
        }

        public static PropertyAccessor GetInstance(PropertyInfo propertyInfo)
        {
            return _cache.GetOrAdd(propertyInfo, prop => new PropertyAccessor(prop));
        }
    }
}
