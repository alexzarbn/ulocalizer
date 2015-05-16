using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Threading;

namespace ULocalizer.Classes
{
    [Serializable]
    public class CObservableList<T> : ObservableCollection<T>
    {
        private readonly Dispatcher _dispatcher;
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        protected override async void ClearItems()
        {
            if (_dispatcher.CheckAccess())
            {
                base.ClearItems();
            }
            else
            {
                await _dispatcher.InvokeAsync(ClearItems);
            }
        }

        protected override async void RemoveItem(int index)
        {
            if (_dispatcher.CheckAccess())
            {
                base.RemoveItem(index);
            }
            else
            {
                await _dispatcher.InvokeAsync(() => RemoveItem(index));
            }
        }

        protected override async void InsertItem(int index, T item)
        {
            if (_dispatcher.CheckAccess())
            {
                base.InsertItem(index, item);
            }
            else
            {
                await _dispatcher.InvokeAsync(() => InsertItem(index, item), DispatcherPriority.Background);
            }
        }

        protected override async void SetItem(int index, T item)
        {
            if (_dispatcher.CheckAccess())
            {
                base.SetItem(index, item);
            }
            else
            {
                await _dispatcher.InvokeAsync(() => SetItem(index, item));
            }
        }

        protected override async void MoveItem(int oldIndex, int newIndex)
        {
            if (_dispatcher.CheckAccess())
            {
                base.MoveItem(oldIndex, newIndex);
            }
            else
            {
                await _dispatcher.InvokeAsync(() => MoveItem(oldIndex, newIndex));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            // Be nice - use BlockReentrancy like MSDN said
            using (BlockReentrancy())
            {
                var eventHandler = CollectionChanged;
                if (eventHandler != null)
                {
                    var delegates = eventHandler.GetInvocationList();
                    // Walk thru invocation list
                    foreach (var @delegate in delegates)
                    {
                        var handler = (NotifyCollectionChangedEventHandler) @delegate;
                        var dispatcherObject = handler.Target as DispatcherObject;
                        // If the subscriber is a DispatcherObject and different thread
                        if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                            // Invoke handler in the target dispatcher's thread
                            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                        else // Execute handler as is
                            handler(this, e);
                    }
                }
            }
        }

        #region Constructors

        public CObservableList(Dispatcher dispatcher = null)
        {
            _dispatcher = dispatcher ?? (Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher);
        }

        public CObservableList(IEnumerable<T> collection, Dispatcher dispatcher = null) : base(collection)
        {
            _dispatcher = dispatcher ?? (Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher);
        }

        public CObservableList(List<T> list, Dispatcher dispatcher = null) : base(list)
        {
            _dispatcher = dispatcher ?? (Application.Current != null ? Application.Current.Dispatcher : Dispatcher.CurrentDispatcher);
        }

        #endregion
    }
}