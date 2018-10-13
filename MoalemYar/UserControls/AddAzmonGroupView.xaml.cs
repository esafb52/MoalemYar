/****************************** ghost1372.github.io ******************************\
*	Module Name:	AddAzmonGroup.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 06:32 ب.ظ
*
***********************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddAzmonGroup.xaml
    /// </summary>
    public partial class AddAzmonGroupView : UserControl
    {
        internal static AddAzmonGroupView main;
        private List<DataClass.Tables.Group> _initialCollection;

        public AddAzmonGroupView()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;

            getGroup();
        }

        #region "Query"

        public static async Task<string> DeleteGroupAsync(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteGroup = await db.Groups.FindAsync(id);
                db.Groups.Remove(DeleteGroup);

                var DeleteQuestion = await db.AQuestions.FindAsync(id);
                db.AQuestions.Remove(DeleteQuestion);

                await db.SaveChangesAsync();
                return "Group Deleted Successfully";
            }
        }

        #endregion "Query"

        #region Query"

        private void getGroup()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Groups.ToList();
                    _initialCollection = query;
                    if (query.Any())
                    {
                        dataGrid.ItemsSource = query;
                    }
                    else
                    {
                        dataGrid.ItemsSource = null;
                        MainWindow.main.showNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "Group");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteGroup(long id)
        {
            var query = DeleteGroupAsync(id);
            query.Wait();
        }

        private void updateGroup(long id, string GroupName)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditGroup = db.Groups.Find(id);
                EditGroup.GroupName = GroupName;
                db.SaveChanges();
            }
        }

        private void addGroup(string GroupName)
        {
            using (var db = new DataClass.myDbContext())
            {
                var group = new DataClass.Tables.Group();
                group.GroupName = GroupName;
                db.Groups.Add(group);
                db.SaveChanges();
            }
        }

        #endregion Query"

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ClearScreen();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                txtGroup.Text = selectedItem.GroupName;
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
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                updateGroup(id, txtGroup.Text);
                MainWindow.main.showNotification(AppVariable.Update_Data_KEY, true, txtGroup.Text, "گروه");
                editGrid.IsEnabled = false;
                getGroup();
            }
            catch (Exception)
            {
                MainWindow.main.showNotification(AppVariable.Update_Data_KEY, false, txtGroup.Text, "گروه");
            }
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            txtGroup.Text = string.Empty;
            editGrid.IsEnabled = false;
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGrid.ItemsSource != null)
            {
                if (txtEditSearch.Text != string.Empty)
                    dataGrid.ItemsSource = _initialCollection.Where(x => x.GroupName.Contains(txtEditSearch.Text)).Select(x => x);
                else
                    dataGrid.ItemsSource = _initialCollection.Select(x => x);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtAddGroup.Text == string.Empty)
            {
                MainWindow.main.showNotification(NotificationKEY: AppVariable.Fill_All_Data_KEY);
            }
            else
            {
                try
                {
                    addGroup(txtAddGroup.Text);
                    MainWindow.main.showNotification(AppVariable.Add_Data_KEY, true, txtAddGroup.Text, "گروه");
                    txtAddGroup.Text = string.Empty;
                    txtAddGroup.Focus();
                }
                catch (Exception)
                {
                    MainWindow.main.showNotification(AppVariable.Add_Data_KEY, false, txtAddGroup.Text, "گروه");
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.showNotification(NotificationKEY: AppVariable.Delete_Confirm_KEY, param: new[] { txtGroup.Text, "گروه" });
        }

        public void deleteGroup()
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                deleteGroup(id);
                MainWindow.main.showNotification(AppVariable.Deleted_KEY, true, txtGroup.Text, "گروه");
                editGrid.IsEnabled = false;
                getGroup();
            }
            catch (Exception)
            {
                MainWindow.main.showNotification(AppVariable.Deleted_KEY, false, txtGroup.Text, "گروه");
            }
        }
    }
}