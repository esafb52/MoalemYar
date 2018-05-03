
/****************************** ghost1372.github.io ******************************\
*	Module Name:	QuestionsList.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 4, 28, 08:38 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
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
    /// Interaction logic for QuestionsList.xaml
    /// </summary>
    public partial class QuestionsList : UserControl
    {
        ObservableCollection<string> list = new ObservableCollection<string>();
        private bool runOnceSchool = true;
        internal static QuestionsList main;
        private PersianCalendar pc = new PersianCalendar();
        private static string strDate;
        public System.Windows.Media.Brush BorderColor { get; set; }
        public QuestionsList()
        {
            InitializeComponent();

            this.DataContext = this;
            main = this;
            strDate = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");
            txtDate.Text = string.Format("تاریخ امروز : {0} ", strDate);

            var color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderColor = brush;
        }

        #region "Async Query"

        public async static Task<List<DataClass.Tables.School>> GetAllSchoolsAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Select(x => x);
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsAsync(long SchoolId, string Book)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.Where(x => !db.Questions.Any(f => f.StudentId == x.Id && f.Book == Book) && x.BaseId == SchoolId).Select(x => new DataClass.DataTransferObjects.StudentsDto { Id = x.Id, BaseId = x.BaseId, Name = x.Name, LName = x.LName, FName = x.FName });
                
                return await query.ToListAsync();
            }
        }

        #endregion "Async Query"

        #region Func get Query Wait"

        private void getSchool()
        {
            try
            {
                var query = GetAllSchoolsAsync();
                query.Wait();
                List<DataClass.Tables.School> data = query.Result;
                if (data.Any())
                {
                    cmbBase.ItemsSource = data;
                }
            }
            catch (Exception)
            {
            }
        }

        private void getStudents(long SchoolId, string Book)
        {
            var query = GetAllStudentsAsync(SchoolId, Book);
            query.Wait();
            List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;
            if (data.Any())
            {
                dataGrid.ItemsSource = data;
            }
            else
            {
                MainWindow.main.ShowNoDataNotification("Question");
            }
        }

        #endregion Func get Query Wait"
        private void tabc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (runOnceSchool)
            {
                getSchool();
                runOnceSchool = false;
            }
        }

        private void cmbBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = FindElementByName<ComboBox>(cmbAddContentBook, "cmbBook");
            dynamic selectedItem = cmbBase.SelectedItem;

            if (selectedItem.Base.Contains("اول")) {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");

            } else if (selectedItem.Base.Contains("دوم")) {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");
                list.Add("هدیه های آسمانی");

            } else if (selectedItem.Base.Contains("سوم") || selectedItem.Base.Contains("چهارم") || selectedItem.Base.Contains("پنجم")) {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");
                list.Add("هدیه های آسمانی");
                list.Add("مطالعات اجتماعی");

            } else if (selectedItem.Base.Contains("ششم")) {
                list.Clear();
                list.Add("قرآن");
                list.Add("فارسی");
                list.Add("علوم");
                list.Add("ریاضی");
                list.Add("هدیه های آسمانی");
                list.Add("مطالعات اجتماعی");
                list.Add("کار و فناوری");
                list.Add("تفکر");
            }

            element.ItemsSource = list;


        }



        private void cmbBook_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var element = FindElementByName<ComboBox>(cmbAddContentBook, "cmbBook");
            getStudents(Convert.ToInt64(cmbBase.SelectedValue), element.SelectedItem.ToString());

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

    }
}
