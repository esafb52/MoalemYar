
/****************************** ghost1372.github.io ******************************\
*	Module Name:	AddUser.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 2, 10:56 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
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
using System.Windows.Threading;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : UserControl
    {
        private System.ComponentModel.BackgroundWorker MyWorker = new System.ComponentModel.BackgroundWorker();
        public Brush BorderColor { get; set; }
        internal static AddUser main;
        private int runOnce = 0;
       
        public AddUser()
        {
            InitializeComponent();
            main = this;
            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderColor = brush;
            MyWorker.WorkerSupportsCancellation = true;
            MyWorker.DoWork += MyWorker_DoWork;
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
                    var data = from x in db.Users select new { x.Username, x.Password, x.Id };

                    if (data.Any())
                        dgv.ItemsSource = data.ToList();
                    else
                        MainWindow.main.ShowNoDataNotification("User");
                }
            }), DispatcherPriority.ContextIdle);

            if (MyWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
        }

        private void dgv_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dgv.SelectedItems[0];
                txtUsername.Text = selectedItem.Username;
                txtPassword.Text = selectedItem.Password;
                txtPasswordAg.Text = selectedItem.Password;
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
                if (txtPassword.Text != txtPasswordAg.Text)
                {
                    MainWindow.main.ShowSamePasswordNotification();
                }
                else
                {
                    dynamic selectedItem = dgv.SelectedItems[0];
                    long id = selectedItem.Id;
                    using (var db = new DataClass.myDbContext())
                    {
                        var data = db.Users.Where(s => s.Id == id).FirstOrDefault<DataClass.Tables.User>();
                        data.Username = txtUsername.Text;
                        data.Password = txtPassword.Text;

                        db.SaveChanges();
                        MainWindow.main.ShowUpdateDataNotification(true, txtUsername.Text, "نام کاربری");
                        editGrid.IsEnabled = false;
                        if (!MyWorker.IsBusy)
                            MyWorker.RunWorkerAsync();
                    }
                }
               
            }
            catch (Exception)
            {
                MainWindow.main.ShowUpdateDataNotification(false, txtUsername.Text, "نام کاربری");
            }
        }
        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            txtUsername.Text = string.Empty;
            txtPassword.Text = string.Empty;
            txtPasswordAg.Text = string.Empty;
            editGrid.IsEnabled = false;
        }
        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEditSearch.Text != string.Empty)
            {
                using (var db = new DataClass.myDbContext())
                {
                    var data = from x in db.Users.Where(t => t.Username.Contains(txtEditSearch.Text)) select new { x.Username, x.Password, x.Id };
                    dgv.ItemsSource = data.ToList();
                }
            }
            else
            {
                if (!MyWorker.IsBusy)
                    MyWorker.RunWorkerAsync();
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddUsername.Text == string.Empty || txtAddPassword.Text == string.Empty || txtAddPasswordAg.Text == string.Empty)
            {
                MainWindow.main.ShowFillAllDataNotification();
            }else if(txtAddPassword.Text !=txtAddPasswordAg.Text)
            {
                MainWindow.main.ShowSamePasswordNotification();
            }else
            {
                try
                {
                    using (var db = new DataClass.myDbContext())
                    {
                        var data = new DataClass.Tables.User()
                        {
                            Username = txtAddUsername.Text,
                            Password = txtAddPassword.Text
                           
                        };
                        db.Users.Add(data);
                        db.SaveChanges();
                        MainWindow.main.ShowAddDataNotification(true, txtAddUsername.Text,"نام کاربری");
                        txtAddUsername.Text = string.Empty;
                        txtAddPassword.Text = string.Empty;
                        txtAddPasswordAg.Text = string.Empty;
                        txtAddPassword.Focus();
                    }
                }
                catch (Exception)
                {
                    MainWindow.main.ShowAddDataNotification(false, txtAddUsername.Text, "نام کاربری");
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
                    var data = db.Users.Where(s => s.Id == id).FirstOrDefault<DataClass.Tables.User>();
                    db.Users.Remove(data);
                    db.SaveChanges();
                    MainWindow.main.ShowDeletedNotification(true, txtUsername.Text, "نام کاربری");
                    editGrid.IsEnabled = false;
                    if (!MyWorker.IsBusy)
                        MyWorker.RunWorkerAsync();
                }
            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, txtUsername.Text, "نام کاربری");
            }
        }
    }
}
