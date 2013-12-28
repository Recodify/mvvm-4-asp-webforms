using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.Interfaces;
using Binding.Services;

namespace Binding
{
    public class StatelessBinder : BinderBase
    {

        /// <summary>
        /// The datacontext is always retreived from the view.
        /// </summary>
        public override object DataContext
        {
            get
            {
                return BindingContainer.DataContext;
            }
        }


        public StatelessBinder(IBindingContainer bindingContainer, IDataStorageService dataStorageService, IControlService controlService)
            : base(bindingContainer, dataStorageService, controlService)
        {

        }

    }
}
