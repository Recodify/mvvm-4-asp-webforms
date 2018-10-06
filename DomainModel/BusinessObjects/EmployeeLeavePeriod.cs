using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DomainModel.BusinessObjects
{
    [Serializable]
    public class EmployeeLeavePeriod: INotifyPropertyChanged
    {
        private string employeeName;
        public string EmployeeName 
        {
            get { return employeeName; }
            set
            {
                if (employeeName != value)
                {
                    employeeName = value;
                    OnPropertyChanged("EmployeeName");
                }
            }
        }
        public List<LeavePeriod> LeavePeriods { get; set; }

        #region INotifyPropertyChanged Members

        #region INotifyPropertyChanged Memebers

        [field: NonSerialized]
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



        #endregion
    }
}
