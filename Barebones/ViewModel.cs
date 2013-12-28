using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Input;

namespace Barebones
{
    [Serializable]
    public class ViewModel
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedDate { get; set; }

        public ClickCommand OnClick
        {
            get
            {
                return new ClickCommand();
            }
        }

        public ViewModel()
        {
            //Just some default values so we see something on the screen.
            //In a real world scenerio, these would be loaded from the model.
            ID = 1;
            FirstName = "Dave";
            LastName = "Smith";
            CreatedDate = new DateTime(1983, 07, 01);
        }

    }

    public class ClickCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            
        }

        #endregion
    }
}