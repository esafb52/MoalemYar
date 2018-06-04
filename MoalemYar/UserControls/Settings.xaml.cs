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
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.SQLite;
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
        private ObservableCollection<string> list = new ObservableCollection<string>();

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
                    cmbBase.ItemsSource = query.ToList();
                }
            }
            catch (SQLiteException) { }
            catch (NullReferenceException)
            {
            }
        }

        #endregion Async Query

        private void LoadSettings()
        {
            loadServers();
            if (FindElement.Settings.CredentialLogin)
                swLogin.IsChecked = true;
            else
                swLogin.IsChecked = false;

            if (FindElement.Settings.Autorun)
                swAutoStart.IsChecked = true;
            else
                swAutoStart.IsChecked = false;

            if (FindElement.Settings.AutoSendReport)
                swAutoReport.IsChecked = true;
            else
                swAutoReport.IsChecked = false;

            var hb_Menu = FindElement.Settings.HamburgerMenu ?? true;
            Hamborger_Menu.IsChecked = hb_Menu;

            color1.Background = AppVariable.GetBrush(FindElement.Settings.SkinCode ?? AppVariable.DEFAULT_BORDER_BRUSH);
            colorChart.Background = AppVariable.GetBrush(FindElement.Settings.ChartColor ?? AppVariable.CHART_PURPLE);
        }

        private void color1_close()
        {
            FindElement.Settings.SkinCode = color1.CurrentColor.OpaqueSolidColorBrush.ToString();
        }

        private void colorChart_close()
        {
            FindElement.Settings.ChartColor = colorChart.CurrentColor.OpaqueSolidColorBrush.ToString();
            FindElement.Settings.ChartColorIndex = -1;
        }
        private void swLogin_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings.CredentialLogin = Convert.ToBoolean(swLogin.IsChecked);
        }

        private void swAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings.Autorun = Convert.ToBoolean(swAutoStart.IsChecked);
            AppVariable.RegisterInStartup(Convert.ToBoolean(swAutoStart.IsChecked));
        }

        private void Hamborger_Menu_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings.HamburgerMenu = Hamborger_Menu.IsChecked;
            MainWindow.main.tab.IconMode = Convert.ToBoolean(!Hamborger_Menu.IsChecked);
        }

        private void swAutoReport_Checked(object sender, RoutedEventArgs e)
        {
            FindElement.Settings.AutoSendReport = Convert.ToBoolean(swAutoReport.IsChecked);
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
                cmbBase.ItemsSource = null;
                File.Delete(folder);
                MainWindow.main.exAddOrUpdateSchool.Hint = "0";
                MainWindow.main.exAddOrUpdateStudent.Hint = "0";
                MainWindow.main.exAddOrUpdateUser.Hint = "0";
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
            FindElement.Settings.ChartType = element.SelectedIndex;
        }

        private void cmbChartColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = sender as ComboBox;
            FindElement.Settings.ChartColorIndex = element.SelectedIndex;
            switch (element.SelectedIndex)
            {
                case 0:
                    FindElement.Settings.ChartColor = AppVariable.CHART_GREEN;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_GREEN);
                    break;

                case 1:
                    FindElement.Settings.ChartColor = AppVariable.CHART_PURPLE;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_PURPLE);

                    break;

                case 2:
                    FindElement.Settings.ChartColor = AppVariable.CHART_ORANGE;
                    colorChart.Background = AppVariable.GetBrush(AppVariable.CHART_ORANGE);

                    break;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var elementType = FindElement.FindElementByName<ComboBox>(cmbChartType, "cmbChart");
            var elementColor = FindElement.FindElementByName<ComboBox>(cmbChartColor, "cmbChartColor");
            elementType.SelectedIndex = FindElement.Settings.ChartType ?? -1;
            elementColor.SelectedIndex = FindElement.Settings.ChartColorIndex ?? -1;
        }

        private void cmbBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FindElement.Settings.DefaultSchool = cmbBase.SelectedIndex;
        }

        private void cmbServer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] url = {
        "http://elam.medu.ir/regulation/archive?ocode=90340101",
        "http://chardavol.ilam.medu.ir/regulation/archive?ocode=90341907",
        "http://1805.ea.medu.ir/regulation/archive?ocode=90180501",
        "http://1869.ea.medu.ir/regulation/archive?ocode=90186902",
        "http://1891.ea.medu.ir/regulation/archive?ocode=90189104",
        "http://1836.ea.medu.ir/regulation/archive?ocode=90183600",
        "http://1823.ea.medu.ir/regulation/archive?ocode=90182302",
        "http://1889.ea.medu.ir/regulation/archive?ocode=90188906",
        "http://1901.arta.medu.ir/regulation/archive?ocode=90190104",
        "http://1902.arta.medu.ir/regulation/archive?ocode=1000000106",
        "http://1917.arta.medu.ir/regulation/archive?ocode=1000000108",
        "http://1945.arta.medu.ir/regulation/archive?ocode=1000000112",
        "http://1942.arta.medu.ir/regulation/archive?ocode=90194207",
        "http://1935.arta.medu.ir/regulation/archive?ocode=90193501",
        "http://1932.arta.medu.ir/regulation/archive?ocode=1000000111",
        "http://1949.arta.medu.ir/regulation/archive?ocode=1000000114",
        "http://1947.arta.medu.ir/regulation/archive?ocode=1000000113",
        "http://1926.arta.medu.ir/regulation/archive?ocode=1000000109",
        "http://1914.arta.medu.ir/regulation/archive?ocode=1000000107",
        "http://karaj3.alborz.medu.ir/regulation/archive?ocode=1000000781",
        "http://nahiye2.hormozgan.medu.ir/regulation/archive?ocode=1000000333",
        "http://jenah.hormozgan.medu.ir/regulation/archive?ocode=1000000358",
        "http://5743.zanjan.medu.ir/regulation/archive?ocode=100038170",
        "http://5723.zanjan.medu.ir/regulation/archive?ocode=100038161",
        "http://5701.zanjan.medu.ir/regulation/archive?ocode=100038159",
        "http://5702.zanjan.medu.ir/regulation/archive?ocode=100038172",
        "http://5711.zanjan.medu.ir/regulation/archive?ocode=100038160",
        "http://5725.zanjan.medu.ir/regulation/archive?ocode=100038168",
        "http://5745.zanjan.medu.ir/regulation/archive?ocode=100038171",
        "http://5734.zanjan.medu.ir/regulation/archive?ocode=100038164",
        "http://5712.zanjan.medu.ir/regulation/archive?ocode=100038163",
        "http://5731.zanjan.medu.ir/regulation/archive?ocode=100038169",
        "http://5747.zanjan.medu.ir/regulation/archive?ocode=100038167",
        "http://5714.zanjan.medu.ir/regulation/archive?ocode=100038166",
        "http://5705.zanjan.medu.ir/regulation/archive?ocode=100038162",
        "http://5708.zanjan.medu.ir/regulation/archive?ocode=100038165",
        "http://fars.medu.ir/regulation/archive?ocode=90230005",
        "http://rfs.kerman.medu.ir/regulation/archive?ocode=1000000538",
        "http://orz.kerman.medu.ir/regulation/archive?ocode=90384104",
        "http://bisotun.kermanshah.medu.ir/regulation/archive?ocode=1000000373",
        "http://kh1.lorestan.medu.ir/regulation/archive?ocode=80131603",
        "http://kh2.lorestan.medu.ir/regulation/archive?ocode=80227001",
        "http://est.lorestan.medu.ir/regulation/archive?ocode=1000000340",
        "http://dur.lorestan.medu.ir/regulation/archive?ocode=80418406",
        "http://mazand.medu.ir/regulation/archive?ocode=1000000282"
};
            FindElement.Settings.DefaultServer = url[cmbServer.SelectedIndex].ToString();
        }

        private void loadServers()
        {
            string[] city = new string[]{
        "ایلام← ایلام",
        "ایلام← چرداول",
        "آذربایجان شرقی← ناحیه5 تبریز",
        "آذربایجان شرقی← تسوج",
        "آذربایجان شرقی← تیکمه داش",
        "آذربایجان شرقی← خاروانا",
        "آذربایجان شرقی← خواجه",
        "آذربایجان شرقی← عشایر در کلیبر",
        "اردبیل← ناحیه1",
        "اردبیل← ناحیه2",
        "اردبیل← خلخال",
        "اردبیل← ارشق",
        "اردبیل← انگوت",
        "اردبیل← بیله سوار",
        "اردبیل← پارس آباد",
        "اردبیل← سرعین",
        "اردبیل← جعفرآباد",
        "اردبیل← کوثر",
        "اردبیل ← نمین",
        "البرز← ناحیه3",
        "هرمزگان← ناحیه2 بندرعباس",
        "هرمزگان← جناح",
        "زنجان← بزینه رود",
        "زنجان← خدابنده",
        "زنجان← ناحیه 1 زنجان",
        "زنجان← ناحیه 2 زنجان",
        "زنجان← ابهر",
        "زنجان← افشار",
        "زنجان← انگوران",
        "زنجان← ایجرود",
        "زنجان← خرمدره",
        "زنجان← زنجان رود",
        "زنجان← سجاس رود",
        "زنجان← سلطانیه",
        "زنجان← طارم",
        "زنجان← ماهنشان",
        "فارس← کلیه مناطق فارس",
        "کرمان← رفسنجان",
        "کرمان← ارزونیه",
        "کرمانشاه← بیستون",
        "لرستان← ناحیه1 خرم آباد",
        "لرستان← ناحیه2 خرم آباد",
        "لرستان← استثنایی",
        "لرستان← دورود",
        "مازندران← کلیه مناطق"
 };

            for (int i = 0; i < city.Length; i++)
            {
                list.Add(city[i].ToString());
            }
            cmbServer.ItemsSource = city;
        }
    }
}