using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.ControlWrappers;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASPBinding.ControlWrappers
{

    public class WebformControl<T> : WebformControl, IBindingTarget<T> where T: class
    {

        public WebformControl(Control control) : base(control)
        {

        }

        public T TargetInstance
        {
            get { return this.Control as T; }
        }

    }

    public class WebformControl : IBindingTarget
    {

        private Control control;
        public Control Control
        {
            get { return control; }
        }

        public WebformControl(Control control)
        {
            this.control = control;
        }

        public string UniqueID
        {
            get { return control.UniqueID; }
        }

        public string ID
        {
            get { return control.ID; }
        }


        public System.Collections.IEnumerable Controls
        {
            get { return control.Controls; }
        }

        public IBindingTarget Parent
        { 
            get { return new WebformControl(control.Parent); } 
        }

        public bool Visible
        {
            get
            {
                WebControl webControl = control as WebControl;
                if (webControl != null)
                    return webControl.Enabled;

                return control.Visible;
            }
            set
            {
                WebControl webControl = control as WebControl;
                if (webControl != null)
                    webControl.Enabled = value;
                else
                    control.Visible = value;
               
            }

        }

    }
}
