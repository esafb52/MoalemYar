/****************************** ghost1372.github.io ******************************\
*	Module Name:	DoubleFactor.cs
*	Project:		Arthas
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:54 ب.ظ
*
***********************************************************************************/

using System.Globalization;
using System.Windows.Data;

namespace Arthas.Controls.Converter
{
    public class DoubleFactor : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0.0;
            }
            else if (parameter == null)
            {
                return System.Convert.ToDouble(value);
            }
            else
            {
                return System.Convert.ToDouble(value) * System.Convert.ToDouble(parameter);
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0.0;
            }
            else if (parameter == null)
            {
                return System.Convert.ToDouble(value);
            }
            else
            {
                return System.Convert.ToDouble(value) / System.Convert.ToDouble(parameter);
            }
        }
    }
}