using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using LiveCharts;
using LiveCharts.Wpf;
using nucs.JsonSettings;
using nucs.JsonSettings.Fluent;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Achievement.xaml
    /// </summary>
    public partial class Achievement : UserControl
    {
        private SettingsBag Setting { get; } = JsonSettings.Construct<SettingsBag>(AppVariable.fileName + @"\config.json").EnableAutosave().LoadNow();
        private List<DataClass.Tables.Score> _initialCollection;

        public Achievement()
        {
            InitializeComponent();
           
            getSchool();
        }

        #region Async Query
        public async static Task<List<DataClass.Tables.School>> GetAllSchoolsAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Select(x => x);
                return await query.ToListAsync();
            }
        }
        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsAsync(long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.OrderBy(x => x.LName).Where(x => x.BaseId == BaseId).Select(x => new DataClass.DataTransferObjects.StudentsDto { Name = x.Name, LName = x.LName, FName = x.FName, BaseId = x.BaseId, Id = x.Id });
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.Tables.Score>> GetAllStudentsScoreAsync(long StudentId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Scores.Where(x => x.StudentId == StudentId).Select(x => x);
                return await query.ToListAsync();
            }
        }
        private void getSchool()
        {
            try
            {
                var query = GetAllSchoolsAsync();
                query.Wait();
                List<DataClass.Tables.School> data = query.Result;
                if (data.Any())
                {
                    cmbEditBase.ItemsSource = data;
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
                var query = GetAllStudentsAsync(BaseId);
                query.Wait();
                List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;
                if (data.Any())
                {
                    dataGrid.ItemsSource = data;
                }
                else
                {
                    MainWindow.main.ShowNoDataNotification(null);
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
                var query = GetAllStudentsScoreAsync(StudentId);
                query.Wait();
                List<DataClass.Tables.Score> data = query.Result;
                if (data.Any())
                    _initialCollection = data;
                else
                    _initialCollection = null;
            }
            catch (NullReferenceException)
            {
            }
        }


        #endregion
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
                switch (Setting[AppVariable.ChartType])
                {
                    case AppVariable.CHART_Column:
                        series = new ColumnSeries { };
                        break;
                    case AppVariable.CHART_Column2:
                        series = new StackedColumnSeries { };
                        break;
                    case AppVariable.CHART_Line:
                        series = new LineSeries { };
                        break;
                    case AppVariable.CHART_Line2:
                        series = new StepLineSeries { };
                        break;
                    case AppVariable.CHART_Area:
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
                                Sum = x.Sum(y => EnumToNumber(y.Scores))
                            }).ToArray();

                //get Book Count for generate chart
                var bookCount = score.GroupBy(x => new {x.Book })
                     .Select(g => new
                     {
                         g.Key.Book
                     }).ToList();

                MaterialChart _addUser;
                Control _currentUser;

                //generate chart based on count of books
                foreach (var item in bookCount)
                {
                    Console.WriteLine(getScoreArray(item.Book).FirstOrDefault());
                    _addUser = new MaterialChart(item.Book, selectedItem.Name + " " + selectedItem.LName, getDateArray(item.Book), getScoreArray(item.Book), series, AppVariable.GetBrush(Convert.ToString(Setting[AppVariable.ChartColor] ?? AppVariable.CHART_GREEN)));
                    _currentUser = _addUser;
                    waterfallFlow.Children.Add(_currentUser);
                }
             
                waterfallFlow.Refresh();
            }
            catch (NullReferenceException)
            {

            }
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
                               Sum = x.Sum(y => EnumToNumber(y.Scores))
                           }).Where(x=>x.Book == Book).ToArray();
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
                               Sum = x.Sum(y => EnumToNumber(y.Scores))
                           }).Where(x => x.Book == Book).ToArray();
            return score.Select(x => Convert.ToDouble(x.Sum)).ToArray();
        }

        //Convert string to int in linq
        public static int EnumToNumber(string value)
        {
            switch (value)
            {
                case "خیلی خوب":
                    return 4;

                case "خوب":
                    return 3;

                case "قابل قبول":
                    return 2;

                case "نیاز به تلاش بیشتر":
                    return 1;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Value not recognized");
            }
        }
    }
}

