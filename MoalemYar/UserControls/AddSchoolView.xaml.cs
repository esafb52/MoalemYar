/****************************** ghost1372.github.io ******************************\
*	Module Name:	AddSchool.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 30, 10:38 ق.ظ
*
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddSchool.xaml
    /// </summary>
    public partial class AddSchoolView : UserControl
    {
        internal static AddSchoolView main;
        private PersianCalendar pc = new PersianCalendar();
        private string strDate;
        private List<DataClass.Tables.School> _initialCollection;

        public AddSchoolView()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;

            GenerateEducateYear();
            getSchool();
        }

        #region Query"

        private void getSchool()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.ToList();
                    _initialCollection = query;
                    if (query.Any())
                    {
                        dataGrid.ItemsSource = query.OrderBy(x=>x.Year);
                    }
                    else
                    {
                        dataGrid.ItemsSource = null;
                        MainWindow.main.showNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "School");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteSchool(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var checkQuery = db.Students.Where(x => x.BaseId == id).Any();
                if (checkQuery)
                {
                    MainWindow.main.showNotification(NotificationKEY: AppVariable.Delete_Exist_KEY, param: new[] { "مدرسه", "دانش آموزان" });
                }
                else
                {
                    var DeleteSchool = db.Schools.Find(id);
                    db.Schools.Remove(DeleteSchool);
                    db.SaveChanges();
                    MainWindow.main.showNotification(AppVariable.Deleted_KEY, true, txtSchool.Text, "مدرسه");
                }
            }
        }

        private void updateSchool(long id, string Name, string Base, string Admin, string Year)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditSchool = db.Schools.Find(id);
                EditSchool.SchoolName = Name;
                EditSchool.Base = Base;
                EditSchool.Admin = Admin;
                EditSchool.Year = Year;
                db.SaveChanges();
            }
        }

        private void addSchool(string Name, string Base, string Admin, string Year)
        {
            using (var db = new DataClass.myDbContext())
            {
                var School = new DataClass.Tables.School();
                School.SchoolName = Name;
                School.Base = Base;
                School.Admin = Admin;
                School.Year = Year;
                db.Schools.Add(School);

                db.SaveChanges();
            }
        }

        #endregion Query"

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ClearScreen();
        }

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                updateSchool(id, txtSchool.Text, cmbEditBase.Text, txtAdmin.Text, txtYear.Text);
                MainWindow.main.showNotification(AppVariable.Update_Data_KEY, true, txtSchool.Text, "مدرسه");
                editGrid.IsEnabled = false;
                getSchool();
            }
            catch (Exception)
            {
                MainWindow.main.showNotification(AppVariable.Update_Data_KEY, false, txtSchool.Text, "مدرسه");
            }
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            txtAdmin.Text = string.Empty;
            txtSchool.Text = string.Empty;
            txtYear.Text = string.Empty;
            setComboValue(null);
            editGrid.IsEnabled = false;
            dataGrid.UnselectAll();
        }

        private void setComboValue(string index)
        {
            switch (index)
            {
                case "اول":
                    cmbEditBase.SelectedIndex = 0;
                    break;

                case "دوم":
                    cmbEditBase.SelectedIndex = 1;
                    break;

                case "سوم":
                    cmbEditBase.SelectedIndex = 2;
                    break;

                case "چهارم":
                    cmbEditBase.SelectedIndex = 3;
                    break;

                case "پنجم":
                    cmbEditBase.SelectedIndex = 4;
                    break;

                case "ششم":
                    cmbEditBase.SelectedIndex = 5;
                    break;

                case null:
                    cmbEditBase.SelectedIndex = -1;
                    break;
            }
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGrid.ItemsSource != null)
            {
                if (txtEditSearch.Text != string.Empty)
                    dataGrid.ItemsSource = _initialCollection.Where(x => x.SchoolName.Contains(txtEditSearch.Text) || x.Admin.Contains(txtEditSearch.Text) || x.Base.Contains(txtEditSearch.Text) || x.Year.Contains(txtEditSearch.Text)).Select(x => x);
                else
                    dataGrid.ItemsSource = _initialCollection.Select(x => x);
            }
        }

        private void GenerateEducateYear()
        {
            strDate = pc.GetYear(DateTime.Now).ToString("0000");
            string Year = strDate.Substring(2, 2);
            int NextYear = Convert.ToInt32(Year) + 1;
            txtAddYear.Text = Year + "-" + NextYear;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddSchool.Text == string.Empty || txtAddAdmin.Text == string.Empty || txtAddYear.Text == string.Empty || cmbBase.SelectedIndex == -1)
            {
                MainWindow.main.showNotification(NotificationKEY: AppVariable.Fill_All_Data_KEY);
            }
            else
            {
                try
                {
                    addSchool(txtAddSchool.Text, cmbBase.Text, txtAddAdmin.Text, txtAddYear.Text);
                    MainWindow.main.showNotification(AppVariable.Add_Data_KEY, true, txtAddSchool.Text, "مدرسه");
                    txtAddAdmin.Text = string.Empty;
                    txtAddSchool.Text = string.Empty;
                    txtAddSchool.Focus();
                }
                catch (Exception)
                {
                    MainWindow.main.showNotification(AppVariable.Add_Data_KEY, false, txtAddSchool.Text, "مدرسه");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.showNotification(NotificationKEY: AppVariable.Delete_Confirm_KEY, param:new[] { txtSchool.Text, "مدرسه" });
        }

        public void deleteSchool()
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                deleteSchool(id);
                editGrid.IsEnabled = false;
                getSchool();
            }
            catch (Exception)
            {
                MainWindow.main.showNotification(AppVariable.Deleted_KEY, false, txtSchool.Text, "مدرسه");
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                txtAdmin.Text = selectedItem.Admin;
                txtSchool.Text = selectedItem.SchoolName;
                txtYear.Text = selectedItem.Year;
                setComboValue(selectedItem.Base);
                editGrid.IsEnabled = true;
            }
            catch (Exception)
            {
            }
        }
    }
}