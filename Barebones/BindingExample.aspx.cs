using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
//two using statements
using Binding.Interfaces;
using Binding;

namespace Barebones
{
    /*Implement one interface, with one property only*/
    public partial class BindingExample : System.Web.UI.Page, IBindingContainer
    {

        #region IBindingContainer Members

        private object dataContext = new ViewModel();
        public object DataContext
        {
            get { return dataContext; }
            set { dataContext = value; }
        }

        #endregion

        //override one method
        protected override void OnLoad(EventArgs e)
        {
            //call one method
            this.RegisterForBinding();
            base.OnLoad(e);
        }
       
    }
}