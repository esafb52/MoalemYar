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
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class InitialView : UserControl
    {
        private bool runFirst = false;

        public InitialView()
        {
            InitializeComponent();

            DataContext = this;

            //Todo: Fix
            //txtStCount.Text = MainWindow.main.exAddOrUpdateStudent.Hint;
            //txtUCount.Text = MainWindow.main.exAddOrUpdateUser.Hint;
            //txtScCount.Text = MainWindow.main.exAddOrUpdateSchool.Hint;
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
              ).OrderBy(x => x.Scores).Where(x => x.BaseId == BaseId).ToList();

                var res = query.GroupBy(x => new { x.StudentId })
                          .Select(x => new
                          {
                              x.Key.StudentId,
                              Name = x.FirstOrDefault().Name,
                              LName = x.FirstOrDefault().LName,
                              FName = x.FirstOrDefault().FName,
                              Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                          }).OrderByDescending(x => x.Sum).Take(6).ToArray();

                foreach (var item in res)
                {
                    ProgressBar progressBar;
                    TextBlock textBlock;
                    Control _currentUser;
                    progressBar = new ProgressBar()
                    {
                        FlowDirection = FlowDirection.LeftToRight,
                        Value = item.Sum
                    };
                    textBlock = new TextBlock()
                    {
                        Opacity = .4,
                        Margin = new Thickness(0, 5, 0, 0),
                        FontSize = 15,
                        Text = item.Name + " " + item.LName
                    };

                    _currentUser = progressBar;
                    stkDash.Children.Add(textBlock);

                    stkDash.Children.Add(_currentUser);
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
            if (!runFirst)
            {
                getSchool();
                cmbEditBase.SelectedIndex = Convert.ToInt32(FindElement.Settings.DefaultSchool);
                runFirst = true;
            }
            using (var db = new DataClass.myDbContext())
            {
                long baseId = Convert.ToInt64(cmbEditBase.SelectedValue);
                var query = db.Scores.Join(
                   db.Students,
                   c => c.StudentId,
                   v => v.Id,
                   (c, v) => new DataClass.DataTransferObjects.StudentsScoresDto { Id = c.Id, BaseId = v.BaseId, StudentId = v.Id, Name = v.Name, LName = v.LName, FName = v.FName, Scores = c.Scores }
               ).Where(y => y.BaseId == baseId).ToList();
                var x = query.Where(y => y.Scores == "نیاز به تلاش بیشتر").ToList();
                var xx = query.Where(y => y.Scores == "قابل قبول").ToList();
                var xxx = query.Where(y => y.Scores == "خوب").ToList();
                var xxxx = query.Where(y => y.Scores == "خیلی خوب").ToList();
                AchievementChart.Series.Add(new LineSeries
                {
                    Values = new ChartValues<double>(new double[] { xxxx.Count, xxx.Count, xx.Count, x.Count }),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
                AchievementChart.AxisX.Add(new Axis
                {
                    Labels = new string[] { "خیلی خوب", "خوب", "قابل قبول", "نیاز به تلاش بیشتر" },
                    Separator = new LiveCharts.Wpf.Separator { }
                });
            }
        }
    }
}