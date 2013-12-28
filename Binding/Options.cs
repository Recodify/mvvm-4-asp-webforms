using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Binding
{
    [Serializable]
    public class Options
    {
        /// <summary>
        /// An ID to uniquely identify this set of options
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The source path
        /// </summary>
        public string Path { get; set; }

        public BindingMode Mode { get; set; }
        public PathMode PathMode { get; set; }
        public string Converter { get; set; }
        public bool IsAuthorative { get; set; }

        /// <summary>
        /// Load a binding object with data from an object that contains properties of the same name and type
        /// </summary>
        /// <param name="options"></param>
        public void Load(object options)
        {
            PropertyInfo[] pInfos = this.GetType().GetProperties();
            for (int i = 0; i < pInfos.Length; i++)
            {
                PropertyInfo anonProp = options.GetType().GetProperty(pInfos[i].Name);
                if (anonProp != null)
                {
                    if (anonProp.PropertyType == pInfos[i].PropertyType)
                    {
                        pInfos[i].SetValue(this, anonProp.GetValue(options, null), null);
                    }
                    else if (pInfos[i].PropertyType.IsEnum && anonProp.PropertyType == typeof(string))
                    {
                        string strValue = anonProp.GetValue(options, null) as string;
                        if (!string.IsNullOrEmpty(strValue))
                        {
                            object parsedValue = Enum.Parse(pInfos[i].PropertyType, strValue);
                            pInfos[i].SetValue(this, parsedValue, null);
                        }
                    }
                }
            }
        }
    }
}
