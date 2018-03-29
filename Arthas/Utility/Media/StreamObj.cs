/****************************** ghost1372.github.io ******************************\
*	Module Name:	StreamObj.cs
*	Project:		Arthas
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:54 ب.ظ
*
***********************************************************************************/

using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Arthas.Utility.Media
{
    public class StreamObj
    {
        public static string ToString(Stream stream)
        {
            try { return new StreamReader(stream).ReadToEnd(); }
            catch { return ""; }
        }

        public static ImageSource ToImageSource(Stream stream)
        {
            try
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();

                return bitmapImage;
            }
            catch { return null; }
        }
    }
}