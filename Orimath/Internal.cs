using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;

namespace Orimath
{
    internal static class Internal
    {
        private static readonly KeyGestureConverter _keyGestureConverter = new KeyGestureConverter();

        public static KeyGesture? ConvertToKeyGesture(string source)
        {
            if (!string.IsNullOrWhiteSpace(source))
            {
                try
                {
                    return(KeyGesture)_keyGestureConverter.ConvertFromInvariantString(source);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public static Expression Convert(this Expression expression, Type type) =>
            Expression.Convert(expression, type);

        public static Expression Property(this Expression expression, PropertyInfo propertyInfo) =>
            Expression.Property(expression, propertyInfo);

        public static Expression Assign(this Expression left, Expression right) =>
            Expression.Assign(left, right);

        public static TDelegate CompileLambda<TDelegate>(this Expression body, params ParameterExpression[] parameters)
            where TDelegate : Delegate =>
            Expression.Lambda<TDelegate>(body, parameters).Compile();
    }
}
