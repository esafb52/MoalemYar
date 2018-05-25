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
                List<ScoreBook> parts = new List<ScoreBook>();
                var list = new List<string>();
                List<double> listScore = new List<double>();

                // get list of all book
                foreach (var item in _initialCollection)
                {
                    if (!parts.Any(x => x.BookName == item.Book))
                    {
                        parts.Add(new ScoreBook() { Id = item.Id, BookName = item.Book });
                    }
                }

                // create chart based on number of book exist
                foreach (var item in parts)
                {
                    var score = _initialCollection.Where(x => x.Book == item.BookName && x.StudentId == selectedItem.Id).Select(x => x);
                    MaterialChart _addUser;
                    Control _currentUser;

                    // get all date for student
                    foreach (var sitem in score)
                    {
                        if (!list.Exists(x=>e.Equals(sitem.Date)))
                        {
                            list.Add(sitem.Date);
                            var d = score.Where(x => x.Scores == "خیلی خوب" && x.Date == sitem.Date).Count();
                            var d1 = score.Where(x => x.Scores == "خوب" && x.Date == sitem.Date).Count();
                            var d2 = score.Where(x => x.Scores == "قابل قبول" && x.Date == sitem.Date).Count();
                            var d3 = score.Where(x => x.Scores == "نیاز به تلاش بیشتر" && x.Date == sitem.Date).Count();
                            listScore.Add(((d * 4) + (d1 * 3) + (d2 * 2) + (d3 * 1)));
                        }

                    }


                    List<String> duplicates = list.GroupBy(x => x)
                             .Where(g => g.Count() > 1)
                             .Select(g => g.Key)
                             .ToList();



                    //convert to string[]
                    String[] bok = { };
                    bok = duplicates.ToArray();

                    double[] scd = { };
                    scd = listScore.ToArray();


                    _addUser = new MaterialChart(item.BookName, selectedItem.Name + " " + selectedItem.LName, bok, scd, series, AppVariable.GetBrush(Convert.ToString(Setting[AppVariable.ChartColor] ?? AppVariable.CHART_GREEN)));
                    _currentUser = _addUser;
                    waterfallFlow.Children.Add(_currentUser);
                    waterfallFlow.Refresh();
                }
            }
            catch (NullReferenceException)
            {

            }
        }

    }
}
public class ScoreBook
{
    public long Id { get; set; }
    public string BookName { get; set; }
}
