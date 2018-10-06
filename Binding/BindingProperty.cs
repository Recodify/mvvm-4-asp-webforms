using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Collections;
using System.Web.UI;
using System.Reflection;
using System.Windows.Data;
using Binding.Services;
using Binding.ControlWrappers;
using Binding.Interfaces;

namespace Binding
{

    public class TargetEvent
    {
        /// <summary>
        /// A PropertyDescriptor for the target property
        /// </summary>
        public EventInfo Descriptor { get; set; }

        /// <summary>
        /// The an instance of the control that owns the target property
        /// </summary>
        public object OwningControl { get; set; }

        /// <summary>
        /// The current value of the target property
        /// </summary>
        public object Value { get; set; }
    }

    public class TargetProperty
    {
        /// <summary>
        /// A PropertyDescriptor for the target property
        /// </summary>
        public PropertyDescriptor Descriptor { get; set; }

        /// <summary>
        /// The an instance of the wrapped control that owns the target property
        /// </summary>
        public IBindingTarget OwningControl { get; set; }

        /// <summary>
        /// The an instance of the control that owns the target property
        /// </summary>
        public object OwningControlRaw { get; set; }
        
        /// <summary>
        /// The current value of the target property
        /// </summary>
        public object Value { get; set; }
    }

    /// <summary>
    /// Represents the source property in a data binding
    /// </summary>
    public class SourceProperty
    {
        /// <summary>
        /// A PropertyDescriptor for the source property
        /// </summary>
        public PropertyDescriptor Descriptor { get; set; }
       
        /// <summary>
        /// The class instance to which the property with the current value belongs
        /// </summary>
        public object OwningInstance { get; set; }


        /// <summary>
        /// The current value of the source property
        /// </summary>
        public object Value { get; set; }
    }
   
}
