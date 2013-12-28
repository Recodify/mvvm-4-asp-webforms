using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binding;
using Binding.Interfaces;

namespace SimpleBinding
{
    public partial class CascadingExample : System.Web.UI.Page, IBindingContainer
    {

        private object dataContext = DataHelper.GetDemoAddressViewModel();
        public object DataContext { get { return dataContext; } set { dataContext = value; } }    

        protected override void OnLoad(EventArgs e)
        {
            //after view state is available but before the onload event which is listened to by the binder
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