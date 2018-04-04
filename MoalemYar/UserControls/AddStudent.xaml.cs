
/****************************** ghost1372.github.io ******************************\
*	Module Name:	AddStudent.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 3, 06:49 ب.ظ
*	
***********************************************************************************/
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class AddStudent : UserControl
    {
        private System.ComponentModel.BackgroundWorker MyWorker = new System.ComponentModel.BackgroundWorker();
        public Brush BorderColor { get; set; }
        private int runOnce = 0;
        internal static AddStudent main;
        public AddStudent()
        {
            InitializeComponent();
            this.DataContext = this;
            main = this;
            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderColor = brush;
            MyWorker.WorkerSupportsCancellation = true;
            MyWorker.DoWork += MyWorker_DoWork;
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
            else
            {
                if (runOnce == 0)
                {
                    using (var db = new DataClass.myDbContext())
                    {
                        var dataBase = from x in db.Schools select new { x.Id, x.SchoolName, x.Base, x.Year };
                        cmbBase.ItemsSource = dataBase.ToList();
                    }
                }
            }
        }
        private void MyWorker_DoWork(object Sender, System.ComponentModel.DoWorkEventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                using (var db = new DataClass.myDbContext())
                {
                    var data = from x in db.Students select new { x.Name, x.LName, x.FName, x.Gender, x.Image, x.BaseId, x.Id };
                    if (data.Any())
                        dgv.ItemsSource = data.ToList();
                    else
                        MainWindow.main.ShowNoDataNotification("Student");
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
                txtName.Text = selectedItem.Name;
                txtLName.Text = selectedItem.LName;
                txtFName.Text = selectedItem.FName;
                setComboValue(selectedItem.Gender,true);
                //setComboValue(selectedItem.BaseId, false);

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
                    var data = db.Students.Where(s => s.Id == id).FirstOrDefault<DataClass.Tables.Student>();
                    data.Name = txtName.Text;
                    data.LName = txtLName.Text;
                    data.FName = txtFName.Text;
                    data.Gender = getComboValue(true);
                    //data.BaseId = getComboValue(false);

                    db.SaveChanges();
                    MainWindow.main.ShowUpdateDataNotification(true, txtName.Text, "دانش آموز");
                    editGrid.IsEnabled = false;
                    if (!MyWorker.IsBusy)
                        MyWorker.RunWorkerAsync();
                }
            }
            catch (Exception)
            {
                MainWindow.main.ShowUpdateDataNotification(false, txtName.Text, "دانش آموز");
            }
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = string.Empty;
            txtLName.Text = string.Empty;
            txtFName.Text = string.Empty;
            setComboValue(null,true);
            setComboValue(null, false);
            editGrid.IsEnabled = false;
        }
        private string getComboValue(bool isGender)
        {
            if (isGender)
            {
                var element = FindElementByName<ComboBox>(cmbContentGender, "cmbGender");
                return element.Text;
            }
            else
            {
                var element = FindElementByName<ComboBox>(cmbContentBase, "cmbBase");
                return element.Text;
            }
        }

        private void setComboValue(string index, bool isGender)
        {
            if (isGender)
            {
                var element = FindElementByName<ComboBox>(cmbContentGender, "cmbGender");
                switch (index)
                {
                    case "پسر":
                        element.SelectedIndex = 0;
                        break;

                    case "دختر":
                        element.SelectedIndex = 1;
                        break;
                }
            }
            else
            {
                var element = FindElementByName<ComboBox>(cmbContentBase, "cmbBase");
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
            
        }
        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEditSearch.Text != string.Empty)
            {
                //using (var db = new DataClass.myDbContext())
                //{
                   
                //}
            }
            else
            {
                if (!MyWorker.IsBusy)
                    MyWorker.RunWorkerAsync();
            }
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {

            var elementG = FindElementByName<ComboBox>(cmbAddContentGender, "cmbGender");

            if (txtAddName.Text == string.Empty || txtAddLName.Text == string.Empty || txtAddFName.Text == string.Empty || elementG.SelectedIndex == -1 || cmbBase.SelectedIndex == -1)
            {
                MainWindow.main.ShowFillAllDataNotification();
            }
            else
            {
                try
                {
                    using (var db = new DataClass.myDbContext())
                    {
                        var data = new DataClass.Tables.Student()
                        {
                            Name = txtAddName.Text,
                            LName = txtAddLName.Text,
                            FName = txtAddFName.Text,
                            Gender = elementG.Text,
                            BaseId = Convert.ToInt64(cmbBase.SelectedValue),
                            Image= ReadImageFile((imgStudent.Source as BitmapImage).UriSource.AbsolutePath)
                        };
                        db.Students.Add(data);
                        db.SaveChanges();
                        MainWindow.main.ShowAddDataNotification(true, txtAddName.Text, "دانش آموز");
                        txtAddName.Text = string.Empty;
                        txtAddLName.Text = string.Empty;
                        txtAddFName.Text = string.Empty;
                        txtAddName.Focus();
                    }
                }
                catch (Exception)
                {
                    MainWindow.main.ShowAddDataNotification(true, txtAddName.Text, "دانش آموز");
                }
            }
        }
        public byte[] ReadImageFile(string imageLocation)
        {
            byte[] imageData = null;
            FileInfo fileInfo = new FileInfo(imageLocation);
            long imageFileLength = fileInfo.Length;
            FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            imageData = br.ReadBytes((int)imageFileLength);
            return imageData;
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
                    var data = db.Students.Where(s => s.Id == id).FirstOrDefault<DataClass.Tables.Student>();
                    db.Students.Remove(data);
                    db.SaveChanges();
                    MainWindow.main.ShowDeletedNotification(true, txtName.Text, "دانش آموز");
                    editGrid.IsEnabled = false;
                    if (!MyWorker.IsBusy)
                        MyWorker.RunWorkerAsync();
                }
            }
            catch (Exception)
            {
                MainWindow.main.ShowDeletedNotification(false, txtName.Text, "دانش آموز");
            }
        }

        private void btnChoose_Click(object sender, RoutedEventArgs e)
        {
           
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            var imageExtensions = string.Join(";", ImageCodecInfo.GetImageDecoders().Select(ici => ici.FilenameExtension));
            dialog.Filter = string.Format("تصاویر|{0}|تمام فایل ها|*.*", imageExtensions);
            if ((bool)dialog.ShowDialog())
                imgStudent.Source = new BitmapImage(new Uri(dialog.FileName));
        }

        private void cmbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = FindElementByName<ComboBox>(cmbAddContentGender, "cmbGender");
            if(element.SelectedIndex==0)
                imgStudent.Source = new BitmapImage(new Uri("pack://application:,,,/MoalemYar;component/Resources/Boy.png", UriKind.Absolute));
            else
                imgStudent.Source = new BitmapImage(new Uri("pack://application:,,,/MoalemYar;component/Resources/Girl.png", UriKind.Absolute));

        }

       
    }
}
