using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding;

namespace BinderTests.MockingInterfaces
{
    public class SimpleViewModel : ISimpleViewModel
    {
        #region ISimpleViewModel Members

        public string EmployeeName
        {
            get;
            set;
        }

        public List<Child> ChildrensNames
        {
            get;
            set;
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    public interface ISimpleViewModel 
    {
        string EmployeeName {get; set;}
        List<Child> ChildrensNames { get; set; }
    }

    public class Child
    {
        public string Name { get; set; }
    }
}
