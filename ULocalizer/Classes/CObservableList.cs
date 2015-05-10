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
        private Dispatcher dispatcher;
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        #region Constructors
        public CObservableList(Dispatcher dispatcher = null)
        {
            this.dispatcher = dispatcher ??
                              (Application.Current != null
                                   ? Application.Current.Dispatcher
                                   : Dispatcher.CurrentDispatcher);
        }
        public CObservableList(IEnumerable<T> collection, Dispatcher dispatcher = null)
            : base(collection)
        {
            this.dispatcher = dispatcher ??
                              (Application.Current != null
                                   ? Application.Current.Dispatcher
                                   : Dispatcher.CurrentDispatcher);
        }
        public CObservableList(List<T> list, Dispatcher dispatcher = null)
            : base(list)
        {
            this.dispatcher = dispatcher ??
                              (Application.Current != null
                                   ? Application.Current.Dispatcher
                                   : Dispatcher.CurrentDispatcher);
        }
        #endregion
        protected override async void ClearItems()
        {
            if (dispatcher.CheckAccess())
            {
                base.ClearItems();
            }
            else
            {
                await dispatcher.InvokeAsync(new Action(ClearItems));
            }
        }
        protected override async void RemoveItem(int index)
        {
            if (dispatcher.CheckAccess())
            {
                base.RemoveItem(index);
            }
            else
            {
                await dispatcher.InvokeAsync(new Action(() => RemoveItem(index)));
            }
        }
        protected override async void InsertItem(int index, T item)
        {
            if (dispatcher.CheckAccess())
            {
                base.InsertItem(index, item);
            }
            else
            {
                await dispatcher.InvokeAsync(new Action(() => InsertItem(index, item)),DispatcherPriority.Background);
            }
        }
        protected override async void SetItem(int index, T item)
        {
            if (dispatcher.CheckAccess())
            {
                base.SetItem(index, item);
            }
            else
            {
                await dispatcher.InvokeAsync(new Action(() => SetItem(index, item)));
            }
        }
        protected override async void MoveItem(int oldIndex, int newIndex)
        {
            if (dispatcher.CheckAccess())
            {
                base.MoveItem(oldIndex, newIndex);
            }
            else
            {
                await dispatcher.InvokeAsync(new Action(() => MoveItem(oldIndex, newIndex)));
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
                    Delegate[] delegates = eventHandler.GetInvocationList();
                    // Walk thru invocation list
                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        var dispatcherObject = handler.Target as DispatcherObject;
                        // If the subscriber is a DispatcherObject and different thread
                        if (dispatcherObject != null && dispatcherObject.CheckAccess() == false)
                            // Invoke handler in the target dispatcher's thread
                            dispatcherObject.Dispatcher.Invoke(DispatcherPriority.DataBind,
                                          handler, this, e);
                        else // Execute handler as is
                            handler(this, e);
                    }
                }
            }
        }
    }
}
