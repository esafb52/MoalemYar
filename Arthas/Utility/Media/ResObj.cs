/****************************** ghost1372.github.io ******************************\
*	Module Name:	ResObj.cs
*	Project:		Arthas
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:54 ب.ظ
*
***********************************************************************************/

using System.IO;
using System.Reflection;
using System.Windows.Media;

namespace Arthas.Utility.Media
{
    public class ResObj
    {
        private static Stream Get(Assembly assembly, string path)
        {
            return assembly.GetManifestResourceStream(assembly.GetName().Name + "." + path);
        }

        public static string GetString(Assembly assembly, string path)
        {
            try
            {
                return StreamObj.ToString(Get(assembly, path));
            }
            catch
            {
                return null;
            }
        }

        public static ImageSource GetImageSource(Assembly assembly, string path)
        {
            try
            {
                return StreamObj.ToImageSource(Get(assembly, path));
            }
            catch
            {
                return null;
            }
        }
    }
}