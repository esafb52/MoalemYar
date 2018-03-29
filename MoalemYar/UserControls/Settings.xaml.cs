
/****************************** ghost1372.github.io ******************************\
*	Module Name:	Settings.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 28, 05:42 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();

            color1.ColorChange += delegate
            {
                MainWindow.main.BorderBrush = color1.CurrentColor.OpaqueSolidColorBrush;
            };

            LoadSettings();
        }

         private void LoadSettings()
        {
            if (AppVariable.ReadSetting(AppVariable.CredentialLogin).Equals("True"))
                swLogin.IsChecked = true;
            else
                swLogin.IsChecked = false;

            if (AppVariable.ReadSetting(AppVariable.TosifiSystem).Equals("True"))
                swSystem.IsChecked = true;
            else
                swSystem.IsChecked = false;

            if (AppVariable.ReadSetting(AppVariable.Autorun).Equals("True"))
                swAutoStart.IsChecked = true;
            else
                swAutoStart.IsChecked = false;

            if (AppVariable.ReadSetting(AppVariable.AutoSendReport).Equals("True"))
                swAutoReport.IsChecked = true;
            else
                swAutoReport.IsChecked = false;

            var hb_Menu = Convert.ToBoolean(AppVariable.ReadSetting(AppVariable.HamburgerMenu));
            Hamborger_Menu.IsChecked = hb_Menu;

            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            color1.Background = brush;
        }

        private void color1_close()
        {
            AppVariable.AddUpdateAppSettings(AppVariable.SkinCode, color1.CurrentColor.OpaqueSolidColorBrush.ToString());
            MainWindow.main.RestartNotification();
        }

        private void swLogin_Checked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.CredentialLogin, true);
        }

        private void swLogin_Unchecked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.CredentialLogin, false);
        }

        private void swSystem_Checked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.TosifiSystem, true);

        }

        private void swSystem_Unchecked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.TosifiSystem, false);

        }

        private void swAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.Autorun, true);
            AppVariable.RegisterInStartup(true);
        }

        private void swAutoStart_Unchecked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.Autorun, false);
            AppVariable.RegisterInStartup(false);
        }

        private void Hamborger_Menu_Checked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.HamburgerMenu, true);
            MainWindow.main.tab.IconMode = false;
        }

        private void Hamborger_Menu_Unchecked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.HamburgerMenu, false);
            MainWindow.main.tab.IconMode = true;
        }

        private void swAutoReport_Checked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.AutoSendReport, true);
            MainWindow.main.LogifyCrashReport();
        }

        private void swAutoReport_Unchecked(object sender, RoutedEventArgs e)
        {
            AppVariable.AddUpdateAppSettings(AppVariable.AutoSendReport, false);
            MainWindow.main.LogifyCrashReport();
        }
    }
}
