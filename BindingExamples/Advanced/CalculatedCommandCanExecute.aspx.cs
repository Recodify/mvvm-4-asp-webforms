using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binding.Interfaces;
using Binding;
using DomainModel.ViewModels;

namespace SimpleBinding.Advanced
{
    public partial class CalculatedCommandCanExecute : System.Web.UI.Page, IBindingContainer
    {
     
        private object dataContext = new CommandDemoViewModel();
        public object DataContext { get { return dataContext; } set { dataContext = value; } }

        protected override void OnLoad(EventArgs e)
        {
            this.RegisterForBinding();
            base.OnLoad(e);
        }

        /// <summary>
        /// Place breakpoint here and inspect the BindingContext to see if values have been mapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {

        }
    }
}