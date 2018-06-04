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
        private string changedDate = string.Empty;
        public System.Windows.Media.Brush BorderColor { get; set; }

        public Attendancelist()
        {
            InitializeComponent();

            this.DataContext = this;
            main = this;
            strDate = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");
            txtDate.Text = string.Format("تاریخ امروز : {0} ", strDate);
            BorderColor = AppVariable.GetBrush(MainWindow.main.BorderBrush.ToString());
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

        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsEditAsync(long BaseId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.OrderBy(x => x.LName).Where(x => x.BaseId == BaseId).Select(x => new DataClass.DataTransferObjects.StudentsDto { Name = x.Name, LName = x.LName, FName = x.FName, BaseId = x.BaseId, Id = x.Id });
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.Tables.Attendance>> GetAttendanceAsync(long StudentId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Attendances.Where(x => x.StudentId == StudentId).OrderByDescending(x => x.Date).Select(x => x);
                return await query.ToListAsync();
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
                    var query = db.Schools.Select(x => x);
                    if (query.Any())
                    {
                        cmbBase.ItemsSource = query.ToList();
                        cmbEditBase.ItemsSource = query.ToList();
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

                List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;

                _initialCollection = query.Result;

                if (data.Any())
                {
                    this.dataGrid.ItemsSource = data;
                    swAllPresent.IsEnabled = true;
                }
                else
                {
                    this.dataGrid.ItemsSource = null;
                    swAllPresent.IsEnabled = false;
                    MainWindow.main.ShowNoDataNotification("Student");
                }
            }
            catch (Exception)
            {
            }
        }

        private void getStudentEdit(long BaseId)
        {
            try
            {
                var query = GetAllStudentsEditAsync(BaseId);
                query.Wait();
                List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;
                if (data.Any())
                {
                    cmbEditStudent.ItemsSource = data;
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

        private void addAttendance(long StudentId, bool Exist, string Date)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var Attendance = new DataClass.Tables.Attendance();
                    Attendance.StudentId = StudentId;
                    Attendance.Exist = Exist;
                    Attendance.Date = Date;
                    db.Attendances.Add(Attendance);
                    db.SaveChanges();
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
                var query = GetAttendanceAsync(StudentId);
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

        private void deleteAttendance(long StudentId, long AttendanceId)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var DeleteAttendance = db.Attendances.FirstOrDefault(x => x.StudentId == StudentId && x.Id == AttendanceId);

                    db.Attendances.Remove(DeleteAttendance);
                    db.SaveChanges();
                }
            }
            catch (Exception)
            {
            }
        }

        private void updateAttendance(long AttendanceId, long StudentId, bool Exist, string Date)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditAttendance =  db.Attendances.FirstOrDefault(x => x.StudentId == StudentId && x.Id == AttendanceId);
                EditAttendance.Exist = Exist;

                EditAttendance.Date = Date;
                db.SaveChanges();
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
            getStudent(Convert.ToInt64(cmbBase.SelectedValue));
        }

        private void swAllPresent_Checked(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count != 0)
            {
                for (int i = 0; i < _initialCollection.Count; i++)
                {
                    dataGrid.SelectedIndex = i;
                    dynamic selectedItem = dataGrid.SelectedItems[0];
                    addAttendance((long)selectedItem.Id, true, strDate);
                    UpdateList(Convert.ToInt64(selectedItem.Id), 10);
                }
            }
        }

        private void UpdateList(long SelectedItem, double time)
        {
            Task.Delay(TimeSpan.FromMilliseconds(time)).ContinueWith(ctx =>
            {
                _initialCollection.RemoveAll(x => x.Id == SelectedItem);
                dataGrid.ItemsSource = _initialCollection.Select(x => x);
                if (!_initialCollection.Any())
                    swAllPresent.IsEnabled = false;
                else
                    swAllPresent.IsEnabled = true;
            },
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void chkIsPresent_Checked(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count != 0)
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];

                addAttendance((long)selectedItem.Id, true, strDate);
                UpdateList(Convert.ToInt64(selectedItem.Id), 400);
            }
        }

        private void chkIsAbsent_Checked(object sender, RoutedEventArgs e)
        {
            if (dataGrid.Items.Count != 0)
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];

                addAttendance((long)selectedItem.Id, false, strDate);
                UpdateList(Convert.ToInt64(selectedItem.Id), 400);
            }
        }

        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudentEdit(Convert.ToInt64(cmbEditBase.SelectedValue));
        }

        private void cmbEditStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getAttendance(Convert.ToInt64(cmbEditStudent.SelectedValue));
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dgv.ItemsSource != null)
            {
                if (txtEditSearch.Text != string.Empty)
                    dgv.ItemsSource = _initialCollectionAtendance.Where(x => x.Date.Contains(txtEditSearch.Text)).Select(x => x);
                else
                    dgv.ItemsSource = _initialCollectionAtendance.Select(x => x);
            }
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
                getAttendance(Convert.ToInt64(cmbEditStudent.SelectedValue));
            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, selectedItemCmb.Name + " " + selectedItemCmb.LName, "حضورغیاب");
            }
        }

        private void MetroSwitch_Checked(object sender, RoutedEventArgs e)
        {
            isPresentEdit = true;
        }

        private void MetroSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            isPresentEdit = false;
        }

        private void txtDateEdit_SelectedDateChanged(object sender, RoutedEventArgs e)
        {
            var mytxtDate = sender as PersianCalendarWPF.PersianDatePicker;
            changedDate = mytxtDate.Text.ToString();
        }

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItemCmb = cmbEditStudent.SelectedItem;
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
                long id = selectedItem.Id;
                long studentId = selectedItem.StudentId;
                updateAttendance(id, studentId, isPresentEdit, changedDate);
                MainWindow.main.ShowUpdateDataNotification(true, selectedItemCmb.Name + " " + selectedItemCmb.LName, "حضورغیاب");
                getAttendance(Convert.ToInt64(cmbEditStudent.SelectedValue));
            }
            catch (Exception)
            {
                MainWindow.main.ShowUpdateDataNotification(false, selectedItemCmb.Name + " " + selectedItemCmb.LName, "حضورغیاب");
            }
        }

        private void dgv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
                changedDate = selectedItem.Date;
            }
            catch (Exception)
            {
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbBase.SelectedIndex = FindElement.Settings.DefaultSchool ?? -1;
            cmbEditBase.SelectedIndex = FindElement.Settings.DefaultSchool ?? -1;
        }
    }
}