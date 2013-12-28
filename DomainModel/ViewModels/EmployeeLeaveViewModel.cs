using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainModel.BusinessObjects;
using Binding;
using System.ComponentModel;
using System.Windows.Input;
using Binding.ControlWrappers;

namespace DomainModel.ViewModels
{

    public class DoSomething : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            BinderBase target = (BinderBase)parameter;
            EmployeeLeaveViewModel model = (EmployeeLeaveViewModel)target.DataContext;
            model.EmployeeName = "I've been changed by the command";
        }

        #endregion
    }


    [Serializable]
    public class EmployeeLeaveViewModel: INotifyPropertyChanged
    {


        public ICommand DoSomethingCommand
        {
            get
            {
                return new DoSomething();               
            }
        }

        public string EmployeeName
        {
            get
            {
                return selectedEmployee.EmployeeName;
            }
            set
            {
                if (selectedEmployee.EmployeeName != value)
                {
                    selectedEmployee.EmployeeName = value;
                    OnPropertyChanged("EmployeeName");
                }
            }
        }

        private EmployeeLeavePeriod selectedEmployee;
        public EmployeeLeavePeriod SelectedEmployee 
        {
            get { return selectedEmployee; }
            set
            {
                if (selectedEmployee != value)
                {
                    selectedEmployee = value;
                    OnPropertyChanged("SelectedEmployee");

                    if (selectedEmployee != null)
                        selectedEmployee.PropertyChanged += (s,e) => OnPropertyChanged("EmployeeName");
                }
            }
        }
        public NotifyPropertyCollection<EmployeeLeavePeriod> AvailableEmployees { get; set; }

        #region INotifyPropertyChanged Memebers

        //If we serialize this ViewModel to ViewState we do not want to store any subscribed listeners!
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
    }
}
