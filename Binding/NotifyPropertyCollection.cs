using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Binding 
{

    /// <summary>
    /// Generic container for a collection of INotifyPropertyChanged. Provides auto wiring and raising
    /// of PropertyChanged Events
    /// </summary>
    /// <remarks>
    /// PropertyChanged events are raised against the underlying collection item not from the collection container
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class NotifyPropertyCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        public new event PropertyChangedEventHandler PropertyChanged;

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (INotifyPropertyChanged newItem in e.NewItems)
                {
                    newItem.PropertyChanged += new PropertyChangedEventHandler(newItem_PropertyChanged);
                }
            }

            List<INotifyPropertyChanged> item = new List<INotifyPropertyChanged>();
            item.AddRange(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                return;

            foreach (T itm in items)
            {
                this.Add(itm);
            }
        }

        void newItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, new PropertyChangedEventArgs(e.PropertyName));
            }
        }
    }
}
