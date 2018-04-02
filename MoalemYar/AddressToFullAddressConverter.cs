
/****************************** ghost1372.github.io ******************************\
*	Module Name:	AddressToFullAddressConverter.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 2, 11:52 ق.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using System.IO;

namespace ListBoxSelectionColorChange
{
    [ValueConversion(typeof(Address), typeof(string))]
    public class AddressToFullAddressConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
        {

            if (value == null)
              return null;

            Address address=value as Address;

            if (address != null)
            {
               return string.Format("{0}, {1}, {2}",address.AddressLine1,address.Postcode,address.Country);
            }
            return null;

        }

        public object ConvertBack(object value, Type targetType, object parameter,
      System.Globalization.CultureInfo culture)
        {

            throw new Exception("The method or operation is not implemented.");

        }



    }
}
