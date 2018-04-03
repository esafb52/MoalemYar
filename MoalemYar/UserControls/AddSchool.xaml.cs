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
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddSchool.xaml
    /// </summary>
    public partial class AddSchool : UserControl
    {
        private System.ComponentModel.BackgroundWorker MyWorker = new System.ComponentModel.BackgroundWorker();
        public Brush BorderColor { get; set; }
        internal static AddSchool main;
        private int runOnce = 0;
        private PersianCalendar pc = new PersianCalendar();
        private string strDate;

        public AddSchool()
        {
            InitializeComponent();
            main = this;
            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderColor = brush;
            MyWorker.WorkerSupportsCancellation = true;
            MyWorker.DoWork += MyWorker_DoWork;
            MyWorker.RunWorkerCompleted += MyWorker_RunWorkerCompleted;
            GenerateEducateYear();
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

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.exContent.Content = null;
        }

        private void tabc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabc.SelectedIndex == 1)
            {
                if (runOnce == 0)
                {
                    if (!MyWorker.IsBusy)
                        MyWorker.RunWorkerAsync();
                    runOnce = 1;
                }
            }
        }

        private void MyWorker_DoWork(object Sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                using (var db = new DataClass.myDbContext())
                {
                    var data = from x in db.Schools select new { x.SchoolName, x.Admin, x.Base, x.Year, x.Id };

                    if (data.Any())
                        dgv.ItemsSource = data.ToList();
                    else
                        MainWindow.main.ShowNoDataNotification("School");
                }
            }), DispatcherPriority.ContextIdle);

            if (MyWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }

        private void MyWorker_RunWorkerCompleted(object Sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
        }

        private void dgv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
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

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
                long id = selectedItem.Id;
                using (var db = new DataClass.myDbContext())
                {
                    var data = db.Schools.Where(s => s.Id == id).FirstOrDefault<DataClass.Tables.School>();
                    data.Admin = txtAdmin.Text;
                    data.SchoolName = txtSchool.Text;
                    data.Year = txtYear.Text;
                    data.Base = getComboValue();
                    db.SaveChanges();
                    MainWindow.main.ShowUpdateDataNotification(true, txtSchool.Text, "مدرسه");
                    editGrid.IsEnabled = false;
                    if (!MyWorker.IsBusy)
                        MyWorker.RunWorkerAsync();
                }
            }
            catch (Exception)
            {
                MainWindow.main.ShowUpdateDataNotification(false, txtSchool.Text, "مدرسه");
            }
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            txtAdmin.Text = string.Empty;
            txtSchool.Text = string.Empty;
            txtYear.Text = string.Empty;
            setComboValue(null);
            editGrid.IsEnabled = false;
        }

        private string getComboValue()
        {
            var element = FindElementByName<ComboBox>(cmbContent, "cmbBase");
            return element.Text;
        }

        private void setComboValue(string index)
        {
            var element = FindElementByName<ComboBox>(cmbContent, "cmbBase");
            switch (index)
            {
                case "اول":
                    element.SelectedIndex = 0;
                    break;

                case "دوم":
                    element.SelectedIndex = 1;
                    break;

                case "سوم":
                    element.SelectedIndex = 2;
                    break;

                case "چهارم":
                    element.SelectedIndex = 3;
                    break;

                case "پنجم":
                    element.SelectedIndex = 4;
                    break;

                case "ششم":
                    element.SelectedIndex = 5;
                    break;

                case null:
                    element.SelectedIndex = -1;
                    break;
            }
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEditSearch.Text != string.Empty)
            {
                using (var db = new DataClass.myDbContext())
                {
                    var data = from x in db.Schools.Where(t => t.SchoolName.Contains(txtEditSearch.Text) || t.Admin.Contains(txtEditSearch.Text) || t.Base.Contains(txtEditSearch.Text) || t.Year.Contains(txtEditSearch.Text)) select new { x.SchoolName, x.Admin, x.Base, x.Year, x.Id };
                    dgv.ItemsSource = data.ToList();
                }
            }
            else
            {
                if (!MyWorker.IsBusy)
                    MyWorker.RunWorkerAsync();
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
            var element = FindElementByName<ComboBox>(cmbAddContent, "cmbBase");
            if (txtAddSchool.Text == string.Empty || txtAddAdmin.Text == string.Empty || txtAddYear.Text == string.Empty || element.SelectedIndex == -1)
            {
                MainWindow.main.ShowFillAllDataNotification();
            }
            else
            {
                try
                {
                    using (var db = new DataClass.myDbContext())
                    {
                        var data = new DataClass.Tables.School()
                        {
                            Admin = txtAddAdmin.Text,
                            SchoolName = txtAddSchool.Text,
                            Year = txtAddYear.Text,
                            Base = element.Text
                        };
                        db.Schools.Add(data);
                        db.SaveChanges();
                        MainWindow.main.ShowAddDataNotification(true, txtAddSchool.Text, "مدرسه");
                        txtAddAdmin.Text = string.Empty;
                        txtAddSchool.Text = string.Empty;
                        txtAddSchool.Focus();
                    }
                }
                catch (Exception)
                {
                    MainWindow.main.ShowAddDataNotification(true, txtAddSchool.Text, "مدرسه");
                }
            }
        }

        private void txtEditSearch_ButtonClick(object sender, EventArgs e)
        {
            if (!MyWorker.IsBusy)
                MyWorker.RunWorkerAsync();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
                long id = selectedItem.Id;
                using (var db = new DataClass.myDbContext())
                {
                    var data = db.Schools.Where(s => s.Id == id).FirstOrDefault<DataClass.Tables.School>();
                    db.Schools.Remove(data);
                    db.SaveChanges();
                    MainWindow.main.ShowDeletedNotification(true, txtSchool.Text, "مدرسه");
                    editGrid.IsEnabled = false;
                    if (!MyWorker.IsBusy)
                        MyWorker.RunWorkerAsync();
                }
            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, txtSchool.Text, "مدرسه");
            }
        }
    }
}