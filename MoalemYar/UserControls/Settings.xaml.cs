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
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        internal static Settings main;
        public Settings()
        {
            InitializeComponent();

            getSchool();
            color1.ColorChange += delegate
            {
                MainWindow.main.BorderBrush = color1.CurrentColor.OpaqueSolidColorBrush;
            };
            main = this;
            LoadSettings();
        }

        #region Async Query

        private void getSchool()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.Select(x => x);
                    if (query.Any())
                    {
                        cmbBase.ItemsSource = query.ToList();
                    }
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        #endregion Async Query

        private void LoadSettings()
        {
            if (Convert.ToBoolean(FindElement.Settings[AppVariable.CredentialLogin] ?? false))
                swLogin.IsChecked = true;
            else
                swLogin.IsChecked = false;

            if (Convert.ToBoolean(FindElement.Settings[AppVariable.Autorun] ?? false))
                swAutoStart.IsChecked = true;
            else
                swAutoStart.IsChecked = false;

            if (Convert.ToBoolean(FindElement.Settings[AppVariable.AutoSendReport] ?? false))
                swAutoReport.IsChecked = true;
            else
                swAutoReport.IsChecked = false;

            var hb_Menu = Convert.ToBoolean(FindElement.Settings[AppVariable.HamburgerMenu] ?? true);
            Hamborger_Menu.IsChecked = hb_Menu;

            color1.Background = AppVariable.GetBrush(Convert.ToString(FindElement.Settings[AppVariable.SkinCode] ?? AppVariable.DEFAULT_BORDER_BRUSH));
            colorChart.Background = AppVariable.GetBrush(Convert.ToString(FindElement.Settings[AppVariable.ChartColor] ?? AppVariable.CHART_PURPLE));
        }

        private void color1_close()
        {
            FindElement.Settings[AppVariable.SkinCode] = color1.CurrentColor.OpaqueSolidColorBrush.ToString();
        }

        private void colorChart_close()
        {
            FindElement.Settings[AppVariable.ChartColor] = colorChart.CurrentColor.OpaqueSolidColorBrush.ToString();
            FindElement.Settings[AppVariable.ChartColorIndex] = -1;
        }

        private void swLogin_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings[AppVariable.CredentialLogin] = swLogin.IsChecked;
        }

        private void swAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings[AppVariable.Autorun] = swAutoStart.IsChecked;
            AppVariable.RegisterInStartup(Convert.ToBoolean(swAutoStart.IsChecked));
        }

        private void Hamborger_Menu_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings[AppVariable.HamburgerMenu] = Hamborger_Menu.IsChecked;
            MainWindow.main.tab.IconMode = Convert.ToBoolean(!Hamborger_Menu.IsChecked);
        }

        private void swAutoReport_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings[AppVariable.AutoSendReport] = swAutoReport.IsChecked;
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
            if (context.Set<DataClass.Tables.User>().Count() != 0)
            {
                return;
            }
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
            FindElement.Settings[AppVariable.ChartType] = element.SelectedIndex;
        }

        private void cmbChartColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = sender as ComboBox;
            FindElement.Settings[AppVariable.ChartColorIndex] = element.SelectedIndex;
            switch (element.SelectedIndex)
            {
                case 0:
                    FindElement.Settings[AppVariable.ChartColor] = AppVariable.CHART_GREEN;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_GREEN);
                    break;

                case 1:
                    FindElement.Settings[AppVariable.ChartColor] = AppVariable.CHART_PURPLE;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_PURPLE);

                    break;

                case 2:
                    FindElement.Settings[AppVariable.ChartColor] = AppVariable.CHART_ORANGE;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_ORANGE);

                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var elementType = FindElement.FindElementByName<ComboBox>(cmbChartType, "cmbChart");
            var elementColor = FindElement.FindElementByName<ComboBox>(cmbChartColor, "cmbChartColor");
            elementType.SelectedIndex = Convert.ToInt32(FindElement.Settings[AppVariable.ChartType] ?? -1);
            elementColor.SelectedIndex = Convert.ToInt32(FindElement.Settings[AppVariable.ChartColorIndex] ?? -1);
        }

        private void cmbBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FindElement.Settings[AppVariable.DefaultSchool] = cmbBase.SelectedIndex;
        }
    }
}