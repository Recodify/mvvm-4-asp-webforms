using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows.Data;
using DomainModel.BusinessObjects;

namespace SimpleBinding.Converters
{
    public class AddressConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Address address = value as Address;
            if (address == null)
                return value;

            return string.Format("{0}, {1}, {2}", new object[] { address.HouseNameNumber, address.Street, address.Postcode });
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //Means this can only be used on one-way binds.
            throw new NotImplementedException();
        }

        #endregion
    }
}