using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Binding.Helpers;

namespace DomainModel.ViewModels
{
    [Serializable]
    public class CommandDemoViewModel
    {
        private bool buttonClicked;

        public ICommand OnClick
        {
            get
            {
                return new DelegateCommand((p) => { buttonClicked = true; }, (p) => !buttonClicked, true);
            }
        }

    }
}
