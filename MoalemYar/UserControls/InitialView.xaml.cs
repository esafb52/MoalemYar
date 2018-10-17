/****************************** ghost1372.github.io ******************************\
*	Module Name:	Dashboard.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 3, 03:23 ب.ظ
*
***********************************************************************************/

using System;
using System.Collections.Generic;
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
        private List<DataClass.DataTransferObjects.myChartTemplate> list = new List<DataClass.DataTransferObjects.myChartTemplate>();

        public InitialView()
        {
            InitializeComponent();
            getInitialData();
        }

        public void getInitialData()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.ToList();
                    txtScCount.Text = query.Count().ToString();
                    var query2 = db.Users.ToList();
                    txtUCount.Text = query2.Count().ToString();
                    var query3 = db.Students.ToList();
                    txtStCount.Text = query3.Count().ToString();
                }
            }
            catch (Exception)
            {
            }
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
            getSchool();
            cmbEditBase.SelectedIndex = Convert.ToInt32(FindElement.Settings.DefaultSchool);
            using (var db = new DataClass.myDbContext())
            {
                long baseId = Convert.ToInt64(cmbEditBase.SelectedValue);
                var query = db.Scores.Join(
                   db.Students,
                   c => c.StudentId,
                   v => v.Id,
                   (c, v) => new DataClass.DataTransferObjects.StudentsScoresDto { Id = c.Id, BaseId = v.BaseId, StudentId = v.Id, Name = v.Name, LName = v.LName, FName = v.FName, Scores = c.Scores }
               ).Where(y => y.BaseId == baseId).ToList();

                var niazBeTalash = query.Where(y => y.Scores == "نیاز به تلاش بیشتر").ToList();
                var ghabelGhabol = query.Where(y => y.Scores == "قابل قبول").ToList();
                var khob = query.Where(y => y.Scores == "خوب").ToList();
                var kheyliKhob = query.Where(y => y.Scores == "خیلی خوب").ToList();

                list.Add(new DataClass.DataTransferObjects.myChartTemplate { Caption = "نیاز", Scores = niazBeTalash.Count });
                list.Add(new DataClass.DataTransferObjects.myChartTemplate { Caption = "قابل", Scores = ghabelGhabol.Count });
                list.Add(new DataClass.DataTransferObjects.myChartTemplate { Caption = "خوب", Scores = khob.Count });
                list.Add(new DataClass.DataTransferObjects.myChartTemplate { Caption = "خخ", Scores = kheyliKhob.Count });

                chart.ItemsSource = list;
            }
        }
    }
}