using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Binding.Helpers
{
    public static class TypeHelper
    {

        public static bool IsSimpleType(Type type)
        {
            return IsSimpleType(Type.GetTypeCode(type));
        }

        /// <summary>
        /// Determines whether the type is 'simple' for the purposes of type conversion
        /// </summary>
        public static bool IsSimpleType(TypeCode type)
        {
            switch (type)
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.Char:
                case TypeCode.DateTime:
                case TypeCode.DBNull:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.String:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }
    }
}
