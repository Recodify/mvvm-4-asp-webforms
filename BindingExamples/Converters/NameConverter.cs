using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Data;
using DomainModel.ViewModels;

namespace SimpleBinding.Converters
{
    
    public class NameConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;

            return string.Format("Welcome {0}",value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return value;
            
            if (string.IsNullOrEmpty(value.ToString()))
                return value;

           string valueString = value.ToString();
           valueString = valueString.Replace("Welcome ", "");

           return valueString;
        }

        #endregion
    }
}