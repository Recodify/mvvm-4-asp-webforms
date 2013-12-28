using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.Interfaces;
using Binding.Services;

namespace Binding
{
    public class ViewStateBinder : BinderBase
    {

        private const string VIEW_MODEL_KEY = "VIEW_MODEL";

        /// <summary>
        /// Attempt to load the datacontext from storage
        /// </summary>
        public override object DataContext
        {
            get
            {

                if (this.BindingContainer.IsPostBack)
                    //try and set the datacontext from viewstate if this is a postback
                    this.BindingContainer.DataContext = DataStorageService.Retrieve<object>(VIEW_MODEL_KEY);
                else
                    //else store the datacontext in the viewstate
                    DataStorageService.Store(VIEW_MODEL_KEY, this.BindingContainer.DataContext);

                return this.BindingContainer.DataContext;
            }
        }

        public ViewStateBinder(IBindingContainer bindingContainer, IDataStorageService dataStorageService, IControlService controlService)
            : base(bindingContainer, dataStorageService, controlService)
        {

        }

    }
}
