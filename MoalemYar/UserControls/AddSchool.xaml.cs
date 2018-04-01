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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddSchool.xaml
    /// </summary>
    public partial class AddSchool : UserControl
    {

        public ICommand EditCommand { get; set; }
        public ICommand DeleteCommand { get; set; }


        private FrameworkElement Window { get; set; }
        public Brush BorderColor { get; set; }

        int selectedItemIndex = 0;
        public AddSchool()
        {
            InitializeComponent();
            this.DataContext = this;
           
            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderColor = brush;


            Window = this;
            EditCommand = new RoutedCommand();
            DeleteCommand = new RoutedCommand();

            CommandManager.RegisterClassCommandBinding(Window.GetType(), new CommandBinding(EditCommand, EditCommand_Click));
            CommandManager.RegisterClassCommandBinding(Window.GetType(), new CommandBinding(DeleteCommand, DeleteCommand_Click));
        }



        protected void EditCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {            
            var selectedItems = lvDataBinding.SelectedItems;
            foreach (var selectedItem in selectedItems)
            {
                ListBoxItem myListBoxItem = (ListBoxItem)(lvDataBinding.ItemContainerGenerator.ContainerFromItem(selectedItem));
                ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);
                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                TextBlock myTitleText = (TextBlock)myDataTemplate.FindName("txtTitle", myContentPresenter);
                TextBlock myContentText = (TextBlock)myDataTemplate.FindName("txtContent", myContentPresenter);
                TextBlock myAdminText = (TextBlock)myDataTemplate.FindName("txtAdmin", myContentPresenter);
                TextBlock myBaseText = (TextBlock)myDataTemplate.FindName("txtBase", myContentPresenter);

                txtYear.Text = myContentText.Text;
                txtSchool.Text = myTitleText.Text;
                txtAdmin.Text = myAdminText.Text;
                var element = FindElementByName<ComboBox>(cmbContent, "cmbBase");
                element.SelectedIndex = Convert.ToInt32(myBaseText.Text);
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

        protected void DeleteCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            //Do Somethings

        }
        private void InitalizeData()
        {
            var query = GetAllSchoolsAsync();
            query.Wait();

            List<DataClass.Tables.School> data = query.Result;
            lvDataBinding.ItemsSource = data;

        }

        public async static Task<List<DataClass.Tables.School>> GetAllSchoolsAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                //for (int i = 0; i < 5; i++)
                //{
                //    var asd = new DataClass.Tables.School
                //    {
                //        Admin = "Mahdi" + i,
                //        Base = i,
                //        Name = "Name" + i,
                //        Year = "98-97" + i
                //    };
                //    db.Schools.Add(asd);
                //   await db.SaveChangesAsync();
                //}
                var data = db.Schools.Select(x => x);
                return await data.ToListAsync();
            }
        }

        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            lbi.IsSelected = true;
            selectedItemIndex = lvDataBinding.SelectedIndex;
        }
        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            lvDataBinding.SelectedItems.Clear();
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
            var query = GetAllSchoolsAsync();
            query.Wait();

            List<DataClass.Tables.School> data = query.Result;
            lvDataBinding.ItemsSource = data;
        }
    }
}