using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
                        dataGrid.ItemsSource = query.ToList();
                    }
                    else
                    {
                        MainWindow.main.ShowNoDataNotification(null);
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
                    GenerateMaterialChart(item.Book, selectedItem.Name + " " + selectedItem.LName, getDateArray(item.Book), getScoreArray(item.Book), getAverage(item.Book), getAverageStatus(item.Book), series);
                }

            }
            catch (ArgumentNullException) { }
            catch (NullReferenceException)
            {
            }
        }
        private void GenerateMaterialChart(string Book, string Name, string[] Label, double[] values, string Average, string AverageStatus, Series series)
        {
            Effect effect = this.FindResource("EffectShadow3") as Effect;
            Border mainborder = new Border
            {
                Margin = new Thickness(0, 0, 30, 30),
                Width = 300,
                Height= 330,
                Background = new SolidColorBrush(Colors.White),
                CornerRadius = new CornerRadius(5),
                Effect = effect
            };
            Grid grid = new Grid();
            var row1 = new RowDefinition();
            row1.Height = new GridLength(0, GridUnitType.Auto);

            var row2 = new RowDefinition();
            row2.Height = new GridLength(0, GridUnitType.Auto);

            var row3 = new RowDefinition();
            row3.Height = new GridLength(.50, GridUnitType.Star);

            var row4 = new RowDefinition();
            row4.Height = new GridLength(.5, GridUnitType.Star);

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);
            grid.RowDefinitions.Add(row3);
            grid.RowDefinitions.Add(row4);

            Border bord = new Border
            {
                Background = AppVariable.GetBrush(FindElement.Settings.ChartColor ?? AppVariable.CHART_GREEN),
                CornerRadius = new CornerRadius(5)
            };
            Grid.SetRow(bord, 0);
            Grid.SetRowSpan(bord, 3);
            grid.Children.Add(bord);

            TextBlock txtBook = new TextBlock
            {
                Padding = new Thickness(10, 10, 0, 5),
                FontSize = 18,
                Foreground = new SolidColorBrush(Colors.White),
                TextAlignment = TextAlignment.Center,
                Text = Book
            };
            Grid.SetRow(txtBook, 0);
            grid.Children.Add(txtBook);

            TextBlock txtName = new TextBlock
            {
                Padding = new Thickness(0, 0, 0, 20),
                FontSize = 18,
                Foreground = AppVariable.GetBrush("#59FFFFFF"),
                TextAlignment = TextAlignment.Center,
                Text = Name
            };
            Grid.SetRow(txtName, 1);
            grid.Children.Add(txtName);

            CartesianChart cChart = new CartesianChart
            {
                Margin = new Thickness(10, 0, 10, 20),
                DataTooltip = null,
                Hoverable = false
            };
            Grid.SetRow(cChart, 2);

            if (series.GetType() == typeof(ColumnSeries))
            {
                cChart.Series.Add(new ColumnSeries
                {
                    Style = TryFindResource("columnSeries") as Style,
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            else if (series.GetType() == typeof(LineSeries))
            {
                cChart.Series.Add(new LineSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            else if (series.GetType() == typeof(StackedAreaSeries))
            {
                cChart.Series.Add(new StackedAreaSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            else if (series.GetType() == typeof(StackedColumnSeries))
            {
                cChart.Series.Add(new StackedColumnSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            else if (series.GetType() == typeof(StepLineSeries))
            {
                cChart.Series.Add(new StepLineSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            cChart.AxisX.Add(new Axis
            {
                Style = TryFindResource("axis") as Style,
                Labels = Label,
                Separator = new LiveCharts.Wpf.Separator { Style = TryFindResource("seperator") as Style }
            });
            grid.Children.Add(cChart);

            StackPanel stk = new StackPanel
            {
                Margin = new Thickness(20, 0, 20, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetRow(stk, 3);

            TextBlock txt = new TextBlock
            {
                FontSize = 13,
                Opacity = .4,
                Text = " میانگین نمرات این درس برابر است با:"
            };
            stk.Children.Add(txt);

            StackPanel stkH = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };
            TextBlock txtAverageDouble = new TextBlock
            {
                FontSize = 30,
                Foreground = AppVariable.GetBrush("#303030"),
                Text = Average
            };
            stkH.Children.Add(txtAverageDouble);

            TextBlock txtAverage = new TextBlock
            {
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(8, 6, 8, 6),
                Foreground = AppVariable.GetBrush("#303030"),
                Text = AverageStatus
            };
            stkH.Children.Add(txtAverage);
            Grid.SetRow(stk, 3);
            stk.Children.Add(stkH);
            grid.Children.Add(stk);

            mainborder.Child = grid;
            waterfallFlow.Children.Add(mainborder);
        }

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

            var one = decimal.Divide(sum, 1);
            var sec = decimal.Divide(sum, 2);
            var thi = decimal.Divide(sum, 3);
            var forth = decimal.Divide(sum, 4);

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

            var Avg = decimal.Divide(sum, dCount).ToString("0.00");

            var one = decimal.Divide(dCount * 4, 1).ToString("0.00");
            var sec = decimal.Divide(dCount * 4, 2).ToString("0.00");
            var thi = decimal.Divide(dCount * 4, 3).ToString("0.00");
            var forth = decimal.Divide(dCount * 4, 4).ToString("0.00");

            string status = string.Empty;

            if (Convert.ToDecimal(Avg) >= Convert.ToDecimal(sec))
                status = "خیلی خوب";
            else if (Convert.ToDecimal(Avg) < Convert.ToDecimal(one) && Convert.ToDecimal(Avg) >= Convert.ToDecimal(thi))
                status = "خوب";
            else if (Convert.ToDecimal(Avg) < Convert.ToDecimal(sec) && Convert.ToDecimal(Avg) >= Convert.ToDecimal(forth))
                status = "قابل قبول";
            else if (Convert.ToDecimal(Avg) < Convert.ToDecimal(forth))
                status = "نیاز به تلاش بیشتر";

            return status;
        }

        //get Dates to string[]
        private string[] getDateArray(string Book)
        {
            var score = _initialCollection.GroupBy(x => new { x.Book, x.Date, x.StudentId })
                           .Select(x => new
                           {
                               x.Key.StudentId,
                               x.Key.Book,
                               x.Key.Date,
                               Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                           }).Where(x => x.Book == Book).ToArray();
            return score.Select(x => x.Date).ToArray();
        }

        //get Scores to double[]
        private double[] getScoreArray(string Book)
        {
            var score = _initialCollection.GroupBy(x => new { x.Book, x.Date, x.StudentId })
                           .Select(x => new
                           {
                               x.Key.StudentId,
                               x.Key.Book,
                               x.Key.Date,
                               Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                           }).Where(x => x.Book == Book).ToArray();
            return score.Select(x => Convert.ToDouble(x.Sum)).ToArray();
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            cmbEditBase.SelectedIndex = Convert.ToInt32(FindElement.Settings.DefaultSchool);
        }
    }
}