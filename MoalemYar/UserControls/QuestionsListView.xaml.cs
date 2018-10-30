/****************************** ghost1372.github.io ******************************\
*	Module Name:	QuestionsList.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 28, 08:38 ب.ظ
*
***********************************************************************************/

using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for QuestionsList.xaml
    /// </summary>
    public partial class QuestionsListView : UserControl
    {
        private ObservableCollection<string> list = new ObservableCollection<string>();
        internal static QuestionsListView main;
        private PersianCalendar pc = new PersianCalendar();
        private List<DataClass.DataTransferObjects.SchoolsStudentsJointDto> _initialCollectionStudent;
        private List<DataClass.DataTransferObjects.StudentsDto> _initialCollection;
        private List<DataClass.Tables.Score> _initialCollectionScore;
        long insertedScoreId = 0;

        public QuestionsListView()
        {
            InitializeComponent();

            main = this;
            getSchool();
        }

        #region "Async Query"

        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsAsync(long SchoolId, string Book, bool isExam)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = (isExam == true) ? db.Students.Where(x => x.BaseId == SchoolId)
                    .Select(x => new DataClass.DataTransferObjects.StudentsDto { Id = x.Id, BaseId = x.BaseId, Name = x.Name, LName = x.LName, FName = x.FName }) :
                    db.Students.Where(x => !db.Questions.Any(f => f.StudentId == x.Id && f.Book == Book) && x.BaseId == SchoolId)
                    .Select(x => new DataClass.DataTransferObjects.StudentsDto { Id = x.Id, BaseId = x.BaseId, Name = x.Name, LName = x.LName, FName = x.FName });

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

        public async static Task<long> InsertScoreAsync(long StudentId, string Book, string Date, string Scorez, string Desc)
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
                return Score.Id;
            }
        }

        #endregion "Async Query"

        #region Func get Query Wait"

        private void getSchool()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.ToList();
                    if (query.Any())
                    {
                        cmbBase.ItemsSource = query;
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
                    MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "Student");
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
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Scores.Where(x => x.StudentId == StudentId).Select(x => x).OrderByDescending(x => new { x.Date, x.Book }).ThenBy(x => x.Scores);
                    _initialCollectionScore = query.ToList();
                    if (query.Any())
                    {
                        dataGridEdit.ItemsSource = query.ToList();
                    }
                    else
                    {
                        dataGridEdit.ItemsSource = null;
                        MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "Score");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void getStudents(long SchoolId, string Book, bool isExam)
        {
            var query = GetAllStudentsAsync(SchoolId, Book, isExam);
            query.Wait();
            List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;
            _initialCollection = data;
            if (data.Any())
            {
                dataGrid.ItemsSource = data.OrderBy(x => x.LName);
            }
            else
            {
                dataGrid.ItemsSource = null;
                MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "Question");
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

        private long addScore(long StudentId, string Book, string Date, string Scorez, string Desc)
        {
            var query = InsertScoreAsync(StudentId, Book, Date, Scorez, Desc);
            try
            {
                query.Wait();
            }
            catch (Exception)
            {
            }
            return query.Result;
        }

        private void updateScore(long ScoreId, long StudentId, string Score, string Date, string Book, string Desc)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditScore = db.Scores.FirstOrDefault(x => x.Id == ScoreId);
                EditScore.Scores = Score;
                EditScore.Book = Book;
                EditScore.Date = Date;
                EditScore.Desc = Desc;
                EditScore.StudentId = StudentId;
                db.SaveChanges();
            }
        }

        private void deleteQuestion(long SchoolId, string Book)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var DeleteQuestion = db.Questions.Where(x => x.SchoolId == SchoolId && x.Book == Book).ToList();

                    db.Questions.RemoveRange(DeleteQuestion);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteScore(long ScoreId)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var DeleteScore = db.Scores.Where(x => x.Id == ScoreId).ToList();

                    db.Scores.RemoveRange(DeleteScore);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion Func get Query Wait"

        private void cmbBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fillComboBook(cmbBase, cmbBook);
        }

        private void fillComboBook(ComboBox Schoolcombo, ComboBox SourceCombo)
        {
            try
            {
                dynamic selectedItem = Schoolcombo.SelectedItem;

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
                SourceCombo.ItemsSource = list;
            }
            catch (NullReferenceException) { }
            catch (RuntimeBinderException) { }
        }
        private void chkChecked_Checked(object sender, RoutedEventArgs e)
        {
            if(cmbBook.SelectedIndex == -1 || cmbBase.SelectedIndex == -1)
            {
                MainWindow.main.showGrowlNotification(AppVariable.Fill_All_Data_KEY);
            }
            else
            {
                var row = dataGrid.ContainerFromElement(sender as DependencyObject);
                var MyTextBlock = FindElement.FindVisualChildByName<TextBlock>(row, "txtStatus");

                dynamic selectedItem = dataGrid.SelectedItems[0];
                var selectedChk = sender as ToggleButton;
                string newStatus = string.Empty;
                switch ((selectedChk).Tag.ToString())
                {
                    case "exc":
                        newStatus = "خیلی خوب";
                        break;

                    case "good":

                        newStatus = "خوب";
                        break;

                    case "nbad":
                        newStatus = "قابل قبول";
                        break;

                    case "bad":
                        newStatus = "نیاز به تلاش بیشتر";
                        break;
                }
                if (MyTextBlock.Text == "ثبت نشده")
                {
                    if (isQuestion.IsChecked == true)
                        addQuestion((long)selectedItem.BaseId, (long)selectedItem.Id, cmbBook.SelectedItem.ToString());

                    insertedScoreId = addScore((long)selectedItem.Id, cmbBook.SelectedItem.ToString(), txtDate.SelectedDate.ToString(), newStatus, (txtDesc.Text == string.Empty ? "بدون توضیحات" : txtDesc.Text));
                    MyTextBlock.Foreground = new SolidColorBrush(Colors.Green);
                    MyTextBlock.Text = newStatus + " ثبت شده ";
                }
                else
                {
                    updateScore(insertedScoreId, (long)selectedItem.Id, newStatus, txtDate.SelectedDate.ToString(), cmbBook.SelectedItem.ToString(), (txtDesc.Text == string.Empty ? "بدون توضیحات" : txtDesc.Text));
                }

                var DeleteQuestion = _initialCollection.Where(x => x.Id == (long)selectedItem.Id).FirstOrDefault();
                _initialCollection.Remove(DeleteQuestion);

                if (!_initialCollection.Any())
                {
                    deleteQuestion((long)cmbBase.SelectedValue, cmbBook.SelectedItem.ToString());
                }
            }
        }

        #region "Edit"

        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbEditBase.SelectedValue));
        }

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItemName = cmbEditStudent.SelectedItem;

            try
            {
                dynamic selectedItem = dataGridEdit.SelectedItems[0];
                long id = selectedItem.Id;

                updateScore(id, Convert.ToInt64(cmbEditStudent.SelectedValue), cmbScore.Text, txtDateEdit.SelectedDate.ToString(), cmbBookEdit.Text, txtDescEdit.Text);
                MainWindow.main.showGrowlNotification(AppVariable.Update_Data_KEY, true, selectedItemName.Name + " " + selectedItemName.LName, "نمره");
                getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
            }
            catch (Exception)
            {
                MainWindow.main.showGrowlNotification(AppVariable.Update_Data_KEY, false, selectedItemName.Name + " " + selectedItemName.LName, "نمره");
            }
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGridEdit.ItemsSource != null)
            {
                if (txtEditSearch.Text != string.Empty)
                    dataGridEdit.ItemsSource = _initialCollectionScore.Where(x => x.Book.Contains(txtEditSearch.Text) || x.Date.Contains(txtEditSearch.Text) || x.Scores.Contains(txtEditSearch.Text) || x.Desc.Contains(txtEditSearch.Text)).Select(x => x);
                else
                    dataGridEdit.ItemsSource = _initialCollectionScore.Select(x => x);
            }
        }

        private void txtEditSearch_ButtonClick(object sender, EventArgs e)
        {
            getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = cmbEditStudent.SelectedItem;
            string par0 = Convert.ToString(selectedItem.Name + " " + selectedItem.LName);
            MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.Delete_Confirm_KEY, param: new[] { par0, "نمره" });
        }

        public void deleteScore()
        {
            dynamic selectedItemName = cmbEditStudent.SelectedItem;
            try
            {
                dynamic selectedItem = dataGridEdit.SelectedItems[0];
                long id = selectedItem.Id;
                deleteScore(id);
                MainWindow.main.showGrowlNotification(AppVariable.Deleted_KEY, true, selectedItemName.Name + " " + selectedItemName.LName, "نمره");
                getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
            }
            catch (Exception)
            {
                MainWindow.main.showGrowlNotification(AppVariable.Deleted_KEY, false, selectedItemName.Name + " " + selectedItemName.LName, "نمره");
            }
        }

        private void cmbEditStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getScores(Convert.ToInt64(cmbEditStudent.SelectedValue));
            fillComboBook(cmbEditBase, cmbBookEdit);
        }

        private void dataGridEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGridEdit.SelectedItems[0];
                txtDateEdit.SelectedDate = new PersianCalendarWPF.PersianDate(Convert.ToInt32(selectedItem.Date.Substring(0, 4)), Convert.ToInt32(selectedItem.Date.Substring(5, 2)), Convert.ToInt32(selectedItem.Date.Substring(8, 2)));
                txtDescEdit.Text = selectedItem.Desc;
                cmbScore.Text = selectedItem.Scores;
                cmbBookEdit.Text = selectedItem.Book;
            }
            catch (Exception)
            {
            }
        }

        #endregion "Edit"

        private void isExam_Checked(object sender, RoutedEventArgs e)
        {
            if (isExam.IsChecked == true)
                isQuestion.IsChecked = false;
            cmbBook.IsEnabled = true;
        }

        private void isQuestion_Checked(object sender, RoutedEventArgs e)
        {
            if (isQuestion.IsChecked == true)
                isExam.IsChecked = false;
            cmbBook.IsEnabled = true;
        }

        private void cmbBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (isQuestion.IsChecked == true)
                {
                    getStudents(Convert.ToInt64(cmbBase.SelectedValue), cmbBook.SelectedItem.ToString(), false);
                    txtDesc.Text = "پرسش از درس " + cmbBook.SelectedItem;
                }
                else if (isExam.IsChecked == true)
                {
                    txtDesc.Text = "امتحان / فعالیت " + cmbBook.SelectedItem;
                    getStudents(Convert.ToInt64(cmbBase.SelectedValue), cmbBook.SelectedItem.ToString(), true);
                }
            }
            catch (Exception)
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbBase.SelectedIndex = Convert.ToInt32(FindElement.Settings.DefaultSchool);
            cmbEditBase.SelectedIndex = Convert.ToInt32(FindElement.Settings.DefaultSchool);
        }
    }
}