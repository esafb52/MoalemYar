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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddSchool.xaml
    /// </summary>
    public partial class AddSchool : UserControl
    {

        public ICommand cmdShow { get; set; }
        public ICommand cmddelete { get; set; }


        private FrameworkElement Window { get; set; }

        int index = 0;
        public AddSchool()
        {
            InitializeComponent();
           
            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderColor = brush;

            InitalizeData();


            this.DataContext = this;

            Window = this;

            cmdShow = new RoutedCommand();
            cmddelete = new RoutedCommand();

            CommandManager.RegisterClassCommandBinding(Window.GetType(), new CommandBinding(cmdShow, cmdShow_Click));
            CommandManager.RegisterClassCommandBinding(Window.GetType(), new CommandBinding(cmddelete, cmddelete_Click));
        }

        public Brush BorderColor { get; set; }


        protected void cmdShow_Click(object sender, ExecutedRoutedEventArgs e)
        {
            //MyData selectedData = e.Parameter as MyData;
            //(this.Window as Window).Background = selectedData.MyColor;
            foreach (var item in lvDataBinding.Items)
            {
                Console.WriteLine(item);
            }
            MessageBox.Show(lvDataBinding.SelectedIndex + "");

        }
        protected void cmddelete_Click(object sender, ExecutedRoutedEventArgs e)
        {
            //MyData selectedData = e.Parameter as MyData;
            //(this.Window as Window).Background = selectedData.MyColor;

            MessageBox.Show("delete");
        }
        private void InitalizeData()
        {
            ObservableCollection<Patient> data = new ObservableCollection<Patient>();
            for (int i = 0; i < 3; i++)
            {
                data.Add(new Patient
                {
                    Name = "97-98",
                    Age = "42周岁",
                    Sex = i % 2 == 0 ? "男" : "女",
                    BedNo = i.ToString(),
                    Address = "中国 苏州工业园区",
                    BirthDay = "1955年2月6日",
                    City = "苏州",
                    HomePhoneNumber = "0512-62810609",
                    PostCode = "2695600"
                });
            }
            this.lvDataBinding.ItemsSource = data;
        }
        public class Patient
        {
            public string BedNo { get; set; }
            public string Name { get; set; }
            public string Age { get; set; }
            public string Sex { get; set; }

            public string BirthDay { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public string HomePhoneNumber { get; set; }

        }



        private void ListBoxItem_MouseEnter(object sender, MouseEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            lbi.IsSelected = true;
            index = lvDataBinding.SelectedIndex;

        }
        private void ListBoxItem_MouseLeave(object sender, MouseEventArgs e)
        {
            lvDataBinding.SelectedItems.Clear();
        }

        private void lvDataBinding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


            ListBoxItem myListBoxItem =
    (ListBoxItem)(lvDataBinding.ItemContainerGenerator.ContainerFromItem(lvDataBinding.Items.CurrentItem));

            // Getting the ContentPresenter of myListBoxItem
            ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

            // Finding textBlock from the DataTemplate that is set on that ContentPresenter
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            TextBlock myTextBlock = (TextBlock)myDataTemplate.FindName("txtPhon", myContentPresenter);

            // Do something to the DataTemplate-generated TextBlock
            Console.WriteLine("The text of the TextBlock of the selected list item: "
                + myTextBlock.Text);

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
            MainWindow.main.RestartNotification();
        }

      

       
        
        
    }
}