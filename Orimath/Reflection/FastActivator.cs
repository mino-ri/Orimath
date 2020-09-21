using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Orimath.Reflection
{
    internal static class FastActivator
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> _constructors =
            new ConcurrentDictionary<Type, Func<object>>();

        public static object CreateInstance(Type type)
        {
            var constructor = _constructors.GetOrAdd(type, t =>
                Expression.New(t)
                .Convert(typeof(object))
                .CompileLambda<Func<object>>());

            return constructor();
        }
    }
}
