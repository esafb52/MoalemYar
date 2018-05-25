/****************************** ghost1372.github.io ******************************\
*	Module Name:	AppVariable.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 28, 11:40 ق.ظ
*
***********************************************************************************/

using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media;

namespace MoalemYar
{
    public class AppVariable
    {
        #region Crash Report

        public static string LogifyOfflinePath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Assembly.GetExecutingAssembly().GetName().Name + @"\";
        public static string LogifyAPIKey = "SPECIFY_YOUR_API_KEY_HERE"; // http://logify.devexpress.com/   //Todo: Add API

        #endregion Crash Report

        #region Update Configuration

        public static string UpdateServer = "https://raw.githubusercontent.com/ghost1372/MoalemYar/master/Updater.xml";
        public const string UpdateXmlTag = "MoalemYar"; //Defined in Xml file
        public const string UpdateXmlChildTag = "AppVersion"; //Defined in Xml file
        public const string UpdateVersionTag = "version"; //Defined in Xml file
        public const string UpdateUrlTag = "url"; //Defined in Xml file
        public const string UpdateChangeLogTag = "changelog";

        #endregion Update Configuration

        #region App Details

        public static string getAppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static string getAppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string getAppNameAndVersion = Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string getAppTitle = "معلم یار نسخه آزمایشی ";

        public static string fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Assembly.GetExecutingAssembly().GetName().Name;

        #endregion App Details

        #region Chart

        public const string CHART_PURPLE = "#CE2156";
        public const string CHART_ORANGE = "#EB5A13";
        public const string CHART_GREEN = "#7DBD8D";

        public const string CHART_Line = "Line";
        public const string CHART_Line2 = "StackedLine";
        public const string CHART_Column = "Column";
        public const string CHART_Column2 = "StackedColumn";
        public const string CHART_Area = "Area";

        #endregion Chart

        #region Colors

        public const string CYAN = "#00BCD4";
        public const string GREEN = "#4CAF50";
        public const string BGBLACK = "#333";
        public const string ORANGE = "#E0A030";
        public const string RED = "#F44336";
        public const string BLUE = "#1751C3";
        public const string DEFAULT_BORDER_BRUSH = "#6D819A";

        #endregion Colors

        #region Config Key

        public static string CredentialLogin = "Credential";
        public static string Autorun = "Autorun";
        public static string HamburgerMenu = "HamburgerMenu";
        public static string AutoSendReport = "AutoSendReport";
        public static string SkinCode = "SkinCode";
        public static string VersionCode = "Version";
        public static string ChartType = "ChartType";
        public static string ChartColor = "ChartColor";

        #endregion Config Key

        public static System.Windows.Media.Brush GetBrush(string ColorString)
        {
            var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(ColorString);
            var brush = new SolidColorBrush(color);
            return brush;
        }

        public static string RunActionMeasurePerformance(Action action)
        {
            GC.Collect();
            long initMemUsage = Process.GetCurrentProcess().WorkingSet64;

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            action();

            stopwatch.Stop();

            var currentMemUsage = Process.GetCurrentProcess().WorkingSet64;
            var memUsage = currentMemUsage - initMemUsage;
            if (memUsage < 0) memUsage = 0;

            return string.Format(" Elapsed time: {0}, Memory Usage: {1:N2} KB", stopwatch.Elapsed, memUsage / 1024);
        }

        public static void RegisterInStartup(bool isChecked)
        {
            var productName = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductName;
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
                    ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isChecked)
            {
                registryKey.SetValue(productName, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                registryKey.DeleteValue(productName);
            }
        }

        //Convert string to int in linq
        public static int EnumToNumber(string value)
        {
            switch (value)
            {
                case "خیلی خوب":
                    return 4;

                case "خوب":
                    return 3;

                case "قابل قبول":
                    return 2;

                case "نیاز به تلاش بیشتر":
                    return 1;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value not recognized");
            }
        }
    }
}