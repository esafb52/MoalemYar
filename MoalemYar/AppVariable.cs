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
        public static string getAppNameAndVersion = getAppName + " " + getAppVersion;
        public static string getAppTitle = "معلم یار نسخه آزمایشی ";

        public static string fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Assembly.GetExecutingAssembly().GetName().Name;
        public static string fileNameBakhsh = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Assembly.GetExecutingAssembly().GetName().Name + @"\Circular\";
        public static string DefaultServer2 = "http://5743.zanjan.medu.ir/regulation/archive?ocode=100038170";

        #endregion App Details

        #region Chart

        public const string CHART_PURPLE = "#CE2156";
        public const string CHART_ORANGE = "#EB5A13";
        public const string CHART_GREEN = "#7DBD8D";

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


        public static double NotificationAnimInDur = 0.75;
        public static double NotificationAnimOutDur = 0.5;
        public static int NotificationDelay = 2;

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
        public static string NumberToEnum(string value)
        {
            switch (value)
            {
                case "01":
                    return "فروردین";

                case "02":
                    return "اردیبهشت";

                case "03":
                    return "خرداد";

                case "04":
                    return "تیر";
                case "05":
                    return "مرداد";
                case "06":
                    return "شهریور";
                case "07":
                    return "مهر";
                case "08":
                    return "آبان";
                case "09":
                    return "آذر";
                case "10":
                    return "دی";
                case "11":
                    return "بهمن";
                case "12":
                    return "اسفند";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value not recognized");
            }
        }
    }
}