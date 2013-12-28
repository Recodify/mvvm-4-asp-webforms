using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Data;

namespace SimpleBinding.Converters
{
    public class LeaveTypeImageConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Format("Leave type: {0}", value.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}