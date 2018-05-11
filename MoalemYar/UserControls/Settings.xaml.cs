/****************************** ghost1372.github.io ******************************\
*	Module Name:	Settings.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 28, 05:42 ب.ظ
*
***********************************************************************************/

using nucs.JsonSettings;
using nucs.JsonSettings.Fluent;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        internal static Settings main;
        private SettingsBag Setting { get; } = JsonSettings.Construct<SettingsBag>(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MoalemYar\config.json").EnableAutosave().LoadNow();

        public Settings()
        {
            InitializeComponent();

            color1.ColorChange += delegate
            {
                MainWindow.main.BorderBrush = color1.CurrentColor.OpaqueSolidColorBrush;
            };
            main = this;
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (Convert.ToBoolean(Setting[AppVariable.CredentialLogin] ?? false))
                swLogin.IsChecked = true;
            else
                swLogin.IsChecked = false;

            if (Convert.ToBoolean(Setting[AppVariable.Autorun] ?? false))
                swAutoStart.IsChecked = true;
            else
                swAutoStart.IsChecked = false;

            if (Convert.ToBoolean(Setting[AppVariable.AutoSendReport] ?? false))
                swAutoReport.IsChecked = true;
            else
                swAutoReport.IsChecked = false;

            var hb_Menu = Convert.ToBoolean(Setting[AppVariable.HamburgerMenu] ?? false);
            Hamborger_Menu.IsChecked = hb_Menu;

            var color = (Color)ColorConverter.ConvertFromString(Convert.ToString(Setting[AppVariable.SkinCode] ?? AppVariable.DEFAULT_BORDER_BRUSH));
            var brush = new SolidColorBrush(color);
            color1.Background = brush;
        }

        private void color1_close()
        {
            Setting[AppVariable.SkinCode] = color1.CurrentColor.OpaqueSolidColorBrush.ToString();
            MainWindow.main.RestartNotification();
        }

        private void swLogin_Checked(object sender, RoutedEventArgs e)
        {
            Setting[AppVariable.CredentialLogin] = swLogin.IsChecked;
        }

        private void swAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            Setting[AppVariable.Autorun] = swAutoStart.IsChecked;
            AppVariable.RegisterInStartup(Convert.ToBoolean(swAutoStart.IsChecked));
        }

        private void Hamborger_Menu_Checked(object sender, RoutedEventArgs e)
        {
            Setting[AppVariable.HamburgerMenu] = Hamborger_Menu.IsChecked;
            MainWindow.main.tab.IconMode = Convert.ToBoolean(!Hamborger_Menu.IsChecked);
        }

        private void swAutoReport_Checked(object sender, RoutedEventArgs e)
        {
            Setting[AppVariable.AutoSendReport] = swAutoReport.IsChecked;
            MainWindow.main.LogifyCrashReport();
        }

        private void btnFactoryReset_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ResetDataConfirmNotification("تنظیمات برنامه");
        }

        public void resetConfig()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MoalemYar\config.json";
            if (System.IO.File.Exists(folder))
            {
                File.Delete(folder);
                MainWindow.main.DataResetDeletedNotification("تنظیمات برنامه");
            }
        }

        public void resetDatabase()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\MoalemYar\Database\data.db";
            if (System.IO.File.Exists(folder))
            {
                File.Delete(folder);
                MainWindow.main.DataResetDeletedNotification("دیتابیس برنامه");
            }
        }

        private void btnDatabaseReset_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ResetDataConfirmNotification("دیتابیس برنامه");
        }
    }
}