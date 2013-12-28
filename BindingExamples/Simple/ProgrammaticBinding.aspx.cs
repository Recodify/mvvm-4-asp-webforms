using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Binding;
using DomainModel.ViewModels;
using Binding.Interfaces;

namespace SimpleBinding.Simple
{
    public partial class ProgrammaticBinding : System.Web.UI.Page, IBindingContainer
    {
        private object dataContext = DataHelper.GetEmployeeLeaveViewModel();
        public object DataContext { get { return dataContext; } set { dataContext = value; } }  

        protected override void OnLoad(EventArgs e)
        {
            this.RegisterForBinding();
            base.OnLoad(e);
            CreateBindings();
           
        }

        private void CreateBindings()
        {
            if (!Page.IsPostBack)
            {
                this.CreateBinding(lblEmployeeName, "Text", new Options { Path = "SelectedEmployee.EmployeeName" });
            }
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