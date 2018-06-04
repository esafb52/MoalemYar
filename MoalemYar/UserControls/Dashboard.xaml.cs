/****************************** ghost1372.github.io ******************************\
*	Module Name:	Dashboard.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 3, 03:23 ب.ظ
*
***********************************************************************************/

using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Brush BorderColor { get; set; }
        public static int _SchoolCount = 0;
        public static int _StudentCount = 0;
        public static int _UserCount = 0;
        public Dashboard()
        {
            InitializeComponent();

            DataContext = this;
            BorderColor = AppVariable.GetBrush(FindElement.Settings.ChartColor ?? AppVariable.CHART_GREEN);

            txtStCount.Text = _StudentCount.ToString();
            txtUCount.Text = _UserCount.ToString();
            txtScCount.Text = _SchoolCount.ToString();
            AchievementChart.Series.Add(new LineSeries
            {
                Values = new ChartValues<double>(new double[] { 10, 25, 65, 57, 15, 70 }),
                StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
            });
            AchievementChart.AxisX.Add(new Axis
            {
                Labels = new string[] { "Item", "Item", "Item", "Item", "Item", "Item" },
                Separator = new LiveCharts.Wpf.Separator { }
            });
            
        }

        public void getTopStudent(long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Scores.Join(
                  db.Students,
                  c => c.StudentId,
                  v => v.Id,
                  (c, v) => new DataClass.DataTransferObjects.StudentsScoresDto { Id = c.Id, BaseId = v.BaseId, StudentId = v.Id, Name = v.Name, LName = v.LName, FName = v.FName, Scores = c.Scores }
              ).OrderBy(x => x.Scores).Where(x => x.BaseId == BaseId).Take(7).ToList();

                var res = query.GroupBy(x => new { x.StudentId })
                          .Select(x => new
                          {
                              x.Key.StudentId,
                              Name = x.FirstOrDefault().Name,
                              LName = x.FirstOrDefault().LName,
                              FName = x.FirstOrDefault().FName,
                              Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                          }).OrderByDescending(x => x.Sum).ToArray();
                if (query.Any())
                {
                    try
                    {
                        txtSt1.Text = query[0].Name + " " + query[0].LName;
                        txtSt2.Text = query[1].Name + " " + query[1].LName;
                        txtSt3.Text = query[2].Name + " " + query[2].LName;
                        txtSt4.Text = query[3].Name + " " + query[3].LName;
                        txtSt5.Text = query[4].Name + " " + query[4].LName;
                        txtSt6.Text = query[5].Name + " " + query[5].LName;
                        prgSt1.Value = res[0].Sum;
                        prgSt2.Value = res[1].Sum;
                        prgSt3.Value = res[2].Sum;
                        prgSt4.Value = res[3].Sum;
                        prgSt5.Value = res[4].Sum;
                        prgSt6.Value = res[5].Sum;
                    }
                    catch (Exception)
                    {

                    }
                }
                
            }
        }

        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getTopStudent(Convert.ToInt64(cmbEditBase.SelectedValue));
        }
        private void getSchool()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.Select(x => x);
                    if (query.Any())
                    {
                        cmbEditBase.ItemsSource = query.ToList();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            getSchool();
            cmbEditBase.SelectedIndex = FindElement.Settings.DefaultSchool ?? -1;
        }

        private void btnCheckUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            MainWindow.main.tab.SelectedIndex = 4;
        }
    }
}