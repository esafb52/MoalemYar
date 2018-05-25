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
using System.Data.Entity;
using System.IO;
using System.Linq;
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
        private SettingsBag Setting { get; } = JsonSettings.Construct<SettingsBag>(AppVariable.fileName + @"\config.json").EnableAutosave().LoadNow();

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

            var hb_Menu = Convert.ToBoolean(Setting[AppVariable.HamburgerMenu] ?? true);
            Hamborger_Menu.IsChecked = hb_Menu;

            color1.Background = AppVariable.GetBrush(Convert.ToString(Setting[AppVariable.SkinCode] ?? AppVariable.DEFAULT_BORDER_BRUSH));
            colorChart.Background = AppVariable.GetBrush(Convert.ToString(Setting[AppVariable.ChartColor] ?? AppVariable.CHART_PURPLE));
        }
        
        private void color1_close()
        {
            Setting[AppVariable.SkinCode] = color1.CurrentColor.OpaqueSolidColorBrush.ToString();
        }
        private void colorChart_close()
        {
            Setting[AppVariable.ChartColor] = colorChart.CurrentColor.OpaqueSolidColorBrush.ToString();
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
            string folder = AppVariable.fileName + @"\config.json";
            if (System.IO.File.Exists(folder))
            {
                File.Delete(folder);
                MainWindow.main.DataResetDeletedNotification("تنظیمات برنامه");
            }
        }

        public void resetDatabase()
        {
            string folder = AppVariable.fileName + @"\data.db";
            if (System.IO.File.Exists(folder))
            {
                File.Delete(folder);
                MainWindow.main.DataResetDeletedNotification("دیتابیس برنامه");
                using (var context = new DataClass.myDbContext())
                {
                    CreateAndSeedDatabase(context);
                }
            }
        }

        private static void CreateAndSeedDatabase(DbContext context)
        {
            context.Database.Initialize(true);
            context.Set<DataClass.Tables.User>().Add(new DataClass.Tables.User
            {
                Username = "test",
                Password = "test"
            });

            context.SaveChanges();

        }

        private void btnDatabaseReset_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ResetDataConfirmNotification("دیتابیس برنامه");
        }

        private void cmbChart_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = sender as ComboBox;
            switch (element.SelectedIndex)
            {
                case 0:
                    Setting[AppVariable.ChartType] = AppVariable.CHART_Column;
                    break;
                case 1:
                    Setting[AppVariable.ChartType] = AppVariable.CHART_Column2;
                    break;
                case 2:
                    Setting[AppVariable.ChartType] = AppVariable.CHART_Line;
                    break;
                case 3:
                    Setting[AppVariable.ChartType] = AppVariable.CHART_Line2;
                    break;
                case 4:
                    Setting[AppVariable.ChartType] = AppVariable.CHART_Area;
                    break;
            }

        }

        private void cmbChartColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = sender as ComboBox;
            switch (element.SelectedIndex)
            {
                case 0:
                    Setting[AppVariable.ChartColor] = AppVariable.CHART_GREEN;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_GREEN);
                    break;

                case 1:
                    Setting[AppVariable.ChartColor] = AppVariable.CHART_PURPLE;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_PURPLE);

                    break;
                case 2:
                    Setting[AppVariable.ChartColor] = AppVariable.CHART_ORANGE;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_ORANGE);

                    break;
            }
        }
    }
}