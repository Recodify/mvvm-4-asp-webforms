using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Binding.ControlWrappers
{
    public interface IBindingTarget<T> : IBindingTarget
    {
        T TargetInstance { get; }
    }
    
    public interface IBindingTarget
    {
        string UniqueID { get; }
        string ID { get; }
        bool Visible { get; set; }
        
    }
}
