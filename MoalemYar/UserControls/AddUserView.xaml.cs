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
using System.Windows;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUserView : UserControl
    {
        internal static AddUserView main;
        private int runOnce = 0;
        private List<DataClass.Tables.User> _initialCollection;

        public AddUserView()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;
        }

        #region Query

        private void getUser()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Users.Select(x => x);
                    _initialCollection = query.ToList();
                    if (query.Any())
                    {
                        dataGrid.ItemsSource = query.ToList();
                    }
                    else
                    {
                        dataGrid.ItemsSource = null;
                        MainWindow.main.ShowNoDataNotification("User");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteUser(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteUser = db.Users.Find(id);
                db.Users.Remove(DeleteUser);
                db.SaveChanges();
            }
            MainWindow.main.getexHint();
        }

        private void updateUser(long id, string Username, string Password)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditUser = db.Users.Find(id);
                EditUser.Username = Username;
                EditUser.Password = Password;

                db.SaveChanges();
            }
        }

        private void addUser(string Username, string Password)
        {
            using (var db = new DataClass.myDbContext())
            {
                var User = new DataClass.Tables.User();
                User.Username = Username;
                User.Password = Password;

                db.Users.Add(User);

                db.SaveChanges();
            }
            MainWindow.main.getexHint();
        }

        #endregion Query

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.region.Content = null;
        }

        private void tabc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabc.SelectedIndex == 1)
            {
                if (runOnce == 0)
                {
                    getUser();
                    runOnce = 1;
                }
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
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
                    dynamic selectedItem = dataGrid.SelectedItems[0];
                    long id = selectedItem.Id;
                    updateUser(id, txtUsername.Text.ToLower(), txtPassword.Text.ToLower());
                    MainWindow.main.ShowUpdateDataNotification(true, txtUsername.Text, "نام کاربری");
                    editGrid.IsEnabled = false;
                    getUser();
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
            //if (dataGrid.ItemsSource != null)
            //{
            //    if (txtEditSearch.Text != string.Empty)
            //        dataGrid.ItemsSource = _initialCollection.Where(x => x.Username.Contains(txtEditSearch.Text)).Select(x => x);
            //    else
            //        dataGrid.ItemsSource = _initialCollection.Select(x => x);
            //}
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddUsername.Text == string.Empty || txtAddPassword.Password == string.Empty || txtAddPasswordAg.Password == string.Empty)
            {
                MainWindow.main.ShowFillAllDataNotification();
            }
            else if (txtAddPassword.Password != txtAddPasswordAg.Password)
            {
                MainWindow.main.ShowSamePasswordNotification();
            }
            else
            {
                try
                {
                    addUser(txtAddUsername.Text.ToLower(), txtAddPassword.Password.ToLower());
                    MainWindow.main.ShowAddDataNotification(true, txtAddUsername.Text, "نام کاربری");
                    txtAddUsername.Text = string.Empty;
                    txtAddPassword.Password = string.Empty;
                    txtAddPasswordAg.Password = string.Empty;
                    txtAddPassword.Focus();
                }
                catch (Exception)
                {
                    MainWindow.main.ShowAddDataNotification(false, txtAddUsername.Text, "نام کاربری");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ShowDeleteConfirmNotification(txtUsername.Text, "کاربر");
        }

        public void deleteUser()
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                deleteUser(id);
                MainWindow.main.ShowDeletedNotification(true, txtUsername.Text, "نام کاربری");
                editGrid.IsEnabled = false;
                getUser();
            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, txtUsername.Text, "نام کاربری");
            }
        }
    }
}