using System;
using System.Linq;
using Mvvm;
using Orimath.Plugins;
using ApplicativeProperty;

namespace Orimath.Basics.View.ViewModels
{
    public sealed class AttachedObservableCollection<TModel, TViewModel> : ResettableObservableCollection<TViewModel>, IDisposable
    {
        private readonly IDisposable _disposer;
        private readonly IDispatcher _dispatcher;
        private readonly Func<TModel, TViewModel> _mapper;
        private readonly Action<TViewModel> _onRemove;

        public AttachedObservableCollection(
            IDispatcher dispatcher,
            IReactiveCollection<TModel> source,
            Func<TModel, TViewModel> mapper,
            Action<TViewModel> onRemove)
            : base(source.Select(mapper))
        {
            _dispatcher = dispatcher;
            _mapper = mapper;
            _onRemove = onRemove;
            _disposer = source.Subscribe(Source_CollectionChanged);
        }

        private async void Source_CollectionChanged(CollectionChange<TModel> e)
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

        public void Dispose()
        {
            _disposer.Dispose();
        }
    }
}
