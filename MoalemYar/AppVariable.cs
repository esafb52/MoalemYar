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
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MoalemYar
{
    public class AppVariable
    {
        private static SettingsBag Settings { get; } = JsonSettings.Construct<SettingsBag>(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MoalemYar\config.json").LoadNow().EnableAutosave();
        public static string LogifyOfflinePath = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MoalemYar\";
        public static string LogifyAPIKey = "SPECIFY_YOUR_API_KEY_HERE"; // http://logify.devexpress.com/
        public static string UpdateServer = "https://raw.githubusercontent.com/ghost1372/MoalemYar/master/Updater.xml";
        public const string UpdateXmlTag = "MoalemYar"; //Defined in Xml file
        public const string UpdateXmlChildTag = "AppVersion"; //Defined in Xml file
        public const string UpdateVersionTag = "version"; //Defined in Xml file
        public const string UpdateUrlTag = "url"; //Defined in Xml file
        public const string UpdateChangeLogTag = "changelog";

        public static string getAppName = Assembly.GetExecutingAssembly().GetName().Name;
        public static string getAppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string getAppNameAndVersion = Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static string getAppTitle = "معلم یار نسخه آزمایشی ";

        // COLOR \\
        public const string CYAN = "#00BCD4";

        public const string GREEN = "#4CAF50";
        public const string BGBLACK = "#333";
        public const string ORANGE = "#E0A030";
        public const string RED = "#F44336";
        public const string BLUE = "#1751C3";

        public static string TosifiSystem = "TosifiSystem";
        public static string CredentialLogin = "Credential";
        public static string Autorun = "Autorun";
        public static string HamburgerMenu = "HamburgerMenu";
        public static string AutoSendReport = "AutoSendReport";
        public static string SkinCode = "SkinCode";

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
                Settings[TosifiSystem] = true;
                Settings[CredentialLogin] = false;
                Settings[Autorun] = false;
                Settings[HamburgerMenu] = true;
                Settings[AutoSendReport] = true;
                Settings[SkinCode] = "#6D819A";


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