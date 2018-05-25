using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for TopStudents.xaml
    /// </summary>
    public partial class TopStudents : UserControl
    {
        private List<DataClass.DataTransferObjects.StudentsScoresDto> _initialCollection;

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

        private void getStudent(long BaseId)
        {
            try
            {
                var query = GetAllStudentsAsync(BaseId);
                query.Wait();

                List<DataClass.DataTransferObjects.StudentsScoresDto> data = query.Result;
                _initialCollection = data;

                if (data.Any())
                {
                    var res = _initialCollection.GroupBy(x => new { x.StudentId })
                            .Select(x => new
                            {
                                x.Key.StudentId,
                                Name = x.FirstOrDefault().Name,
                                LName = x.FirstOrDefault().LName,
                                FName = x.FirstOrDefault().FName,
                                Sum = x.Sum(y => AppVariable.EnumToNumber(y.Scores))
                            }).OrderByDescending(x => x.Sum).ToArray();

                    dataGrid.ItemsSource = res.ToList();
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

        #endregion Async Query

        private void cmbBaseEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbBaseEdit.SelectedValue));
        }
    }
}