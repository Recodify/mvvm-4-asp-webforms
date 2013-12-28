using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Binding.Interfaces;
using System.Text.RegularExpressions;

namespace Binding
{
    public class CommandBindingInstruction : IBindingInstruction
    {
        public string RawSection { get; set; }

        public string GetAssignedProperty()
        {
            string assignedProperty = string.Empty;
            if (!string.IsNullOrEmpty(RawSection))
            {
                assignedProperty = Regex.Match(RawSection, "ldstr \"On[a-zA-Z0-9]*\"").Value;
                assignedProperty = assignedProperty.Replace("ldstr \"On", "");
                assignedProperty = assignedProperty.Replace("\"", "");
            }

            return assignedProperty;
        }
    }
}
