using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;
using System.Web.UI;
using Binding.ControlWrappers;
using ASPBinding.Services;

namespace Binding
{

    public class OptionsCollection :  List<Options>
    {
    }

    public class BindingOptionsControl : WebControl
    {
        public StateMode StateMode { get; set; }
        public UpdateSourceTrigger UpdateSourceTrigger { get; set; }
        public OptionsCollection Resources { get; set; }

        protected override void OnInit(EventArgs e)
        {
            IBindingTarget page = this.Page as IBindingTarget;
            if (page == null)
                throw new InvalidOperationException("This control can only be used on pages that implement IBindingTarget");

            WebformsControlService controlService = new WebformsControlService();

            IBindingTarget ctrl = controlService.FindControlRecursive(page, this.GetType());
            if (ctrl != null && ctrl.UniqueID != this.UniqueID)
                throw new InvalidOperationException("Only one control of type BindingOptions can appear of each page");
            
            base.OnInit(e);
        }
    }
}
