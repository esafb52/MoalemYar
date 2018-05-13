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

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for TopStudents.xaml
    /// </summary>
    public partial class TopStudents : UserControl
    {
        public TopStudents()
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
        public async static Task<List<DataClass.DataTransferObjects.StudentsScoresDto>> GetAllStudentsAsync(long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Scores.Join(
                  db.Students,
                  c => c.StudentId,
                  v => v.Id,
                  (c, v) => new DataClass.DataTransferObjects.StudentsScoresDto { Id = c.Id, BaseId = v.BaseId, StudentId = v.Id, Name = v.Name, LName = v.LName, FName = v.FName, Scores = c.Scores }
              ).OrderBy(x => x.Scores).Where(x => x.BaseId == BaseId);

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
                    cmbBaseEdit.ItemsSource = data;
                }
            }
            catch (Exception)
            {
            }
        }

        ObservableCollection<DataClass.DataTransferObjects.StudentsScoresDto> list = new ObservableCollection<DataClass.DataTransferObjects.StudentsScoresDto>();


        private void getStudent(long BaseId)
        {
            try
            {
                var query = GetAllStudentsAsync(BaseId);
                query.Wait();

                List<DataClass.DataTransferObjects.StudentsScoresDto> data = query.Result;
                List<TopScore> parts = new List<TopScore>();

                foreach (var item in data)
                {
                    if (!list.Any(x=>x.StudentId == item.StudentId))
                    {
                        list.Add(item);
                    }
                }
                
                foreach (var item in list)
                {
                    var d = data.Where(x => x.StudentId == item.StudentId && x.Scores == "خیلی خوب").Count();
                    var d1 = data.Where(x => x.StudentId == item.StudentId && x.Scores == "خوب").Count();
                    var d2= data.Where(x => x.StudentId == item.StudentId && x.Scores == "قابل قبول").Count();
                    var d3 = data.Where(x => x.StudentId == item.StudentId && x.Scores == "نیاز به تلاش بیشتر").Count();
                    parts.Add(new TopScore() { Id = item.StudentId, Score = ((d * 4) + (d1 * 3) + (d2 * 2) + (d3 * 1)),Name = item.Name, LName = item.LName, FName = item.FName });
                }


                if (data.Any())
                {
                    dataGrid.ItemsSource = parts.OrderByDescending(x=>x.Score).ToList();
                }
                else
                {
                    dataGrid.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("TopStudent");
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion
        private void cmbBaseEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbBaseEdit.SelectedValue));
        }
    }
}
public class TopScore 
{
    public long Id { get; set; }
    public int Score { get; set; }
    public string Name { get; set; }
    public string LName { get; set; }
    public string FName { get; set; }

}
