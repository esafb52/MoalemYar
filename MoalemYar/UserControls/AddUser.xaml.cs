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
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddUser.xaml
    /// </summary>
    public partial class AddUser : UserControl
    {
        public Brush BorderColor { get; set; }
        internal static AddUser main;
        private int runOnce = 0;
        private List<DataClass.Tables.User> _initialCollection;

        public AddUser()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;
            BorderColor = AppVariable.GetBrush(MainWindow.main.BorderBrush.ToString());
        }

        #region "Async Query"

        public async static Task<List<DataClass.Tables.User>> GetAllUsersAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Users.Select(x => x);
                return await query.ToListAsync();
            }
        }

        public static async Task<string> DeleteUserAsync(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteUser = await db.Users.FindAsync(id);
                db.Users.Remove(DeleteUser);
                await db.SaveChangesAsync();
                return "User Deleted Successfully";
            }
        }

        public async static Task<string> UpdateUserAsync(long id, string Username, string Password)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditUser = await db.Users.FindAsync(id);
                EditUser.Username = Username;
                EditUser.Password = Password;

                await db.SaveChangesAsync();
                return "User Updated Successfully";
            }
        }

        public async static Task<string> InsertUserAsync(string Username, string Password)
        {
            using (var db = new DataClass.myDbContext())
            {
                var User = new DataClass.Tables.User();
                User.Username = Username;
                User.Password = Password;

                db.Users.Add(User);

                await db.SaveChangesAsync();

                return "User Added Successfully";
            }
        }

        #endregion "Async Query"

        #region Func get Query Wait"

        private void getUser()
        {
            try
            {
                var query = GetAllUsersAsync();
                query.Wait();

                List<DataClass.Tables.User> data = query.Result;
                _initialCollection = query.Result;
                if (data.Any())
                {
                    dataGrid.ItemsSource = data;
                }
                else
                {
                    dataGrid.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("User");
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteUser(long id)
        {
            var query = DeleteUserAsync(id);
            query.Wait();
            MainWindow.main.getexHint();
        }

        private void updateUser(long id, string Username, string Password)
        {
            var query = UpdateUserAsync(id, Username, Password);
            query.Wait();
        }

        private void addUser(string Username, string Password)
        {
            var query = InsertUserAsync(Username, Password);
            query.Wait();
            MainWindow.main.getexHint();
        }

        #endregion Func get Query Wait"

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
            if (dataGrid.ItemsSource != null)
            {
                if (txtEditSearch.Text != string.Empty)
                    dataGrid.ItemsSource = _initialCollection.Where(x => x.Username.Contains(txtEditSearch.Text)).Select(x => x);
                else
                    dataGrid.ItemsSource = _initialCollection.Select(x => x);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddUsername.Text == string.Empty || txtAddPassword.Text == string.Empty || txtAddPasswordAg.Text == string.Empty)
            {
                MainWindow.main.ShowFillAllDataNotification();
            }
            else if (txtAddPassword.Text != txtAddPasswordAg.Text)
            {
                MainWindow.main.ShowSamePasswordNotification();
            }
            else
            {
                try
                {
                    addUser(txtAddUsername.Text.ToLower(), txtAddPassword.Text.ToLower());
                    MainWindow.main.ShowAddDataNotification(true, txtAddUsername.Text, "نام کاربری");
                    txtAddUsername.Text = string.Empty;
                    txtAddPassword.Text = string.Empty;
                    txtAddPasswordAg.Text = string.Empty;
                    txtAddPassword.Focus();
                }
                catch (Exception)
                {
                    MainWindow.main.ShowAddDataNotification(false, txtAddUsername.Text, "نام کاربری");
                }
            }
        }

        private void txtEditSearch_ButtonClick(object sender, EventArgs e)
        {
            getUser();
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