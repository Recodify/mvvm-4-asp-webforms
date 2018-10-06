using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace BinderTests.MockHelpers
{
    public class DynamicItemTemplate : ITemplate
    {
        #region ITemplate Members
        private List<Func<Control>> controls;
        
        public DynamicItemTemplate(List<Func<Control>> controls)
        {
            this.controls = controls;
        }


        public void InstantiateIn(Control container)
        {
            foreach (Func<Control> control in this.controls)
            {
                container.Controls.Add(control.Invoke());
            }
        }

        #endregion
    }
}
