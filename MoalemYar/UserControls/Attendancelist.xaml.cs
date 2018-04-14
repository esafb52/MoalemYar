
/****************************** ghost1372.github.io ******************************\
*	Module Name:	Attendancelist.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 13, 01:23 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
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
    /// Interaction logic for Attendancelist.xaml
    /// </summary>
    public partial class Attendancelist : UserControl
    {
        private bool runOnceSchool = true;
        internal static Attendancelist main;
        private List<DataClass.DataTransferObjects.StudentsDto> _initialCollection;
        private PersianCalendar pc = new PersianCalendar();
        private static string strDate;
        public Attendancelist()
        {
            InitializeComponent();

            this.DataContext = this;
            main = this;
            strDate = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");
            txtDate.Text = string.Format("تاریخ امروز : {0} ", strDate);
        }

        #region "Async Query"

        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsAsync( long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.OrderBy(x => x.LName).Where(x => !db.Attendances.Any(f => f.StudentId == x.Id && (f.Date == strDate)) && x.BaseId == BaseId).Select(x => new DataClass.DataTransferObjects.StudentsDto { Name = x.Name, LName = x.LName, FName = x.FName, BaseId = x.BaseId, Id = x.Id });
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.Tables.School>> GetAllSchoolsAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Select(x => x);
                return await query.ToListAsync();
            }
        }

        public static async Task<string> DeleteStudentAsync(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteStudent = await db.Students.FindAsync(id);
                db.Students.Remove(DeleteStudent);
                await db.SaveChangesAsync();
                return "Student Deleted Successfully";
            }
        }

        public async static Task<string> UpdateStudentAsync(long id, long BaseId, string Name, string LName, string FName, string Gender, byte[] Image)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditStudent = await db.Students.FindAsync(id);
                EditStudent.Name = Name;

                EditStudent.LName = LName;

                EditStudent.FName = FName;
                EditStudent.Gender = Gender;
                EditStudent.BaseId = BaseId;
                EditStudent.Image = Image;
                await db.SaveChangesAsync();
                return "Student Updated Successfully";
            }
        }

        public async static Task<string> InsertAttendanceAsync(long StudentId, bool Exist, string Date)
        {
            using (var db = new DataClass.myDbContext())
            {
                var Attendance = new DataClass.Tables.Attendance();
                Attendance.StudentId = StudentId;
                Attendance.Exist = Exist;
                Attendance.Date = Date;
                
                db.Attendances.Add(Attendance);

                await db.SaveChangesAsync();

                return "Attendances Added Successfully";
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

                _initialCollection = query.Result;

                if (data.Any())
                {
                    this.listView1.ItemsSource = data.ToList();
                    swAllPresent.IsEnabled = true;
                }
                else
                {
                    swAllPresent.IsEnabled = false;
                    MainWindow.main.ShowNoDataNotification("Student");
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteStudent(long id)
        {
            var query = DeleteStudentAsync(id);
            query.Wait();
            MainWindow.main.getexHint();
        }

        private void updateStudent(long id, long BaseId, string Name, string LName, string FName, string Gender, byte[] Image)
        {
            var query = UpdateStudentAsync(id, BaseId, Name, LName, FName, Gender, Image);
            query.Wait();
        }

        private void addAttendance(long StudentId, bool Exist, string Date)
        {
            var query = InsertAttendanceAsync(StudentId, Exist, Date);
            query.Wait();
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

        private void dgv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //editGrid.IsEnabled = true;
                //dynamic selectedItem = dgv.SelectedItems[0];
                //txtName.Text = selectedItem.Name;
                //txtLName.Text = selectedItem.LName;
                //txtFName.Text = selectedItem.FName;
                //setComboValue(selectedItem.Gender);
                //cmbEditBase.SelectedValue = selectedItem.BaseId;

                //byte[] bytes = selectedItem.Image as byte[];
                //MemoryStream stream = new MemoryStream(bytes);
                //BitmapImage image = new BitmapImage();
                //image.BeginInit();
                //image.StreamSource = stream;
                //image.EndInit();
                //imgEditStudent.Source = image;
            }
            catch (Exception)
            {
            }
        }
        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            //dynamic selectedItem = dgv.SelectedItems[0];
            //long id = selectedItem.Id;

            //updateStudent(id, Convert.ToInt64(cmbEditBase.SelectedValue), txtName.Text, txtLName.Text, txtFName.Text, getComboValue(), CreateThumbnail(imgEditStudent.Source as BitmapImage));
            //MainWindow.main.ShowUpdateDataNotification(true, txtName.Text, "دانش آموز");
            //editGrid.IsEnabled = false;
            //getStudent();
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            //txtName.Text = string.Empty;
            //txtLName.Text = string.Empty;
            //txtFName.Text = string.Empty;
            //setComboValue(null);
            //cmbEditBase.SelectedIndex = -1;
            //editGrid.IsEnabled = false;
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if (txtEditSearch.Text != string.Empty)
            //    dgv.ItemsSource = _initialCollection.Where(x => x.Username.Contains(txtEditSearch.Text)).Select(x => x);
            //else
            //    dgv.ItemsSource = _initialCollection.Select(x => x);
        }
        
        private void txtEditSearch_ButtonClick(object sender, EventArgs e)
        {
            //getStudent();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            //MainWindow.main.ShowDeleteConfirmNotification(txtName.Text, "دانش آموز");
        }

        public void deleteStudent()
        {
            //try
            //{
            //    dynamic selectedItem = dgv.SelectedItems[0];
            //    long id = selectedItem.Id;
            //    deleteStudent(id);
            //    MainWindow.main.ShowDeletedNotification(true, txtName.Text, "دانش آموز");
            //    editGrid.IsEnabled = false;
            //    getStudent();
            //}
            //catch (Exception)
            //{
            //    MainWindow.main.ShowDeletedNotification(false, txtName.Text, "دانش آموز");
            //}
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

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }


        private void chkIsPresent_Checked(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = listView1.SelectedItems[0];

            addAttendance((long)selectedItem.Id, true, strDate);
            UpdateList(Convert.ToInt64(selectedItem.Id));
        }

        private void chkIsAbsent_Checked(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = listView1.SelectedItems[0];

            addAttendance((long)selectedItem.Id, false, strDate);
            UpdateList(Convert.ToInt64(selectedItem.Id));
        }

        private void cmbBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbBase.SelectedValue));
        }
        
        private void UpdateList(long SelectedItem)
        {
            Task.Delay(500).ContinueWith(ctx =>
                {
                    _initialCollection.RemoveAll(x => x.Id == SelectedItem);
                    listView1.ItemsSource = _initialCollection.Select(x => x);
                    if (!_initialCollection.Any())
                        swAllPresent.IsEnabled = false;
                    else
                        swAllPresent.IsEnabled = true;
                },
                TaskScheduler.FromCurrentSynchronizationContext());
            
        }

        private void swAllPresent_Checked(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _initialCollection.Count; i++)
            {
                listView1.SelectedIndex = i;
                dynamic selectedItem = listView1.SelectedItems[0];
                addAttendance((long)selectedItem.Id, true, strDate);
                UpdateList(Convert.ToInt64(selectedItem.Id));
            }
        }
    }
}
