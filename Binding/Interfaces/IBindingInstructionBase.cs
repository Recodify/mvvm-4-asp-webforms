using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binding.Interfaces
{
    /// <summary>
    /// Abstraction of the actual data bind instruction as extracted from IL
    /// </summary>
    public interface IBindingInstruction
    {
        string RawSection { get; set; }
        string GetAssignedProperty();
    }
}
