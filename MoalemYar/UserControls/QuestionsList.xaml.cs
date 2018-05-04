
/****************************** ghost1372.github.io ******************************\
*	Module Name:	QuestionsList.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 28, 08:38 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Globalization;
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
    /// Interaction logic for QuestionsList.xaml
    /// </summary>
    public partial class QuestionsList : UserControl
    {
        ObservableCollection<string> list = new ObservableCollection<string>();
        private bool runOnceSchool = true;
        internal static QuestionsList main;
        private PersianCalendar pc = new PersianCalendar();
        private static string strDate;
        private List<DataClass.DataTransferObjects.SchoolsStudentsJointDto> _initialCollectionStudent;
        private List<DataClass.DataTransferObjects.StudentsDto> _initialCollection;
        private List<DataClass.Tables.Score> _initialCollectionScore;
        private string changedDate = string.Empty;

        public System.Windows.Media.Brush BorderColor { get; set; }

        public QuestionsList()
        {
            InitializeComponent();

            this.DataContext = this;
            main = this;
            strDate = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");
            txtDate.Text = string.Format("تاریخ امروز : {0} ", strDate);

            var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderColor = brush;
        }

        #region "Async Query"

        public async static Task<List<DataClass.Tables.School>> GetAllSchoolsAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Select(x => x);
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsAsync(long SchoolId, string Book)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.Where(x => !db.Questions.Any(f => f.StudentId == x.Id && f.Book == Book) && x.BaseId == SchoolId).Select(x => new DataClass.DataTransferObjects.StudentsDto { Id = x.Id, BaseId = x.BaseId, Name = x.Name, LName = x.LName, FName = x.FName });
                
                return await query.ToListAsync();
            }
        }

        public async static Task<string> InsertQuestionAsync(long SchoolId, long StudentId, string Book)
        {
            using (var db = new DataClass.myDbContext())
            {
                var Question = new DataClass.Tables.Question();
                Question.SchoolId = SchoolId;
                Question.StudentId = StudentId;
                Question.Book = Book;
                db.Questions.Add(Question);

                await db.SaveChangesAsync();

                return "Question Added Successfully";
            }
        }
        public async static Task<List<DataClass.DataTransferObjects.SchoolsStudentsJointDto>> GetAllStudentsAsync(long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Join(
                  db.Students,
                  c => c.Id,
                  v => v.BaseId,
                  (c, v) => new DataClass.DataTransferObjects.SchoolsStudentsJointDto { Name = v.Name, LName = v.LName, FName = v.FName, Gender = v.Gender, BaseId = v.BaseId, Image = v.Image, Id = v.Id, Base = c.Base }
              ).OrderBy(x => x.LName).Where(x => x.BaseId == BaseId);

                return await query.ToListAsync();
            }
        }
        public async static Task<List<DataClass.Tables.Score>> GetAllScoresAsync(long StudentId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Scores.Where(x => x.StudentId == StudentId).Select(x =>x).OrderByDescending(x=> new { x.Date, x.Book }).ThenBy(x=>x.Scores);
                return await query.ToListAsync();
            }
        }
        public async static Task<string> InsertScoreAsync(long StudentId, string Book, string Date, string Scorez, string Desc)
        {
            using (var db = new DataClass.myDbContext())
            {
                var Score = new DataClass.Tables.Score();
                Score.StudentId = StudentId;
                Score.Book = Book;
                Score.Date = Date;
                Score.Scores = Scorez;
                Score.Desc = Desc;

                db.Scores.Add(Score);

                await db.SaveChangesAsync();

                return "Score Added Successfully";
            }
        }

        public async static Task<string> UpdateQuestionAsync(long QuestionId ,long SchoolId, long StudentId, string Book)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditQuestion = await db.Questions.FirstOrDefaultAsync(x => x.StudentId == StudentId && x.Id == QuestionId);
                EditQuestion.SchoolId = SchoolId;
                EditQuestion.StudentId = StudentId;
                EditQuestion.Book = Book;

                await db.SaveChangesAsync();
                return "Question Updated Successfully";
            }
        }

        public async static Task<string> UpdateScoreAsync(long ScoreId, long StudentId, string Score, string Date, string Book, string Desc)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditScore = await db.Scores.FirstOrDefaultAsync(x => x.Id == ScoreId);
                EditScore.Scores = Score;
                EditScore.Book = Book;
                EditScore.Date = Date;
                EditScore.Desc = Desc;
                EditScore.StudentId = StudentId;
                await db.SaveChangesAsync();
                return "EditScore Updated Successfully";
            }
        }

        public static async Task<string> DeleteQuestionAsync(long SchoolId, string Book)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteQuestion = await db.Questions.Where(x => x.SchoolId == SchoolId && x.Book == Book).ToListAsync();

                db.Questions.RemoveRange(DeleteQuestion);
                await db.SaveChangesAsync();
                return "Question Deleted Successfully";
            }
        }

        public static async Task<string> DeleteScoreAsync(long ScoreId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteScore = await db.Scores.Where(x => x.Id == ScoreId).ToListAsync();

                db.Scores.RemoveRange(DeleteScore);
                await db.SaveChangesAsync();
                return "Scores Deleted Successfully";
            }
        }
        #endregion "Async Query"

        #region Func get Query Wait"

        private void getSchool()
        {
            try
            {
                var query = GetAllSchoolsAsync();
                query.Wait();
                List<DataClass.Tables.School> data = query.Result;
                if (data.Any())
                {
                    cmbBase.ItemsSource = data;
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

                List<DataClass.DataTransferObjects.SchoolsStudentsJointDto> data = query.Result;
                _initialCollectionStudent = query.Result;

                if (data.Any())
                {
                    cmbEditStudent.ItemsSource = data.ToList();
                }
                else
                {
                    cmbEditStudent.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("Student");
                }
            }
            catch (Exception)
            {
            }
        }

        private void getScores(long StudentId)
        {
            try
            {
                var query = GetAllScoresAsync(StudentId);
                query.Wait();

                List<DataClass.Tables.Score> data = query.Result;
                _initialCollectionScore = query.Result;

                if (data.Any())
                {
                    dataGridEdit.ItemsSource = data.ToList();
                }
                else
                {
                    dataGridEdit.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("Score");
                }
            }
            catch (Exception)
            {
            }
        }

        private void getStudents(long SchoolId, string Book)
        {
            var query = GetAllStudentsAsync(SchoolId, Book);
            query.Wait();
            List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;
            _initialCollection = data;
            if (data.Any())
            {
                dataGrid.ItemsSource = data;
            }
            else
            {
                dataGrid.ItemsSource = null;
                MainWindow.main.ShowNoDataNotification("Question");
            }
           
        }

        private void addQuestion(long SchoolId, long StudentId, string Book)
        {
            try
            {
                var query = InsertQuestionAsync(SchoolId, StudentId, Book);
                query.Wait();
            }
            catch (Exception)
            {
            }
        }

        private void addScore(long StudentId, string Book, string Date, string Scorez, string Desc)
        {
            try
            {
                var query = InsertScoreAsync(StudentId, Book, Date, Scorez, Desc);
                query.Wait();
            }
            catch (Exception)
            {
            }
        }
        private void updateQuestion(long QuestionId, long SchoolId, long StudentId, string Book)
        {
            var query = UpdateQuestionAsync(QuestionId, SchoolId, StudentId, Book);
            query.Wait();
        }

        private void updateScore(long ScoreId, long StudentId, string Score, string Date, string Book, string Desc)
        {
            var query = UpdateScoreAsync(ScoreId, StudentId, Score, Date, Book, Desc);
            query.Wait();
        }
        private void deleteQuestion(long SchoolId, string Book)
        {
            try
            {
                var query = DeleteQuestionAsync(SchoolId, Book);
                query.Wait();
            }
            catch (Exception)
            {
            }
        }
        private void deleteScore(long ScoreId)
        {
            try
            {
                var query = DeleteScoreAsync(ScoreId);
                query.Wait();
            }
            catch (Exception)
            {
            }
        }
        #endregion Func get Query Wait"
        private void tabc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (runOnceSchool)
            {
                getSchool();
                runOnceSchool = false;
            }
        }

        private void cmbBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillComboBook(cmbAddContentBook, cmbBase);
        }

        private void fillComboBook(FrameworkElement frameworkElement, ComboBox combo)
        {
            var element = FindElementByName<ComboBox>(frameworkElement, "cmbBook");
            dynamic selectedItem = combo.SelectedItem;

            if (selectedItem.Base.Contains("اول"))
            {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");

            }
            else if (selectedItem.Base.Contains("دوم"))
            {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");
                list.Add("هدیه های آسمانی");

            }
            else if (selectedItem.Base.Contains("سوم") || selectedItem.Base.Contains("چهارم") || selectedItem.Base.Contains("پنجم"))
            {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");
                list.Add("هدیه های آسمانی");
                list.Add("مطالعات اجتماعی");

            }
            else if (selectedItem.Base.Contains("ششم"))
            {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");
                list.Add("هدیه های آسمانی");
                list.Add("مطالعات اجتماعی");
                list.Add("کار و فناوری");
                list.Add("تفکر");
            }

            element.ItemsSource = list;
        }
        private void cmbBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var element = FindElementByName<ComboBox>(cmbAddContentBook, "cmbBook");
                getStudents(Convert.ToInt64(cmbBase.SelectedValue), element.SelectedItem.ToString());
            }
            catch (Exception)
            {

            }
            

        }

        public T FindElementByName<T>(FrameworkElement element, string sChildName) where T : FrameworkElement
        {
            T childElement = null;
            var nChildCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < nChildCount; i++)
            {
                FrameworkElement child = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(sChildName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, sChildName);

                if (childElement != null)
                    break;
            }
            return childElement;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
        private void chkChecked_Checked(object sender, RoutedEventArgs e)
        {
            var row = dataGrid.ContainerFromElement(sender as DependencyObject);
            Arthas.Controls.Metro.MetroTextBlock MyTextBlock = FindVisualChildByName<Arthas.Controls.Metro.MetroTextBlock>(row, "txtStatus");
            dynamic selectedItem = dataGrid.SelectedItems[0];
            var element = FindElementByName<ComboBox>(cmbAddContentBook, "cmbBook");
            switch ((sender as Arthas.Controls.Metro.MetroSwitch).Tag.ToString())
            {
                case "exc":
                    if (MyTextBlock.Text == "ثبت نشده")
                    {
                        MyTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        MyTextBlock.Text = "ثبت شده";
                        addQuestion((long)selectedItem.BaseId, (long)selectedItem.Id, element.SelectedItem.ToString());
                        addScore((long)cmbBase.SelectedValue, element.SelectedItem.ToString(), strDate, "خیلی خوب", (txtDesc.Text==string.Empty ? "بدون توضیحات" : txtDesc.Text));
                    }
                    break;
                case "good":
                    if (MyTextBlock.Text == "ثبت نشده")
                    {
                        MyTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        MyTextBlock.Text = "ثبت شده";
                        addQuestion((long)selectedItem.BaseId, (long)selectedItem.Id, element.SelectedItem.ToString());
                        addScore((long)cmbBase.SelectedValue, element.SelectedItem.ToString(), strDate, "خوب", (txtDesc.Text == string.Empty ? "بدون توضیحات" : txtDesc.Text));
                    }
                    break;
                case "nbad":
                    if (MyTextBlock.Text == "ثبت نشده")
                    {
                        MyTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        MyTextBlock.Text = "ثبت شده";
                        addQuestion((long)selectedItem.BaseId, (long)selectedItem.Id, element.SelectedItem.ToString());
                        addScore((long)cmbBase.SelectedValue, element.SelectedItem.ToString(), strDate, "قابل قبول", (txtDesc.Text == string.Empty ? "بدون توضیحات" : txtDesc.Text));
                    }
                    break;
                case "bad":
                    if (MyTextBlock.Text == "ثبت نشده")
                    {
                        MyTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                        MyTextBlock.Text = "ثبت شده";
                        addQuestion((long)selectedItem.BaseId, (long)selectedItem.Id, element.SelectedItem.ToString());
                        addScore((long)cmbBase.SelectedValue, element.SelectedItem.ToString(), strDate, "نیاز به تلاش بیشتر", (txtDesc.Text == string.Empty ? "بدون توضیحات" : txtDesc.Text));
                    }
                    break;
            }
            var DeleteQuestion = _initialCollection.Where(x => x.Id == (long)selectedItem.Id).FirstOrDefault();
            _initialCollection.Remove(DeleteQuestion);

            if (!_initialCollection.Any())
            {
                deleteQuestion((long)cmbBase.SelectedValue, element.SelectedItem.ToString());
            }
        }
        public T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);

                    if (result != null)

                        return result;
                }
            }
            return null;
        }
        private void StackPanel_Checked(object sender, RoutedEventArgs e)
        {
            Arthas.Controls.Metro.MetroSwitch cb = e.OriginalSource as Arthas.Controls.Metro.MetroSwitch;
            if (cb.IsChecked == false)
            {
                return;
            }
            foreach (var item in ((StackPanel)sender).Children)
            {
                if (item != cb)
                {
                    ((Arthas.Controls.Metro.MetroSwitch)item).IsChecked = false;
                }
            }
        }

        #region "Edit"
        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillComboBook(cmbAddContentBookEdit, cmbEditBase);

            getStudent(Convert.ToInt64(cmbEditBase.SelectedValue));
        }

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItemName = cmbEditStudent.SelectedItem;
            dynamic selectedItem = dataGridEdit.SelectedItems[0];
            long id = selectedItem.Id;


            var element = FindElementByName<ComboBox>(cmbContentScore, "cmbScore");
            var element2 = FindElementByName<ComboBox>(cmbAddContentBookEdit, "cmbBook");
            
            updateScore(id, Convert.ToInt64(cmbEditStudent.SelectedValue), element.Text,txtDateEdit.SelectedDate.ToString(), element2.Text, txtDescEdit.Text);
            MainWindow.main.ShowUpdateDataNotification(true, selectedItemName.Name + " " + selectedItemName.LName, "نمره");
            getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEditSearch.Text != string.Empty)
                dataGridEdit.ItemsSource = _initialCollectionScore.Where(x => x.Book.Contains(txtEditSearch.Text) || x.Date.Contains(txtEditSearch.Text) || x.Scores.Contains(txtEditSearch.Text) || x.Desc.Contains(txtEditSearch.Text)).Select(x => x);
            else
                dataGridEdit.ItemsSource = _initialCollectionScore.Select(x => x);
        }

        private void txtEditSearch_ButtonClick(object sender, EventArgs e)
        {
            getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = cmbEditStudent.SelectedItem;
            MainWindow.main.ShowDeleteConfirmNotification(selectedItem.Name + " " + selectedItem.LName, "نمره");
        }

        public void deleteScore()
        {
            dynamic selectedItemName = cmbEditStudent.SelectedItem;
            try
            {
                dynamic selectedItem = dataGridEdit.SelectedItems[0];
                long id = selectedItem.Id;
                deleteScore(id);
                MainWindow.main.ShowDeletedNotification(true, selectedItemName.Name + " " + selectedItemName.LName, "نمره");
                getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, selectedItemName.Name + " " + selectedItemName.LName, "نمره");
            }
        }
        private void cmbEditStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
        }

        private void txtDateEdit_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            var mytxtDate = sender as PersianCalendarWPF.PersianDatePicker;
            changedDate = mytxtDate.Text.ToString();
        }

        #endregion

        private void dataGridEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGridEdit.SelectedItems[0];
                txtDateEdit.SelectedDate = new PersianCalendarWPF.PersianDate(Convert.ToInt32(selectedItem.Date.Substring(0, 4)), Convert.ToInt32(selectedItem.Date.Substring(5, 2)), Convert.ToInt32(selectedItem.Date.Substring(8, 2)));
                txtDescEdit.Text = selectedItem.Desc;
                var element = FindElementByName<ComboBox>(cmbContentScore, "cmbScore");
                var element2 = FindElementByName<ComboBox>(cmbAddContentBookEdit, "cmbBook");
                element.Text = selectedItem.Scores;
                element2.Text = selectedItem.Book;

            }
            catch (Exception)
            {
            }
        }
    }
}
