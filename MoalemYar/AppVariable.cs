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
using nucs.JsonSettings;
using nucs.JsonSettings.Fluent;
using System;
using System.Diagnostics;
using System.Reflection;

namespace MoalemYar
{
    public class AppVariable
    {
        private static SettingsBag Settings { get; } = JsonSettings.Construct<SettingsBag>(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MoalemYar\config.json").EnableAutosave().LoadNow();

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

        #endregion App Details

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

        #endregion Config Key

        #region "ReadWrite Settings"

        public static int ReadIntSetting(string key)
        {
            try
            {
                return Convert.ToInt32(Settings[key]);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static bool ReadBoolSetting(string key)
        {
            try
            {
                return Convert.ToBoolean(Settings[key]);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ReadSetting(string key)
        {
            try
            {
                return Settings[key].ToString();
            }
            catch (Exception)
            {
                return "Error Reading";
            }
        }

        public static void InitializeSettings()
        {
            try
            {
                Settings[CredentialLogin] = false;
                Settings[Autorun] = false;
                Settings[HamburgerMenu] = true;
                Settings[AutoSendReport] = true;
                Settings[SkinCode] = DEFAULT_BORDER_BRUSH;
                Settings[VersionCode] = getAppVersion;
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void AddUpdateAppSettings(string key, string value)
        {
            try
            {
                Settings[key] = value;
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void AddUpdateAppSettings(string key, int value)
        {
            try
            {
                Settings[key] = value;
            }
            catch (Exception)
            {
                return;
            }
        }

        public static void AddUpdateAppSettings(string key, bool value)
        {
            try
            {
                Settings[key] = value;
            }
            catch (Exception)
            {
                return;
            }
        }

        #endregion "ReadWrite Settings"

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
    }
}