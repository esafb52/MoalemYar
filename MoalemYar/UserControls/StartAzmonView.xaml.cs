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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for StartAzmon.xaml
    /// </summary>
    public partial class StartAzmonView : UserControl
    {
        private int QN = 0;
        private int gozineh;
        private int sahih, nazade, qalat;

        private List<DataClass.Tables.AQuestion> result;
        private ListBox answerlist;
        private string uClass;
        private bool isGuid = true;

        private DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public StartAzmonView()
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

        private void getSchool()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.Select(x => x);
                    if (query.Any())
                    {
                        cmbEditBase.ItemsSource = query.ToList();
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
                    MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "Student");
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
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Groups.Select(x => x);
                    if (query.Any())
                    {
                        cmbGroup.ItemsSource = query.ToList();
                    }
                    else
                    {
                        cmbGroup.ItemsSource = null;
                        MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.No_Data_KEY, param: "Group");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        #endregion Async Query

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (txtTedad.Value == 0 || cmbGroup.SelectedIndex == -1 || cmbEditStudent.SelectedIndex == -1)
            {
                MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.Fill_All_Data_KEY);
                return;
            }
            else
            {
                using (var db = new DataClass.myDbContext())
                {
                    var id = Convert.ToInt64(cmbGroup.SelectedValue);
                    var classText = uClass;

                    if (isGuid)
                        result = db.AQuestions.Where(x => x.GroupId == id && x.Class == classText).OrderBy(x => Guid.NewGuid()).Take(Convert.ToInt32(txtTedad.Value)).ToList();
                    else
                        result = db.AQuestions.Where(x => x.GroupId == id && x.Class == classText).Take(Convert.ToInt32(txtTedad.Value)).ToList();

                    if (result.Count < Convert.ToInt32(txtTedad.Value))
                        MainWindow.main.showGrowlNotification(NotificationKEY: AppVariable.Azmon_KEY);
                    else
                    {
                        QN = 0;

                        lblQNumber.Content = Convert.ToString(QN + 1);

                        lblQtext.Content = result[QN].QuestionText;
                        txtFirst.Text = result[QN].Case1;
                        txtSec.Text = result[QN].Case2;
                        txtThird.Text = result[QN].Case3;
                        txtForth.Text = result[QN].Case4;
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
                            lblTime.Content = Convert.ToString(Convert.ToInt32(txtTime.Value) * (Convert.ToInt32(txtTedad.Value)));
                            dispatcherTimer.Start();
                        }
                        else
                        {
                            lblTime.Content = "نامحدود";
                        }
                    }
                }
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblTime.Content) > 1)
            {
                lblTime.Content = Convert.ToString(Convert.ToInt32(lblTime.Content) - 1);
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
            dynamic getGroupName = cmbGroup.SelectedItem;
            string gpName = getGroupName.GroupName;
            gpControl.Visibility = Visibility.Hidden;
            btnStart.IsEnabled = true;
            txtTedad.IsEnabled = true;
            swLimit.IsEnabled = true;
            cmbGroup.IsEnabled = true;
            cmbEditBase.IsEnabled = true;
            swRandom.IsEnabled = true;
            gpAzmon.IsEnabled = false;
            ClearCheck();
            AzmonResultView._True = sahih;
            AzmonResultView._False = qalat;
            AzmonResultView._None = nazade;
            AzmonResultView._UserId = uId;
            AzmonResultView._GroupName = gpName;
            AzmonView.main.exContent.Content = new AzmonResultView();
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
                lblQtext.Content = result[QN].QuestionText;
                txtFirst.Text = result[QN].Case1;
                txtSec.Text = result[QN].Case2;
                txtThird.Text = result[QN].Case3;
                txtForth.Text = result[QN].Case4;
                lblQNumber.Content = Convert.ToString(Convert.ToInt32(lblQNumber.Content) + 1);
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
                lblQtext.Content = result[QN].QuestionText.ToString();
                txtFirst.Text = result[QN].Case1.ToString();
                txtSec.Text = result[QN].Case2.ToString();
                txtThird.Text = result[QN].Case3.ToString();
                txtForth.Text = result[QN].Case4.ToString();
                lblQNumber.Content = Convert.ToString(Convert.ToInt32(lblQNumber.Content) - 1);
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
            if (swRandom.IsChecked == true)
                isGuid = true;
            else
                isGuid = false;
        }

        private void swCase1_Checked(object sender, RoutedEventArgs e)
        {
            swCase2.IsChecked = false;
            swCase3.IsChecked = false;
            swCase4.IsChecked = false;
        }

        private void swCase2_Checked(object sender, RoutedEventArgs e)
        {
            swCase1.IsChecked = false;
            swCase3.IsChecked = false;
            swCase4.IsChecked = false;
        }

        private void swCase3_Checked(object sender, RoutedEventArgs e)
        {
            swCase1.IsChecked = false;
            swCase2.IsChecked = false;
            swCase4.IsChecked = false;
        }

        private void swCase4_Checked(object sender, RoutedEventArgs e)
        {
            swCase1.IsChecked = false;
            swCase2.IsChecked = false;
            swCase3.IsChecked = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            cmbEditBase.SelectedIndex = Convert.ToInt32(FindElement.Settings.DefaultSchool);
        }

        private void ClearCheck()
        {
            swCase1.IsChecked = false;
            swCase2.IsChecked = false;
            swCase3.IsChecked = false;
            swCase4.IsChecked = false;
        }
    }
}