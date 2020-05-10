using System;
using System.Collections.Generic;
using System.Linq;
using Mvvm;
using Orimath.Plugins;

namespace Orimath.Basics.View.ViewModels
{
    public sealed class AttachedObservableCollection<TModel, TViewModel> : ResettableObservableCollection<TViewModel>, IDisposable
    {
        private readonly IDispatcher _dispatcher;
        private readonly Action<CollectionChangedEventHandler<TModel>> _removeHandler;
        private readonly Func<TModel, TViewModel> _mapper;
        private readonly Action<TViewModel> _onRemove;

        public AttachedObservableCollection(
            IDispatcher dispatcher,
            IEnumerable<TModel> init,
            Action<CollectionChangedEventHandler<TModel>> addHandler,
            Action<CollectionChangedEventHandler<TModel>> removeHandler,
            Func<TModel, TViewModel> mapper,
            Action<TViewModel> onRemove)
            : base(init.Select(mapper))
        {
            _dispatcher = dispatcher;
            _removeHandler = removeHandler;
            _mapper = mapper;
            _onRemove = onRemove;
            addHandler(Source_CollectionChanged);
        }

        private async void Source_CollectionChanged(object sender, CollectionChange<TModel> e)
        {
            await _dispatcher.SwitchToUI();
            switch (e)
            {
                case CollectionChange<TModel>.Add add:
                    foreach (var item in add.items) Add(_mapper(item));
                    break;

                case CollectionChange<TModel>.Remove remove:
                    var length = Count;
                    for (var index = length - 1; index >= length - remove.items.Count; index--)
                    {
                        _onRemove(this[index]);
                        RemoveAt(index);
                    }
                    break;

                case CollectionChange<TModel>.Replace replace:
                    _onRemove(this[replace.index]);
                    this[replace.index] = _mapper(replace.newItem);
                    break;

                case CollectionChange<TModel>.Reset reset:
                    foreach (var item in this) _onRemove(item);
                    Reset(reset.newItems.Select(_mapper));
                    break;
            }
        }

#pragma warning disable CA1063 // Implement IDisposable Correctly
        public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
        {
            _removeHandler(Source_CollectionChanged);
        }
    }
}
