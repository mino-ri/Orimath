using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Threading.Tasks;
using Orimath.Controls;

namespace Orimath.ViewModels
{
    public class SettingViewModel
    {
        private readonly object _obj;
        private bool _loaded;
        private readonly ResettableObservableCollection<SettingItemViewModel> _items = new();

        public ResettableObservableCollection<SettingItemViewModel> Items
        {
            get
            {
                if (!_loaded) LoadItems();
                return _items;
            }
        }

        public SettingViewModel(object obj)
        {
            _obj = obj;
        }

        private async void LoadItems()
        {
            _loaded = true;

            var values = await Task.Run(() =>
            {
                var result = new List<SettingItemViewModel>();

                foreach (var prop in _obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (prop.GetGetMethod() is null || prop.GetSetMethod() is null)
                        continue;

                    if (prop.GetCustomAttribute<EditableAttribute>() is { } editable &&
                        !editable.AllowEdit)
                        continue;

                    var propertyType = prop.PropertyType;

                    if (propertyType == typeof(double))
                        result.Add(new DoubleSettingItemViewModel(prop, _obj));
                    else if (propertyType == typeof(int))
                        result.Add(new Int32SettingItemViewModel(prop, _obj));
                    else if (propertyType == typeof(bool))
                        result.Add(new BooleanSettingItemViewModel(prop, _obj));
                    else if (propertyType == typeof(string))
                        result.Add(new StringSettingItemViewModel(prop, _obj));
                    else if (propertyType.IsEnum)
                        result.Add(new EnumSettingItemViewModel(prop, _obj));
                }

                return result;
            });

            _items.Reset(values);
        }
    }
}
