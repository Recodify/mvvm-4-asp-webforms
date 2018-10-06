using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binding.Services
{
    public static class Logger
    {
        public static void Trace(Exception e)
        {
            System.Diagnostics.Debug.Write(string.Format("Binding Error - {0}", e.Message));
        }

        public static void Trace(string message)
        {
            System.Diagnostics.Debug.Write(string.Format("Binding Error - {0}", message));
        }
    }
}
