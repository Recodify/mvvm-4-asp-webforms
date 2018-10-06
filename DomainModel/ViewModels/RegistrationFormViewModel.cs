using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Binding.Helpers;
using System.ComponentModel;
using Binding;

namespace DomainModel.ViewModels
{
    [Serializable]
    public class RegistrationFormViewModel : INotifyPropertyChanged
    {

        public List<string> AvailableTitles { get; set; }

        public List<string> SelectedTitle { get; set; }

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string EmailAddress { get; set; }

        public string Password { get; set; }
        public string ConfirmedPassword { get; set; }

        private string userMessage;
        public string UserMessage 
        {
            get { return userMessage; }
            set
            {
                if (userMessage != value)
                {
                    userMessage = value;
                    OnPropertyChanged("UserMessage");
                }
            }
        }

        private bool showInputForm = true;
        public bool ShowInputForm
        {
            get { return showInputForm; }
            private set
            {
                if (showInputForm != value)
                {
                    showInputForm = value;
                    OnPropertyChanged("ShowInputForm");
                }
            }
        }

        public ICommand SubmitCommand
        {
            get
            {
                return new DelegateCommand((p) => Submit(p), (p) => true);
            }
        }

        public ICommand SubmitWithUnbindCommand
        {
            get
            {
                return new DelegateCommand((p) => SubmitWithUnbind(p), (p) => true);
            }
        }

        private void SubmitWithUnbind(object parameter)
        {
            //If you set the UpdateSourceTrigger to explicit in the view markup, the following code
            //will explicity update the viewmodel from the view. (ie, Execute an Unbind)
            BinderBase binder = parameter as BinderBase;
            binder.ExecuteUnbind();
            
            //this is where you'd talk to the service layer.
            this.UserMessage = "The registration has been submitted to the service layer";
            this.ShowInputForm = false;
        }

        private void Submit(object parameter)
        {
            
            //this is where you'd talk to the service layer.
            this.UserMessage = "The registration has been submitted to the service layer";
            this.ShowInputForm = false;
        }


        #region INotifyPropertyChanged Members
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
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
