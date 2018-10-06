using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binding
{
    public enum BindingMode
    {
        Unset, TwoWay, OneWay, Command, Unsupported
    }

    public enum UpdateSourceTrigger
    {
        /// <summary>
        /// Retreive data from view and update the model automatically on each postback
        /// </summary>
        PostBack,
        /// <summary>
        /// Retreive data from view and update the model only when explicitly instructed 
        /// </summary>
        Explicit
    }

    public enum PathMode
    {

        /// <summary>
        /// The root of the path is considered to be the BindingContext of the parent Binding
        /// </summary>
        Relative,
        /// <summary>
        /// The root of the path is considered to be the BindingContext of the page
        /// </summary>
        Absolute
    }

    public enum StateMode
    {
        /// <summary>
        /// The viewmodel is persisted in viewstate so that it is available across postbacks
        /// </summary>
        Persist,
        /// <summary>
        /// Responsibility for recreation of the viewmodel is deferred to the implementer
        /// </summary>
        Recreate
    }
   
}
