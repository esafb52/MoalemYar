﻿using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Achievement.xaml
    /// </summary>
    public partial class AchievementView : UserControl
    {
        private List<DataClass.Tables.Score> _initialCollection;
        public ChartValues<DataClass.DataTransferObjects.myChartTemplate> Results { get; set; }
        public ObservableCollection<string> Labels { get; set; }

        public AchievementView()
        {
            InitializeComponent();
            getSchool();
        }

        #region Query

        public void getSchool()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.ToList();
                    if (query.Any())
                    {
                        cmbEditBase.ItemsSource = query;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void getStudent(long BaseId)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Students.OrderBy(x => x.LName).Where(x => x.BaseId == BaseId).Select(x => new DataClass.DataTransferObjects.StudentsDto { Name = x.Name, LName = x.LName, FName = x.FName, BaseId = x.BaseId, Id = x.Id });
                    if (query.Any())
                    {
                        dataGrid.ItemsSource = query.OrderBy(x => x.LName).ToList();
                    }
                    else
                    {
                        MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.No_Data_KEY, param: string.Empty);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void getStudentScore(long StudentId)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Scores.Where(x => x.StudentId == StudentId).ToList();
                    if (query.Any())
                        _initialCollection = query;
                    else
                        _initialCollection = null;
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        #endregion Query

        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbEditBase.SelectedValue));
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];

                waterfallFlow.Children.Clear();

                getStudentScore(selectedItem.Id); // get Student scores

                //get scores merge duplicates and replace string to int
                var score = _initialCollection.GroupBy(x => new { x.Book, x.Date, x.StudentId })
                            .Select(x => new
                            {
                                x.Key.StudentId,
                                x.Key.Book,
                                x.Key.Date,
                                Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                            }).ToArray();

                //get Book Count for generate chart
                var bookCount = score.GroupBy(x => new { x.Book })
                     .Select(g => new
                     {
                         g.Key.Book
                     }).ToList();

                //generate chart based on count of books
                foreach (var item in bookCount)
                {
                    Mapper = Mappers.Xy<DataClass.DataTransferObjects.myChartTemplate>()
                        .X((myData, index) => index)
                        .Y(myData => myData.Scores);

                    var records = getDataList(item.Book).OrderByDescending(x => x.Caption).ToArray();

                    Results = records.AsChartValues();

                    Labels = new ObservableCollection<string>(records.Select(x => x.Caption));

                    var chart = new CartesianChart();

                    var series = new SeriesCollection
                    {
                       new ColumnSeries{
                           Title = item.Book + Environment.NewLine + getAverageStatus(item.Book) + Environment.NewLine + "میانگین: " + getAverage(item.Book),
                           Configuration = Mapper, Values = Results, DataLabels = true, FontFamily = TryFindResource("TeacherYar.Fonts.IRANSans") as FontFamily,
                           Fill = TryFindResource("PrimaryBrush") as Brush
                       }
                    };
                    chart.Margin = new System.Windows.Thickness(5);
                    chart.Series = series;
                    chart.LegendLocation = LegendLocation.Top;
                    chart.AxisX.Add(new Axis
                    {
                        FontFamily = TryFindResource("TeacherYar.Fonts.IRANSans") as FontFamily,
                        Labels = Labels,
                        LabelsRotation = -20,
                        Separator = new LiveCharts.Wpf.Separator { Step = 1 }
                    });
                    chart.AxisY.Add(new Axis { FontFamily = TryFindResource("TeacherYar.Fonts.IRANSans") as FontFamily });
                    chart.DataTooltip.Background = TryFindResource("SecondaryRegionBrush") as Brush;
                    var mainBorder = new Border();
                    mainBorder.Width = 300;
                    mainBorder.Height = 320;
                    mainBorder.Effect = TryFindResource("EffectShadow3") as Effect;
                    mainBorder.CornerRadius = new System.Windows.CornerRadius(5);
                    mainBorder.Margin = new System.Windows.Thickness(10);
                    mainBorder.Background = TryFindResource("RegionBrush") as Brush;
                    mainBorder.Child = chart;

                    waterfallFlow.Children.Add(mainBorder);
                }
            }
            catch (ArgumentNullException) { }
            catch (NullReferenceException)
            {
            }
        }

        public object Mapper { get; set; }

        //get Score Average to string
        private string getAverage(string Book)
        {
            var score = _initialCollection.GroupBy(x => new { x.Book, x.Date, x.StudentId })
                           .Select(x => new
                           {
                               x.Key.StudentId,
                               x.Key.Book,
                               x.Key.Date,
                               Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                           }).Where(x => x.Book == Book).ToArray();
            var dCount = score.Select(x => x.Date).Count();
            var sum = score.Sum(x => x.Sum);
            
            return decimal.Divide(sum, dCount).ToString("0.00");
        }

        private string getAverageStatus(string Book)
        {
            var score = _initialCollection.GroupBy(x => new { x.Book, x.Date, x.StudentId })
                           .Select(x => new
                           {
                               x.Key.StudentId,
                               x.Key.Book,
                               x.Key.Date,
                               Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                           }).Where(x => x.Book == Book).ToArray();

            var sum = score.Sum(x => x.Sum);

            var dCount = score.Select(x => x.Date).Count();

            var TotalScore = Convert.ToDecimal(dCount * 4);
            var HalfScore = Convert.ToDecimal(decimal.Divide(dCount * 4, 2).ToString("0.00"));
            var ThirdScore = Convert.ToDecimal(decimal.Divide(dCount * 4, 3).ToString("0.00"));
            var ForthScore = Convert.ToDecimal(decimal.Divide(dCount * 4, 4).ToString("0.00"));

            string status = string.Empty;

            if (sum == TotalScore || (TotalScore - sum) <= 1)
                status = "خیلی خوب";
            else if (sum >= HalfScore && sum < TotalScore) 
                status = "خوب";
            else if (sum >= ThirdScore && sum < HalfScore)
                status = "قابل قبول";
            else if (sum >= ForthScore && sum < ThirdScore || sum <= ForthScore)
                status = "نیاز به تلاش بیشتر";

            return status;
        }

        private List<DataClass.DataTransferObjects.myChartTemplate> getDataList(string Book)
        {
            var score = _initialCollection.GroupBy(x => new { x.Book, x.Date, x.StudentId })
                           .Select(x => new
                           {
                               x.Key.StudentId,
                               x.Key.Book,
                               x.Key.Date,
                               Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                           }).Where(x => x.Book == Book).ToArray();
            return score.Select(x => new DataClass.DataTransferObjects.myChartTemplate { Book = x.Book, Caption = x.Date, Scores = x.Sum, StudentId = x.StudentId }).ToList();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            cmbEditBase.SelectedIndex = Convert.ToInt32(FindElement.Settings.DefaultSchool);
        }
    }
}