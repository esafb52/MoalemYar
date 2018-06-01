
/****************************** ghost1372.github.io ******************************\
*	Module Name:	StartAzmon.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 08:34 ب.ظ
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
using System.Windows.Threading;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for StartAzmon.xaml
    /// </summary>
    public partial class StartAzmon : UserControl
    {

        private int QN = 0;
        private int gozineh;
        private int sahih, nazade, qalat;

        private List<DataClass.Tables.AQuestion> result;
        private ListBox answerlist;
        private string uClass;
        private bool isGuid = true;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        
        public StartAzmon()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            getSchool();
        }

        #region Async Query
        public async static Task<List<DataClass.DataTransferObjects.StudentsDto>> GetAllStudentsAsync(long SchoolId)
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.Where(x => x.BaseId == SchoolId)
                    .Select(x => new DataClass.DataTransferObjects.StudentsDto { Id = x.Id, BaseId = x.BaseId, Name = x.Name, LName = x.LName, FName = x.FName });

                return await query.ToListAsync();
            }
        }
        public async static Task<List<DataClass.Tables.School>> GetAllSchoolsAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Select(x => x);
                return await query.ToListAsync();
            }
        }

        private void getSchool()
        {
            try
            {
                var query = GetAllSchoolsAsync();
                query.Wait();
                List<DataClass.Tables.School> data = query.Result;
                if (data.Any())
                {
                    cmbEditBase.ItemsSource = data;
                }
            }
            catch (Exception)
            {
            }
        }
        public async static Task<List<DataClass.Tables.Group>> GetAllGroupAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Groups.Select(x => x);
                return await query.ToListAsync();
            }
        }
        private void getStudent(long BaseId)
        {
            try
            {
                var query = GetAllStudentsAsync(BaseId);
                query.Wait();

                List<DataClass.DataTransferObjects.StudentsDto> data = query.Result;

                if (data.Any())
                {
                    cmbEditStudent.ItemsSource = data.ToList();
                }
                else
                {
                    cmbEditStudent.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("Student");
                }
            }
            catch (Exception)
            {
            }
        }
        private void getGroup()
        {
            try
            {
                var query = GetAllGroupAsync();
                query.Wait();

                List<DataClass.Tables.Group> data = query.Result;
                if (data.Any())
                {
                    cmbGroup.ItemsSource = data;
                }
                else
                {
                    cmbGroup.ItemsSource = null;
                    MainWindow.main.ShowNoDataNotification("Group");
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion

        private void txtTedad_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((e.Text) == null || !(e.Text).All(char.IsDigit))
            {
                e.Handled = true;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTedad.Text) || txtTedad.Text == "0" || txtTedad.Text == "تعداد سوالات" || cmbGroup.SelectedIndex == -1 || cmbEditStudent.SelectedIndex == -1)
            {
                MainWindow.main.ShowFillAllDataNotification();
                return;
            }else {
                    using (var db = new DataClass.myDbContext())
                    {
                        var id = Convert.ToInt64(cmbGroup.SelectedValue);
                        var classText = uClass;

                        if (isGuid)
                            result = db.AQuestions.Where(x => x.GroupId == id && x.Class == classText).OrderBy(x => Guid.NewGuid()).Take(Convert.ToInt32(txtTedad.Text)).ToList();
                        else
                            result = db.AQuestions.Where(x => x.GroupId == id && x.Class == classText).Take(Convert.ToInt32(txtTedad.Text)).ToList();

                        if (result.Count < Convert.ToInt32(txtTedad.Text))
                            MainWindow.main.ShowAzmonNotification();
                         else
                        {
                            QN = 0;

                            lblQNumber.Text = Convert.ToString(QN + 1);

                            lblQtext.Text = result[QN].QuestionText;
                            swCase1.Content = result[QN].Case1;
                        swCase2.Content = result[QN].Case2;
                        swCase3.Content = result[QN].Case3;
                        swCase4.Content = result[QN].Case4;
                            btnNext.IsEnabled = true;
                            btnPrev.IsEnabled = false;
                            btnStart.IsEnabled = false;
                            swLimit.IsEnabled = false;
                            txtTedad.IsEnabled = false;
                            cmbGroup.IsEnabled = false;
                            cmbEditBase.IsEnabled = false;
                            swRandom.IsEnabled = false;
                        gpAzmon.IsEnabled = true;
                            ClearCheck();
                            answerlist = new ListBox();

                            for (int i = 0; i < result.Count; i++)
                            {
                                answerlist.Items.Add("0");
                            }

                            gpControl.Visibility = Visibility.Visible;

                            if (swLimit.IsChecked == true)
                            {
                                lblTime.Text = Convert.ToString(Convert.ToInt32(txtTime.Text) * (Convert.ToInt32(txtTedad.Text)));
                            dispatcherTimer.Start();
                        }
                            else
                            {
                                lblTime.Text = "نامحدود";
                            }
                        }
                    }
            }
        }
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblTime.Text) > 1)
            {
                lblTime.Text = Convert.ToString(Convert.ToInt32(lblTime.Text) - 1);
            }
            else
            {
                btnFinish_Click(null, null);
            }
        }
        private void cmbEditBase_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            getStudent(Convert.ToInt64(cmbEditBase.SelectedValue));
        }

        private void cmbEditStudent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            try
            {

                getGroup();
                dynamic selectedItem = cmbEditBase.SelectedItem;
                uClass = selectedItem.Base;
            }
            catch (Exception)
            {
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();

            sahih = 0;
            nazade = 0;
            qalat = 0;

            if (swCase1.IsChecked == true)
            {
                gozineh = 1;
            }
            else if (swCase2.IsChecked == true)
            {
                gozineh = 2;
            }
            else if (swCase3.IsChecked == true)
            {
                gozineh = 3;
            }
            else if (swCase4.IsChecked == true)
            {
                gozineh = 4;
            }
            else
            {
                gozineh = 0;
            }

            answerlist.Items[QN] = gozineh.ToString();

            for (int i = 0; i < Convert.ToInt32(answerlist.Items.Count.ToString()); i++)
            {
                if (answerlist.Items[i].ToString() == result[i].Answer.ToString())
                {
                    sahih = sahih + 1;
                }
                else if (answerlist.Items[i].ToString() == "0")
                {
                    nazade = nazade + 1;
                }
                else
                {
                    qalat = qalat + 1;
                }
            }
            var uId = Convert.ToInt64(cmbEditStudent.SelectedValue);
            
            gpControl.Visibility = Visibility.Hidden;
            btnStart.IsEnabled = true;
            txtTedad.IsEnabled = true;
            swLimit.IsEnabled = true;
            cmbGroup.IsEnabled = true;
            cmbEditBase.IsEnabled = true;
            swRandom.IsEnabled = true;
            gpAzmon.IsEnabled = false;
            ClearCheck();
            AzmonResult._True = sahih;
            AzmonResult._False = qalat;
            AzmonResult._None = nazade;
            AzmonResult._UserId = uId;
            AzmonResult._GroupName = cmbGroup.Text;
            Azmon.main.exContent.Content = new AzmonResult();

        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            btnPrev.IsEnabled = true;
            if (swCase1.IsChecked == true)
                gozineh = 1;
            else if (swCase2.IsChecked == true)
                gozineh = 2;
            else if (swCase3.IsChecked == true)
                gozineh = 3;
            else if (swCase4.IsChecked == true)
                gozineh = 4;
            else
                gozineh = 0;
            answerlist.Items[QN] = gozineh.ToString();

            if (QN < result.Count - 1)
            {
                QN++;
                lblQtext.Text = result[QN].QuestionText;
                swCase1.Content = result[QN].Case1;
                swCase2.Content = result[QN].Case2;
                swCase3.Content = result[QN].Case3;
                swCase4.Content = result[QN].Case4;
                lblQNumber.Text = Convert.ToString(Convert.ToInt32(lblQNumber.Text) + 1);
            }

            if (QN == result.Count - 1)
            {
                btnNext.IsEnabled = false;
                btnFinish.IsEnabled = true;
            }

            if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 0)
            {
                ClearCheck();
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 1)
            {
                swCase1.IsChecked = true;
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 2)
            {
                swCase2.IsChecked = true;
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 3)
            {
                swCase3.IsChecked = true;
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 4)
            {
                swCase4.IsChecked = true;
            }
        }

        private void btnPrev_Click(object sender, RoutedEventArgs e)
        {
            btnNext.IsEnabled = true;
            btnFinish.IsEnabled = false;

            if (swCase1.IsChecked == true)
            {
                gozineh = 1;
            }
            else if (swCase2.IsChecked == true)
            {
                gozineh = 2;
            }
            else if (swCase3.IsChecked == true)
            {
                gozineh = 3;
            }
            else if (swCase4.IsChecked == true)
            {
                gozineh = 4;
            }
            else
            {
                gozineh = 0;
            }

            answerlist.Items[QN] = gozineh.ToString();

            if (QN > 0)
            {
                QN--;
                lblQtext.Text = result[QN].QuestionText.ToString();
                swCase1.Content = result[QN].Case1.ToString();
                swCase2.Content = result[QN].Case2.ToString();
                swCase3.Content = result[QN].Case3.ToString();
                swCase4.Content = result[QN].Case4.ToString();
                lblQNumber.Text = Convert.ToString(Convert.ToInt32(lblQNumber.Text) - 1);
            }

            if (QN == 0)
            {
                btnPrev.IsEnabled = false;
            }

            if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 0)
            {
                ClearCheck();
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 1)
            {
                swCase1.IsChecked = true;
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 2)
            {
                swCase2.IsChecked = true;
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 3)
            {
                swCase3.IsChecked = true;
            }
            else if (Convert.ToInt32(answerlist.Items[QN].ToString()) == 4)
            {
                swCase4.IsChecked = true;
            }
        }

        private void swRandom_Checked(object sender, RoutedEventArgs e)
        {
            if (swRandom.IsChecked==true)
                isGuid = true;
            else
                isGuid = false;
        }

        private void ClearCheck()
        {
            swCase1.IsChecked = false;
            swCase2.IsChecked = false;
            swCase3.IsChecked = false;
            swCase4.IsChecked = false;
        }
        private void StackPanel_Checked(object sender, RoutedEventArgs e)
        {
            Arthas.Controls.Metro.MetroSwitch cb = e.OriginalSource as Arthas.Controls.Metro.MetroSwitch;
            if (cb.IsChecked == false)
            {
                return;
            }
            foreach (var item in ((StackPanel)sender).Children)
            {
                if (item != cb)
                {
                    ((Arthas.Controls.Metro.MetroSwitch)item).IsChecked = false;
                }
            }
        }
    }
}
