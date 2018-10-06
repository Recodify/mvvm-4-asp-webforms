using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Binding;
using DomainModel.BusinessObjects;

namespace SimpleBinding
{
   
    [Serializable]
    public class AddressViewModel : INotifyPropertyChanged
    {
        #region Properties 

        private string id;
        public string ID
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        private string userName;
        public string UserName {get { return userName;}
            set
            {
                if (userName != value)
                {
                    userName = value;
                    OnPropertyChanged("UserName");
                }
            }
        }

        private NotifyPropertyCollection<Address> availableAddresses;
        public NotifyPropertyCollection<Address> AvailableAddresses
        {
            get { return availableAddresses; }
            set
            {
                if (availableAddresses != value)
                {
                    availableAddresses = value;
                    availableAddresses.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
                }
            }
        }

        private Address selectedAddress;
        public Address SelectedAddress
        {
            get { return selectedAddress; }
            set
            {
                if (selectedAddress != value)
                {
                    selectedAddress = value;
                    selectedAddress.PropertyChanged += new PropertyChangedEventHandler(OnPropertyChanged);
                }
            }
        }

        #endregion 

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        #endregion 
    }

    
}