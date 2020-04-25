using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Orimath.ViewPlugins
{
    //public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    //{
    //    public event PropertyChangedEventHandler? PropertyChanged;

    //    protected void OnPropertyChanged([CallerMemberName]string? propertyName = null)
    //    {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //    }

    //    protected bool SetValue(ref string storage, string value, [CallerMemberName] string? propertyName = null)
    //    {
    //        if (storage == value)
    //            return false;

    //        storage = value;
    //        OnPropertyChanged(propertyName);
    //        return true;
    //    }

    //    protected bool SetValue<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    //        where T : notnull, IEquatable<T>
    //    {
    //        if (storage.Equals(value))
    //            return false;

    //        storage = value;
    //        OnPropertyChanged(propertyName);
    //        return true;
    //    }

    //    protected bool SetValue<T>(ref T? storage, T? value, [CallerMemberName] string? propertyName = null)
    //        where T : struct
    //    {
    //        if (Nullable.Equals(storage, value))
    //            return false;

    //        storage = value;
    //        OnPropertyChanged(propertyName);
    //        return true;
    //    }

    //    protected bool SetValueRef<T>(ref T? storage, T? value, [CallerMemberName] string? propertyName = null)
    //        where T : class
    //    {
    //        if (ReferenceEquals(storage, value))
    //            return false;

    //        storage = value;
    //        OnPropertyChanged(propertyName);
    //        return true;
    //    }
    //}
}
