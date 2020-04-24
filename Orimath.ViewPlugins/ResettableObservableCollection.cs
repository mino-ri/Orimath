using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Orimath.ViewPlugins
{
    public class ResettableObservableCollection<T> : ObservableCollection<T>
    {
        public void Reset(IEnumerable<T> newItems)
        {
            Items.Clear();
            ((List<T>)Items).AddRange(newItems);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
