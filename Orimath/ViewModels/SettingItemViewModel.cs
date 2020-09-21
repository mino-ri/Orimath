using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Mvvm;
using Orimath.Reflection;

namespace Orimath.ViewModels
{
    public abstract class SettingItemViewModel : NotifyPropertyChanged
    {
        private readonly object _obj;
        private readonly PropertyAccessor _accessor;

        public string Name { get; }

        protected PropertyInfo PropertyInfo { get; }

        [return: MaybeNull]
        protected T GetValue<T>() => (T)_accessor.GetValue(_obj);

        protected void SetValue<T>([AllowNull] T value) => _accessor.SetValue(_obj, value);

        public SettingItemViewModel(PropertyInfo property, object obj)
        {
            PropertyInfo = property;
            _accessor = PropertyAccessor.GetInstance(property);
            _obj = obj;

            Name = property.GetCustomAttribute<DisplayAttribute>() is { } display
                ? display.Name
                : property.Name;
        }
    }

    public abstract class RangeSettingItemViewModel<T> : SettingItemViewModel
    {
        public T Value { get => GetValue<T>()!; set => SetValue(value); }

        public bool HasRange { get; }

        public T Maximum { get; }

        public T Minimum { get; }

        protected abstract T DefaultMaximum { get; }

        protected abstract T DefaultMinimum { get; }

        protected RangeSettingItemViewModel(PropertyInfo property, object obj)
            : base(property, obj)
        {
            if (property.GetCustomAttribute<RangeAttribute>() is { } range)
            {
                HasRange = true;
                Maximum = (T)range.Maximum;
                Minimum = (T)range.Minimum;
            }
            else
            {
                Maximum = DefaultMaximum;
                Minimum = DefaultMinimum;
            }
        }
    }

    public class DoubleSettingItemViewModel : RangeSettingItemViewModel<double>
    {
        protected override double DefaultMaximum => double.MaxValue;

        protected override double DefaultMinimum => double.MinValue;

        public DoubleSettingItemViewModel(PropertyInfo property, object obj)
            : base (property, obj) { }
    }

    public class Int32SettingItemViewModel : RangeSettingItemViewModel<int>
    {
        protected override int DefaultMaximum => int.MaxValue;

        protected override int DefaultMinimum => int.MinValue;

        public Int32SettingItemViewModel(PropertyInfo property, object obj)
            : base(property, obj) { }
    }

    public class BooleanSettingItemViewModel : SettingItemViewModel
    {
        public bool Value { get => GetValue<bool>(); set => SetValue(value); }

        public BooleanSettingItemViewModel(PropertyInfo property, object obj)
            : base(property, obj) { }
    }

    public class StringSettingItemViewModel : SettingItemViewModel
    {
        public string? Value { get => GetValue<string>(); set => SetValue(value); }

        public int MaxLength { get; }

        public StringSettingItemViewModel(PropertyInfo property, object obj)
            : base(property, obj)
        {
            if (property.GetCustomAttribute<StringLengthAttribute>() is { } stringLength)
            {
                MaxLength = stringLength.MaximumLength;
            }
        }
    }

    public class EnumSettingItemViewModel : SettingItemViewModel
    {
        public EnumValueViewModel[] Choices { get; }

        public EnumValueViewModel Value
        {
            get
            {
                var value = GetValue<Enum>();
                return Choices.First(v => v.Value.Equals(value));
            }
            set => SetValue(value.Value);
        }

        public EnumSettingItemViewModel(PropertyInfo property, object obj)
            : base(property, obj)
        {
            Choices = EnumValueAccessor.GetValues(property.PropertyType);
        }
    }
}
