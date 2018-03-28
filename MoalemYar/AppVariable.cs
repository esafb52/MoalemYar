
/****************************** ghost1372.github.io ******************************\
*	Module Name:	AppVariable.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 28, 11:40 ق.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MoalemYar
{
    public class AppVariable
    {

        public static string LogifyAPIKey = "SPECIFY_YOUR_API_KEY_HERE"; // http://logify.devexpress.com/
        public static string getAppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static string getAppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string getAppNameAndVersion = Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string getAppTitle = "معلم یار نسخه آزمایشی ";

    }
}
