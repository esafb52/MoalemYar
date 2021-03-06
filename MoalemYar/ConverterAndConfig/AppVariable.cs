﻿/****************************** ghost1372.github.io ******************************\
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
using System.Data.SQLite;
using System.Diagnostics;
using System.Reflection;

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
        public static string getAppTitle = "معلم یار ";

        public static string fileName = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Assembly.GetExecutingAssembly().GetName().Name;
        public static string fileNameBakhsh = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\" + Assembly.GetExecutingAssembly().GetName().Name + @"\Circular\";
        public static string DefaultServer2 = "http://5743.zanjan.medu.ir";

        #endregion App Details

        #region Notification KEY

        public static string Delete_Confirm_KEY = "DeleteConfirm";
        public static string Reset_Data_Confirm_KEY = "ResetDataConfirm";
        public static string Data_Reset_Deleted_KEY = "DataResetDeleted";
        public static string Same_Password_KEY = "SamePassword";
        public static string Delete_Exist_KEY = "DeleteExist";
        public static string Azmon_KEY = "Azmon";
        public static string Fill_All_Data_KEY = "FillAllData";
        public static string No_Data_KEY = "NoData";
        public static string Backup_KEY = "Backup";
        public static string Recived_Circular_KEY = "RecivedCircular";
        public static string Update_Data_KEY = "UpdateData";
        public static string Deleted_KEY = "Deleted";
        public static string Add_Data_KEY = "AddData";
        public static string Update_KEY = "Update";

        #endregion Notification KEY

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

        #region Backup Database

        public static void takeBackup()
        {
            try
            {
                var saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog1.Filter = "DataBase Backup|*.db";
                saveFileDialog1.Title = "Save an Backup File";
                saveFileDialog1.FileName = "data" + DateTime.Now.ToShortDateString().Replace("/", "-");
                if (saveFileDialog1.ShowDialog() != System.Windows.Forms.DialogResult.Cancel && saveFileDialog1.FileName != "")
                {
                    using (var source = new SQLiteConnection(@"Data Source=|DataDirectory|\data.db; Version=3;"))
                    using (var destination = new SQLiteConnection("Data Source=" + saveFileDialog1.FileName + "; Version=3;"))
                    {
                        source.Open();
                        destination.Open();
                        source.BackupDatabase(destination, "main", "main", -1, null, 0);
                    }
                    MainWindow.main.showGrowlNotification(Backup_KEY, true, "پشتیبان گیری از اطلاعات ");
                }
            }
            catch (Exception)
            {
                MainWindow.main.showGrowlNotification(Backup_KEY, false, "پشتیبان گیری از اطلاعات ");
            }
        }

        public static void dbRestore()
        {
            try
            {
                var openFileDialog1 = new System.Windows.Forms.OpenFileDialog();

                openFileDialog1.Filter = "Backup files (*.db)|*.db";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.RestoreDirectory = true;

                if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    System.IO.File.Copy(openFileDialog1.FileName, AppVariable.fileName + @"\data.db", true);
                    MainWindow.main.showGrowlNotification(Backup_KEY, true, "بازگردانی اطلاعات ");
                }
            }
            catch (Exception)
            {
                MainWindow.main.showGrowlNotification(Backup_KEY, false, "بازگردانی اطلاعات ");
            }
        }

        #endregion Backup Database
    }
}