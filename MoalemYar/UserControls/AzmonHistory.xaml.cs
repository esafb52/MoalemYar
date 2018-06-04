/****************************** ghost1372.github.io ******************************\
*	Module Name:	AzmonHistory.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 2, 07:58 ب.ظ
*
***********************************************************************************/

using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AzmonHistory.xaml
    /// </summary>
    public partial class AzmonHistory : UserControl
    {
        public Brush BorderColor { get; set; }

        public AzmonHistory()
        {
            InitializeComponent();
            DataContext = this;
            BorderColor = AppVariable.GetBrush(FindElement.Settings.ChartColor ?? AppVariable.CHART_GREEN);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var qschool = db.Schools.ToList();
                    cmbEditBase.ItemsSource = qschool.Any() ? qschool : null;

                    var qGroupName = db.Groups.ToList();
                    cmbGroups.ItemsSource = qGroupName.Any() ? qGroupName : null;
                }
            }
            catch (Exception)
            {

            }
           
            cmbEditBase.SelectedIndex = FindElement.Settings.DefaultSchool ?? -1;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cmbGroups_SelectionChanged(null, null);
        }

        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            using (var db = new DataClass.myDbContext())
            {
                long id = Convert.ToInt64(cmbEditBase.SelectedValue);
                var qUser = db.Students.Where(x => x.BaseId == id).ToList();
                dataGrid.ItemsSource = qUser.Any() ? qUser : null;
            }
        }

        private void cmbGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic getGroupName = cmbGroups.SelectedItem;
                string gpName = getGroupName.GroupName;
                using (var db = new DataClass.myDbContext())
                {
                    dynamic selectedItem = dataGrid.SelectedItems[0];
                    long uId = selectedItem.Id;
                    var qDates = db.AHistories.Where(x => x.UserId == uId && x.GroupName.Equals(gpName)).Select(x => new { x.DatePass, x.Id }).OrderByDescending(x => x.DatePass).ToList();
                    cmbAzmon.ItemsSource = qDates.Any() ? qDates : null;
                }
            }
            catch (Exception)
            {
            }
        }

        private void cmbAzmon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gpChart.Visibility = Visibility.Visible;
            double[] values = new double[] { };

            try
            {
                dynamic getGroupName = cmbGroups.SelectedItem;
                string gpName = getGroupName.GroupName;
                dynamic getDateText = cmbAzmon.SelectedItem;
                string dPass = getDateText.DatePass;
                using (var db = new DataClass.myDbContext())
                {
                    dynamic selectedItem = dataGrid.SelectedItems[0];
                    long uId = selectedItem.Id;
                    var data = db.AHistories.Where(x => x.UserId == uId && x.DatePass == dPass && x.GroupName.Equals(gpName)).Select(x => x).OrderByDescending(x => x.DatePass).ToList();
                    values = new double[] { data.FirstOrDefault().TrueItem, data.FirstOrDefault().FalseItem, data.FirstOrDefault().NoneItem };

                    Series series = new ColumnSeries();

                    switch (FindElement.Settings.ChartType ?? 0)
                    {
                        case 0:
                            series = new ColumnSeries { };
                            break;

                        case 1:
                            series = new StackedColumnSeries { };
                            break;

                        case 2:
                            series = new LineSeries { };
                            break;

                        case 3:
                            series = new StepLineSeries { };
                            break;

                        case 4:
                            series = new StackedAreaSeries { };
                            break;
                    }

                    if (series.GetType() == typeof(ColumnSeries))
                    {
                        AchievementChart.Series.Add(new ColumnSeries
                        {
                            Values = new ChartValues<double>(values),
                            StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                        });
                    }
                    else if (series.GetType() == typeof(LineSeries))
                    {
                        AchievementChart.Series.Add(new LineSeries
                        {
                            Values = new ChartValues<double>(values),
                            StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                        });
                    }
                    else if (series.GetType() == typeof(StackedAreaSeries))
                    {
                        AchievementChart.Series.Add(new StackedAreaSeries
                        {
                            Values = new ChartValues<double>(values),
                            StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                        });
                    }
                    else if (series.GetType() == typeof(StackedColumnSeries))
                    {
                        AchievementChart.Series.Add(new StackedColumnSeries
                        {
                            Values = new ChartValues<double>(values),
                            StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                        });
                    }
                    else if (series.GetType() == typeof(StepLineSeries))
                    {
                        AchievementChart.Series.Add(new StepLineSeries
                        {
                            Values = new ChartValues<double>(values),
                            StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                        });
                    }

                    AchievementChart.AxisX.Add(new Axis
                    {
                        Labels = new string[] { "پاسخ صحیح", "پاسخ غلط", "بدون پاسخ" },
                        Separator = new LiveCharts.Wpf.Separator { }
                    });
                    txtName.Text = dPass;
                    txtBook.Text = gpName;
                }
            }
            catch (Exception)
            {
            }
        }
    }
}