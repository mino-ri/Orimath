using System;

namespace Orimath.ViewModels
{
    public class EnumValueViewModel
    {
        public Enum Value { get; }

        public string Name { get; }

        public EnumValueViewModel(Enum value, string name)
        {
            Value = value;
            Name = name;
        }

        public override string ToString() => Name;
    }
}
