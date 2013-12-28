using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DomainModel.BusinessObjects
{
    [Serializable]
    public class Address : INotifyPropertyChanged
    {
        #region Properties

        private List<PhoneNumber> phoneNumbers;
        public List<PhoneNumber> PhoneNumbers
        {
            get { return phoneNumbers; }
            set
            {
                //HACK: no change notification currently implemented which 
                //will mean that cascading updates will not work.
                phoneNumbers = value;
            }

        }


        private string houseNameNumber;
        public string HouseNameNumber
        {
            get { return houseNameNumber; }
            set
            {
                if (houseNameNumber != value)
                {
                    houseNameNumber = value;
                    OnPropertyChanged("HouseNameNumber");
                }
            }

        }

        private string street;
        public string Street
        {
            get { return street; }
            set
            {
                if (street != value)
                {
                    street = value;
                    OnPropertyChanged("Street");
                }
            }

        }

        private string postcode;
        public string Postcode
        {
            get { return postcode; }
            set
            {
                if (postcode != value)
                {
                    postcode = value;
                    OnPropertyChanged("Postcode");
                }
            }
        }


        #endregion

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
