
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
using System.Data.Entity;
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
    /// Interaction logic for AddAzmonGroup.xaml
    /// </summary>
    public partial class AddAzmonGroup : UserControl
    {
        public Brush BorderColor { get; set; }
        internal static AddAzmonGroup main;
        private int runOnce = 0;
        private List<DataClass.Tables.Group> _initialCollection;
        public AddAzmonGroup()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;
            BorderColor = AppVariable.GetBrush(MainWindow.main.BorderBrush.ToString());
        }

        #region "Async Query"

        public async static Task<List<DataClass.Tables.Group>> GetAllGroupAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Groups.Select(x => x);
                return await query.ToListAsync();
            }
        }

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

        public async static Task<string> UpdateGroupAsync(long id, string GroupName)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditGroup = await db.Groups.FindAsync(id);
                EditGroup.GroupName = GroupName;
                await db.SaveChangesAsync();
                return "Group Updated Successfully";
            }
        }

        public async static Task<string> InsertGroupAsync(string GroupName)
        {
            using (var db = new DataClass.myDbContext())
            {
                var group = new DataClass.Tables.Group();
                group.GroupName = GroupName;
                db.Groups.Add(group);

                await db.SaveChangesAsync();

                return "Group Added Successfully";
            }
        }
        #endregion "Async Query"

        #region Func get Query Wait"

        private void getGroup()
        {
            try
            {
                var query = GetAllGroupAsync();
                query.Wait();

                List<DataClass.Tables.Group> data = query.Result;
                _initialCollection = query.Result;
                if (data.Any())
                {
                    dataGrid.ItemsSource = data;
                }
                else
                {
                    dataGrid.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("Group");
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
            MainWindow.main.getexHint();
        }

        private void updateGroup(long id, string GroupName)
        {
            var query = UpdateGroupAsync(id, GroupName);
            query.Wait();
        }

        private void addGroup(string GroupName)
        {
            var query = InsertGroupAsync(GroupName);
            query.Wait();
            MainWindow.main.getexHint();
        }

        #endregion Func get Query Wait"

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Azmon.main.exContent.Content = null;
        }

        private void tabc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabc.SelectedIndex == 1)
            {
                if (runOnce == 0)
                {
                    getGroup();
                    runOnce = 1;
                }
            }
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
                MainWindow.main.ShowUpdateDataNotification(true, txtGroup.Text, "گروه");
                editGrid.IsEnabled = false;
                getGroup();
            }
            catch (Exception)
            {
                MainWindow.main.ShowUpdateDataNotification(false, txtGroup.Text, "گروه");
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
                MainWindow.main.ShowFillAllDataNotification();
            }
            else
            {
                try
                {
                    addGroup(txtAddGroup.Text);
                    MainWindow.main.ShowAddDataNotification(true, txtAddGroup.Text, "گروه");
                    txtAddGroup.Text = string.Empty;
                    txtAddGroup.Focus();
                    Azmon.main.getHint();

                }
                catch (Exception)
                {
                    MainWindow.main.ShowAddDataNotification(false, txtAddGroup.Text, "گروه");
                }
            }
        }
        private void txtEditSearch_ButtonClick(object sender, EventArgs e)
        {
            getGroup();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ShowDeleteConfirmNotification(txtGroup.Text, "گروه");
        }

        public void deleteGroup()
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                deleteGroup(id);
                MainWindow.main.ShowDeletedNotification(true, txtGroup.Text, "گروه");
                editGrid.IsEnabled = false;
                getGroup();
                Azmon.main.getHint();

            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, txtGroup.Text, "گروه");
            }
        }
    }
}
