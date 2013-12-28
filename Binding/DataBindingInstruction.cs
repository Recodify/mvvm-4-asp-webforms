using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Binding.Interfaces;

namespace Binding
{
    public class DataBindingInstruction : IBindingInstruction
    {
        public string RawSection { get; set; }

        public string GetAssignedProperty()
        {
            string assignedProperty = string.Empty;
            if (!string.IsNullOrEmpty(RawSection))
            {
                assignedProperty = Regex.Match(RawSection, "[^_]*$").Value;

            }

            return assignedProperty;
        }

    }
}
