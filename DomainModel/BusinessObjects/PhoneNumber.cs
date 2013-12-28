using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DomainModel.BusinessObjects
{
    [Serializable]
    public class PhoneNumber : INotifyPropertyChanged
    {
        private long number;
        public long Number
        {
            get { return number; }
            set
            {
                if (number != value)
                {
                    number = value;
                }
            }

        }

        #region INotifyPropertyChanged Memebers

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
