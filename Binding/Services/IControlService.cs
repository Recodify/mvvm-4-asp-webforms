using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.ControlWrappers;

namespace Binding.Services
{
    public interface IControlService
    {
        IBindingTarget FindControlUnique(IBindingTarget root, string unique);
        IBindingTarget FindControlRecursive(IBindingTarget root, string id);
        IBindingTarget FindControlRecursive(IBindingTarget root, Type type);
        object Unwrap(IBindingTarget wrapper);
        IBindingTarget GetParent(IBindingTarget child);
        bool TryGetTargetPropertyName(IBindingTarget target, Options options, out string targetPropertyName);

    }
}
