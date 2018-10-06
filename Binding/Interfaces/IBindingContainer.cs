using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Binding.ControlWrappers;
using Binding.Services;
using System.ComponentModel;

namespace Binding.Interfaces
{
    

    /// <summary>
    /// Strongly typed IBindingContainer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBindingContainer<T> : IBindingContainer
    {
        T TypedDataContext { get;}
    }

    /// <summary>
    /// Implement this interface on your control container (page, form, window etc) in order facilitate databinding
    /// </summary>
    public interface IBindingContainer: IBindingTarget
    {
        //If implementing this interface on the a System.Web.UI.Page you
        //are only required to implement this one property explictly
        /// <summary>
        /// The ViewModel used to supply data to and receive values from the view
        /// </summary>
        /// <remarks>If you want cascading updates to work then this object must implement INotifyPropertyChanged</remarks>
        //object BindingContext { get; set; }

        //All implemented by System.Web.UI.Page
        void DataBind();
        bool IsPostBack { get; }
        event EventHandler Load;

        object DataContext { get; set; }
    }
}
