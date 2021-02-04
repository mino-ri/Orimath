using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Orimath.Controls
{
    public class ResettableObservableCollection<T> : ObservableCollection<T>
    {
        public ResettableObservableCollection() : base() { }

        public ResettableObservableCollection(IEnumerable<T> collection) : base(collection) { }

        public ResettableObservableCollection(List<T> list) : base(list) { }

        public void Reset(IEnumerable<T> newItems)
        {
            Items.Clear();
            ((List<T>)Items).AddRange(newItems);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        }
    }
}
