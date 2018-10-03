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
using System.Data.Entity;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ThumbnailSharp;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for AddStudent.xaml
    /// </summary>
    public partial class AddStudentView : UserControl
    {
        private bool isCreateThumbnail = false;
        internal static AddStudentView main;
        private List<DataClass.DataTransferObjects.SchoolsStudentsJointDto> _initialCollection;
        uint thumbSize = 450;
        public AddStudentView()
        {
            InitializeComponent();
            main = this;
            getSchool();
        }

        #region "Async Query"

        public static async Task<string> DeleteStudentAsync(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var DeleteStudent = await db.Students.FindAsync(id);
                db.Students.Remove(DeleteStudent);

                var DeleteScore = await db.Scores.Where(x => x.StudentId == id).ToListAsync();
                db.Scores.RemoveRange(DeleteScore);

                var DeleteQuestion = await db.Questions.Where(x => x.StudentId == id).ToListAsync();
                db.Questions.RemoveRange(DeleteQuestion);

                var DeleteAttendance = await db.Attendances.Where(x => x.StudentId == id).ToListAsync();
                db.Attendances.RemoveRange(DeleteAttendance);

                await db.SaveChangesAsync();
                return "Student Deleted Successfully";
            }
        }

        #endregion "Async Query"

        #region Func get Query Wait"

        private void getSchool()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.ToList();
                    if (query.Any())
                    {
                        cmbBase.ItemsSource = query;
                        cmbEditBase.ItemsSource = query;
                        cmbBaseEdit.ItemsSource = query;
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void getStudent(long BaseId)
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.Join(
                      db.Students,
                      c => c.Id,
                      v => v.BaseId,
                      (c, v) => new DataClass.DataTransferObjects.SchoolsStudentsJointDto { Name = v.Name, LName = v.LName, FName = v.FName, Gender = v.Gender, BaseId = v.BaseId, Image = v.Image, Id = v.Id, Base = c.Base }
                  ).OrderBy(x => x.LName).Where(x => x.BaseId == BaseId);

                    _initialCollection = query.ToList();

                    if (query.Any())
                    {
                        dataGrid.ItemsSource = query.ToList();
                    }
                    else
                    {
                        dataGrid.ItemsSource = null;
                        MainWindow.main.ShowNoDataNotification("Student");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void deleteStudent(long id)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = DeleteStudentAsync(id);
                query.Wait();
                MainWindow.main.ShowDeletedNotification(true, txtName.Text, "دانش آموز");
            }
        }

        private void updateStudent(long id, long BaseId, string Name, string LName, string FName, string Gender, byte[] Image)
        {
            using (var db = new DataClass.myDbContext())
            {
                var EditStudent = db.Students.Find(id);
                EditStudent.Name = Name;

                EditStudent.LName = LName;

                EditStudent.FName = FName;
                EditStudent.Gender = Gender;
                EditStudent.BaseId = BaseId;
                EditStudent.Image = Image;
                db.SaveChanges();
            }
        }

        private void addStudent(long BaseId, string Name, string LName, string FName, string Gender, byte[] Image)
        {
            using (var db = new DataClass.myDbContext())
            {
                var Student = new DataClass.Tables.Student();
                Student.BaseId = BaseId;
                Student.Name = Name;
                Student.LName = LName;
                Student.FName = FName;
                Student.Gender = Gender;
                Student.Image = Image;
                db.Students.Add(Student);

                db.SaveChanges();
            }
        }

        #endregion Func get Query Wait"

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ClearScreen();
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                editStack.IsEnabled = true;
                dynamic selectedItem = dataGrid.SelectedItems[0];
                txtName.Text = selectedItem.Name;
                txtLName.Text = selectedItem.LName;
                txtFName.Text = selectedItem.FName;
                setComboValue(selectedItem.Gender);
                cmbEditBase.SelectedValue = selectedItem.BaseId;

                byte[] bytes = selectedItem.Image as byte[];
                MemoryStream stream = new MemoryStream(bytes);
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.StreamSource = stream;
                image.EndInit();
                imgEditStudent.Source = image;
            }
            catch (Exception)
            {
            }
        }

        private void btnEditSave_Click(object sender, RoutedEventArgs e)
        {
            dynamic selectedItem = dataGrid.SelectedItems[0];
            long id = selectedItem.Id;

            updateStudent(id, Convert.ToInt64(cmbEditBase.SelectedValue), txtName.Text, txtLName.Text, txtFName.Text, getComboValue(), (isCreateThumbnail ? CreateThumbnail(imgEditStudent.Source as BitmapImage) : getImageByte(imgEditStudent.Source as BitmapImage)));
            isCreateThumbnail = false;

            MainWindow.main.ShowUpdateDataNotification(true, txtName.Text, "دانش آموز");
            editStack.IsEnabled = false;
            getStudent(Convert.ToInt64(cmbBaseEdit.SelectedValue));
        }

        private void btnEditCancel_Click(object sender, RoutedEventArgs e)
        {
            txtName.Text = string.Empty;
            txtLName.Text = string.Empty;
            txtFName.Text = string.Empty;
            setComboValue(null);
            cmbEditBase.SelectedIndex = -1;
            editStack.IsEnabled = false;
        }

        private string getComboValue()
        {
            var element = FindElement.FindElementByName<ComboBox>(cmbContentGender, "cmbGender");
            return element.Text;
        }

        private void setComboValue(string index)
        {
            var element = FindElement.FindElementByName<ComboBox>(cmbContentGender, "cmbGender");
            switch (index)
            {
                case "پسر":
                    element.SelectedIndex = 0;
                    break;

                case "دختر":
                    element.SelectedIndex = 1;
                    break;

                case null:
                    element.SelectedIndex = -1;
                    break;
            }
        }

        private void txtEditSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGrid.ItemsSource != null)
            {
                if (txtEditSearch.Text != string.Empty)
                    dataGrid.ItemsSource = _initialCollection.Where(x => x.Name.Contains(txtEditSearch.Text) || x.LName.Contains(txtEditSearch.Text) || x.FName.Contains(txtEditSearch.Text) || x.Gender.Contains(txtEditSearch.Text)).Select(x => x);
                else
                    dataGrid.ItemsSource = _initialCollection.Select(x => x);
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var elementG = FindElement.FindElementByName<ComboBox>(cmbAddContentGender, "cmbGender");

            if (txtAddName.Text == string.Empty || txtAddLName.Text == string.Empty || txtAddFName.Text == string.Empty || elementG.SelectedIndex == -1 || cmbBase.SelectedIndex == -1)
            {
                MainWindow.main.ShowFillAllDataNotification();
            }
            else
            {
                try
                {
                    addStudent(Convert.ToInt64(cmbBase.SelectedValue), txtAddName.Text, txtAddLName.Text, txtAddFName.Text, elementG.Text, CreateThumbnail(imgStudent.Source as BitmapImage));
                    MainWindow.main.ShowAddDataNotification(true, txtAddName.Text, "دانش آموز");
                    txtAddName.Text = string.Empty;
                    txtAddLName.Text = string.Empty;
                    txtAddFName.Text = string.Empty;
                    txtAddName.Focus();
                }
                catch (Exception)
                {
                    MainWindow.main.ShowAddDataNotification(false, txtAddName.Text, "دانش آموز");
                }
            }
        }

        public byte[] CreateThumbnail(BitmapImage imageC)
        {
            //Read Image byte

            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);

            //Create Thumbnail from image

            byte[] resultBytes = new ThumbnailCreator().CreateThumbnailBytes(
                thumbnailSize: thumbSize,
                    imageBytes: memStream.ToArray(),
                    imageFormat: Format.Jpeg
            );
            return resultBytes;
        }

        public byte[] getImageByte(BitmapImage imageC)
        {
            MemoryStream memStream = new MemoryStream();
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageC));
            encoder.Save(memStream);
            return memStream.ToArray();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.main.ShowDeleteConfirmNotification(txtName.Text, "دانش آموز");
        }

        public void deleteStudent()
        {
            try
            {
                dynamic selectedItem = dataGrid.SelectedItems[0];
                long id = selectedItem.Id;
                deleteStudent(id);
                editStack.IsEnabled = false;
                getStudent(Convert.ToInt64(cmbBaseEdit.SelectedValue));
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
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(dialog.FileName);
                if (img.Height > 450)
                {
                    thumbSize = 450;
                }
                else
                {
                    uint mines = Convert.ToUInt32(450 - img.Height);
                    uint fixedValue = 450 - mines - 10;
                    thumbSize = fixedValue;
                }
                imgStudent.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }

        private void btnEditChoose_Click(object sender, RoutedEventArgs e)
        {
            VistaOpenFileDialog dialog = new VistaOpenFileDialog();
            var imageExtensions = string.Join(";", ImageCodecInfo.GetImageDecoders().Select(ici => ici.FilenameExtension));
            dialog.Filter = string.Format("تصاویر|{0}|تمام فایل ها|*.*", imageExtensions);
            if ((bool)dialog.ShowDialog())
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(dialog.FileName);
                if (img.Height > 450)
                {
                    thumbSize = 450;
                }
                else
                {
                    uint mines = Convert.ToUInt32(450 - img.Height);
                    uint fixedValue = 450 - mines - 10;
                    thumbSize = fixedValue;
                }
                
                imgEditStudent.Source = new BitmapImage(new Uri(dialog.FileName));
                isCreateThumbnail = true;
            }
        }

        private void cmbBaseEdit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbBaseEdit.SelectedValue));
        }

        private void cmbGender_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            imgStudent.Source = new BitmapImage(new Uri("pack://application:,,,/MoalemYar;component/Resources/Choose.png", UriKind.Absolute));
        }
    }
}