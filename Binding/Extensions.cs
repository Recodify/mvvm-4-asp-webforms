using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Binding
{

    public static class EnumExtensions
    {
        public static bool TryParse(this Enum theEnum, string value)
        {
            foreach (string item in Enum.GetNames(theEnum.GetType()))
            {
                if (item.Equals(value))
                {
                    theEnum = (Enum)Enum.Parse(theEnum.GetType(), value.ToString());
                    return true;
                }
            }

            if (Enum.IsDefined(theEnum.GetType(), Convert.ChangeType(value, (Enum.GetUnderlyingType(theEnum.GetType())))))
            {
                theEnum = (Enum)Enum.Parse(theEnum.GetType(), value.ToString());
                return true;
            }
            return false;
        }
    }

    public static class EnumberableExtensions
    {
        public static object GetByIndex(this IEnumerable enumerable, int i)
        {
            int m = 0;
            foreach (object obj in enumerable)
            {
                if (m == i)
                    return obj;
                m++;
            }
            return null;
        }
    }
}
