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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
        private List<DataClass.Tables.Attendance> _initialCollectionAtendance;
        private PersianCalendar pc = new PersianCalendar();
        private static string strDate;
        private bool isPresentEdit = true;

        public Attendancelist()
        {
            InitializeComponent();

            this.DataContext = this;
            main = this;
            strDate = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");
            txtDate.Text = string.Format("تاریخ امروز : {0} ", strDate);
        }

        #region "Async Query"

        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsAsync(long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.OrderBy(x => x.LName).Where(x => !db.Attendances.Any(f => f.StudentId == x.Id && (f.Date == strDate)) && x.BaseId == BaseId).Select(x => new DataClass.DataTransferObjects.StudentsDto { Name = x.Name, LName = x.LName, FName = x.FName, BaseId = x.BaseId, Id = x.Id });
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsPersonAsync(long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Join(
                  db.Students,
                  c => c.Id,
                  v => v.BaseId,
                  (c, v) => new DataClass.DataTransferObjects.StudentsDto { Name = v.Name, LName = v.LName, FName = v.FName, Id = v.Id, BaseId = v.BaseId }
              ).OrderBy(x => x.LName).Where(x => x.BaseId == BaseId);

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

        public async static Task<List<DataClass.Tables.Attendance>> GetAllAttendanceAsync(long StudentId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Attendances.Where(x => x.StudentId == StudentId).Select(x => x);
                return await query.ToListAsync();
            }
        }

        public static async Task<string> DeleteAttendanceAsync(long StudentId, long AttendanceId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteAttendance = await db.Attendances.FirstOrDefaultAsync(x => x.StudentId == StudentId && x.Id == AttendanceId);

                db.Attendances.Remove(DeleteAttendance);
                await db.SaveChangesAsync();
                return "Attendance Deleted Successfully";
            }
        }

        public async static Task<string> UpdateAttendanceAsync(long AttendanceId, long StudentId, bool Exist, string Date)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditAttendance = await db.Attendances.FirstOrDefaultAsync(x => x.StudentId == StudentId && x.Id == AttendanceId);
                EditAttendance.Exist = Exist;

                EditAttendance.Date = Date;
                await db.SaveChangesAsync();
                return "Attendance Updated Successfully";
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
                    cmbEditBase.ItemsSource = data;
                }
            }
            catch (Exception)
            {
            }
        }

        private void getAttendance(long StudentId)
        {
            try
            {
                var query = GetAllAttendanceAsync(StudentId);
                query.Wait();
                List<DataClass.Tables.Attendance> data = query.Result;
                _initialCollectionAtendance = data;
                if (data.Any())
                {
                    dgv.ItemsSource = data;
                }
                else
                {
                    dgv.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("Attendance");
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

        private void getStudentPerson(long BaseId)
        {
            try
            {
                var query = GetAllStudentsPersonAsync(BaseId);
                query.Wait();

                List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;
                if (data.Any())
                {
                    this.cmbEditStudent.ItemsSource = data.ToList();
                }
                else
                {
                    MainWindow.main.ShowNoDataNotification("Student");
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteAttendance(long StudentId, long AttendanceId)
        {
            var query = DeleteAttendanceAsync(StudentId, AttendanceId);
            query.Wait();
            MainWindow.main.getexHint();
        }

        private void updateAttendance(long AttendanceId, long StudentId, bool Exist, string Date)
        {
            var query = UpdateAttendanceAsync(AttendanceId, StudentId, Exist, Date);
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
                editGrid.IsEnabled = true;
                dynamic selectedItem = dgv.SelectedItems[0];
                bool isPresent = selectedItem.Exist;
                txtDateEdit.Text = selectedItem.Date;

                if (isPresent)
                {
                    chkEditIsPresent.IsChecked = true;
                    chkEditIsAbsent.IsChecked = false;
                }
                else
                {
                    chkEditIsAbsent.IsChecked = true;
                    chkEditIsPresent.IsChecked = false;
                }
            }
            catch (Exception)
            {
            }
        }

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItemCmb = cmbEditStudent.SelectedItem;
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
                long id = selectedItem.Id;
                long studentId = selectedItem.StudentId;
                updateAttendance(id, studentId, isPresentEdit, txtDateEdit.Text.ToString());
                MainWindow.main.ShowUpdateDataNotification(true, selectedItemCmb.Name + " " + selectedItemCmb.LName, "حضورغیاب");
                editGrid.IsEnabled = false;
                getAttendance(Convert.ToInt64(cmbEditStudent.SelectedValue));
            }
            catch (Exception)
            {

                MainWindow.main.ShowUpdateDataNotification(false, selectedItemCmb.Name + " " + selectedItemCmb.LName, "حضورغیاب");
            }
            
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            editGrid.IsEnabled = false;
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEditSearch.Text != string.Empty)
                dgv.ItemsSource = _initialCollectionAtendance.Where(x => x.Date.Contains(txtEditSearch.Text)).Select(x => x);
            else
                dgv.ItemsSource = _initialCollectionAtendance.Select(x => x);
        }

        private void txtEditSearch_ButtonClick(object sender, EventArgs e)
        {
            getAttendance(Convert.ToInt64(cmbEditStudent.SelectedValue));
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = cmbEditStudent.SelectedItem;

                MainWindow.main.ShowDeleteConfirmNotification(selectedItem.Name + " " + selectedItem.LName, "حضورغیاب");
            }
            catch (Exception)
            {
            }
        }

        public void deleteAttendance()
        {
            dynamic selectedItemCmb = cmbEditStudent.SelectedItem;
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
                long id = selectedItem.Id;
                long studentId = selectedItem.StudentId;

                deleteAttendance(studentId, id);
                MainWindow.main.ShowDeletedNotification(true, selectedItemCmb.Name + " " + selectedItemCmb.LName, "حضورغیاب");
                editGrid.IsEnabled = false;
                getAttendance(Convert.ToInt64(cmbEditStudent.SelectedValue));
            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, selectedItemCmb.Name + " " + selectedItemCmb.LName, "حضورغیاب");
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
            UpdateList(Convert.ToInt64(selectedItem.Id), 400);
        }

        private void chkIsAbsent_Checked(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = listView1.SelectedItems[0];

            addAttendance((long)selectedItem.Id, false, strDate);
            UpdateList(Convert.ToInt64(selectedItem.Id), 400);
        }

        private void cmbBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbBase.SelectedValue));
        }

        private void UpdateList(long SelectedItem, double time)
        {
            Task.Delay(TimeSpan.FromMilliseconds(time)).ContinueWith(ctx =>
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
                UpdateList(Convert.ToInt64(selectedItem.Id), 10);
            }
        }

        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudentPerson(Convert.ToInt64(cmbEditBase.SelectedValue));
        }

        private void cmbEditStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getAttendance(Convert.ToInt64(cmbEditStudent.SelectedValue));
        }

        private void chkEditIsPresent_Checked(object sender, RoutedEventArgs e)
        {
            chkEditIsAbsent.IsChecked = false;
            if (chkEditIsPresent.IsChecked == true)
            {
                isPresentEdit = true;
            }
        }

        private void chkEditIsAbsent_Checked(object sender, RoutedEventArgs e)
        {
            chkEditIsPresent.IsChecked = false;
            if (chkEditIsAbsent.IsChecked == true)
            {
                isPresentEdit = false;
            }
        }
    }
}