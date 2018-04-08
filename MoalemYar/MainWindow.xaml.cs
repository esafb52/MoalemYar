/****************************** ghost1372.github.io ******************************\
*	Module Name:	MainWindow.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:53 ب.ظ
*
***********************************************************************************/

using Arthas.Controls.Metro;
using DevExpress.Logify.WPF;
using Enterwell.Clients.Wpf.Notifications;
using MoalemYar.UserControls;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public string appTitle { get; set; }
        internal static MainWindow main;
        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            main = this;

            appTitle = AppVariable.getAppTitle + AppVariable.getAppVersion; // App Title with Version

            ShowCredentialDialog();

            LogifyCrashReport();

            LoadSettings();
        }

        #region "Async Query"

        public async static Task<List<DataClass.Tables.School>> GetAllSchoolAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Schools.Select(x => x);
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.Tables.User>> GetAllUserAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Users.Select(x => x);
                return await query.ToListAsync();
            }
        }

        public async static Task<List<DataClass.Tables.Student>> GetAllStudentAsync()
        {
            using (var db = new DataClass.myDbContext())
            {
                var query = db.Students.Select(x => x);
                return await query.ToListAsync();
            }
        }

        #endregion "Async Query"

        #region Func get Query Wait"

        public void getexHint()
        {
            try
            {
                var querySchool = GetAllSchoolAsync();
                querySchool.Wait();

                var queryUser = GetAllUserAsync();
                queryUser.Wait();

                var queryStudent = GetAllStudentAsync();
                queryStudent.Wait();

                List<DataClass.Tables.School> dataSchool = querySchool.Result;
                List<DataClass.Tables.User> dataUser = queryUser.Result;
                List<DataClass.Tables.Student> dataStudent = queryStudent.Result;

                exAddOrUpdateSchool.Hint = dataSchool.Count().ToString();
                exAddOrUpdateUser.Hint = dataUser.Count().ToString();
                exAddOrUpdateStudent.Hint = dataStudent.Count().ToString();
            }
            catch (Exception)
            {
            }
        }

        #endregion Func get Query Wait"

        public void LogifyCrashReport()
        {
            var isEnabledReport = AppVariable.ReadBoolSetting(AppVariable.AutoSendReport);
            LogifyAlert client = LogifyAlert.Instance;
            client.ApiKey = AppVariable.LogifyAPIKey;
            client.AppName = AppVariable.getAppName;
            client.AppVersion = AppVariable.getAppVersion;
            client.OfflineReportsEnabled = true;
            client.OfflineReportsCount = 20;
            client.OfflineReportsDirectory = AppVariable.LogifyOfflinePath;
            client.SendOfflineReports();
            client.StartExceptionsHandling();
            if (isEnabledReport.Equals("True"))
                client.StartExceptionsHandling();
            else
                client.StopExceptionsHandling();
        }

        private void LoadSettings()
        {
            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            BorderBrush = brush;

            var hb_Menu = AppVariable.ReadBoolSetting(AppVariable.HamburgerMenu);
            MainWindow.main.tab.IconMode = !hb_Menu;
        }

        #region "Notification"

        public void ShowNoDataNotification(string Type)
        {
            var builder = this.Manager
               .CreateMessage()
               .Accent(AppVariable.RED)
               .Background(AppVariable.BGBLACK)
               .HasBadge("هشدار")
               .HasMessage("اطلاعاتی در پایگاه داده یافت نشد")
               .Dismiss().WithButton("ثبت اطلاعات جدید", button =>
               {
                   switch (Type)
                   {
                       case "School":
                           AddSchool.main.tabc.SelectedIndex = 0;
                           break;

                       case "User":
                           AddUser.main.tabc.SelectedIndex = 0;
                           break;

                       case "Student":
                           AddStudent.main.tabc.SelectedIndex = 0;
                           break;
                   }
               })
               .Dismiss().WithButton("بیخیال", button => { })
               .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
            builder.Queue();
        }

        public void ShowFillAllDataNotification()
        {
            var builder = this.Manager
                .CreateMessage()
               .Accent(AppVariable.RED)
               .Background(AppVariable.BGBLACK)
               .HasBadge("هشدار")
               .HasMessage("لطفا تمام فیلدها را پر کنید")
               .Dismiss().WithButton("باشه", button => { })
            .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
            builder.Queue();
        }

        public void ShowSamePasswordNotification()
        {
            var builder = this.Manager
                .CreateMessage()
               .Accent(AppVariable.RED)
               .Background(AppVariable.BGBLACK)
               .HasBadge("هشدار")
               .HasMessage("رمز های عبور باید یکسان باشند")
               .Dismiss().WithButton("باشه", button => { })
            .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
            builder.Queue();
        }

        public void ShowUpdateDataNotification(bool isUpdateSuccess, string SchoolName, string Type)
        {
            if (isUpdateSuccess)
            {
                var builder = this.Manager
                .CreateMessage()
               .Accent(AppVariable.ORANGE)
               .Background(AppVariable.BGBLACK)
               .HasBadge("اطلاعیه")
               .HasMessage(string.Format("{1} {0} با موفقیت ویرایش شد", SchoolName, Type))
               .Dismiss().WithButton("باشه", button => { })
               .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
            else
            {
                var builder = this.Manager
                .CreateMessage()
               .Accent(AppVariable.RED)
               .Background(AppVariable.BGBLACK)
               .HasBadge("هشدار")
               .HasMessage(string.Format("ویرایش {1} {0} با خطا مواجه شد", SchoolName, Type))
               .Dismiss().WithButton("دوباره امتحان کنید", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
        }

        public void ShowDeletedNotification(bool isDeleteSuccess, string SchoolName, string Type)
        {
            if (isDeleteSuccess)
            {
                var builder = this.Manager
                   .CreateMessage()
                   .Accent(AppVariable.BLUE)
                   .Background(AppVariable.BGBLACK)
                   .HasBadge("اطلاعیه")
                   .HasMessage(string.Format("{1} {0} با موفقیت حذف شد", SchoolName, Type))
                   .Dismiss().WithButton("باشه", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
            else
            {
                var builder = this.Manager
                    .CreateMessage()
                   .Accent(AppVariable.RED)
                   .Background(AppVariable.BGBLACK)
                   .HasBadge("هشدار")
                   .HasMessage(string.Format("حذف {1} {0} با خطا مواجه شد", SchoolName, Type))
                   .Dismiss().WithButton("دوباره امتحان کنید", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
        }

        public void ShowAddDataNotification(bool isAddSuccess, string SchoolName, string Type)
        {
            if (isAddSuccess)
            {
                var builder = this.Manager
               .CreateMessage()
               .Accent(AppVariable.GREEN)
               .Background(AppVariable.BGBLACK)
               .HasBadge("اطلاعیه")
               .HasMessage(string.Format("{1} {0} با موفقیت ثبت شد", SchoolName, Type))
               .Dismiss().WithButton("باشه", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
            else
            {
                var builder = this.Manager
               .CreateMessage()
               .Accent(AppVariable.RED)
               .Background(AppVariable.BGBLACK)
               .HasBadge("هشدار")
               .HasMessage(string.Format("ثبت {1} {0} با خطا مواجه شد", SchoolName, Type))
               .Dismiss().WithButton("دوباره امتحان کنید", button => { })
               .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
        }

        public void RestartNotification()
        {
            var builder = this.Manager
                  .CreateMessage()
                 .Accent(AppVariable.ORANGE)
                 .Background(AppVariable.BGBLACK)
                 .HasBadge("هشدار")
                 .HasHeader("برای اعمال رنگ بندی بصورت کامل برنامه را دوباره راه اندازی کنید")
                 .WithButton("راه اندازی", button =>
                 {
                     Application.Current.Shutdown();
                     System.Windows.Forms.Application.Restart();
                 })
                 .Dismiss().WithButton("بیخیال", button => { })
                 .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
            builder.Queue();
        }

        public void ShowUpdateNotification(bool isAvailable, string Version, string URL)
        {
            if (isAvailable)
            {
                var builder = this.Manager
                   .CreateMessage()
                    .Accent(AppVariable.GREEN)
                    .Background(AppVariable.BGBLACK)
                    .HasBadge("اطلاعیه")
                    .HasHeader(string.Format("نسخه جدید {0} پیدا شد،همین حالا به آخرین نسخه بروزرسانی کنید", Version))
                    .WithButton("ارتقا", button =>
                    {
                        System.Diagnostics.Process.Start(URL);
                    })
                    .Dismiss().WithButton("بیخیال", button => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
            else
            {
                var builder = this.Manager
                  .CreateMessage()
                   .Accent(AppVariable.RED)
                   .Background(AppVariable.BGBLACK)
                   .HasBadge("هشدار")
                   .HasHeader(string.Format("شما از آخرین نسخه {0} استفاده می کنید", AppVariable.getAppVersion))
                   .Dismiss().WithDelay(TimeSpan.FromSeconds(3))
                   .Dismiss().WithButton("تایید", button => { });

                builder.Queue();
            }
        }

        public void ShowDeleteConfirmNotification(string SchoolName, string Type)
        {
            var builder = this.Manager
                  .CreateMessage()
                 .Accent(AppVariable.RED)
                 .Background(AppVariable.BGBLACK)
                 .HasBadge("هشدار")
                 .HasHeader(string.Format("آیا برای حذف {1} {0} اطمینان دارید؟", SchoolName, Type))
                 .Dismiss().WithButton("بله", button =>
                 {
                     switch (Type)
                     {
                         case "مدرسه":
                             AddSchool.main.deleteSchool();
                             break;

                         case "دانش آموز":
                             AddStudent.main.deleteStudent();
                             break;

                         case "کاربر":
                             AddUser.main.deleteUser();
                             break;
                     }
                 })
                 .Dismiss().WithButton("خیر", button => { });
            builder.Queue();
        }

        #endregion "Notification"

        private void ShowCredentialDialog()
        {
            var isLogin = AppVariable.ReadBoolSetting(AppVariable.CredentialLogin);
            if (isLogin)
            {
                using (CredentialDialog dialog = new CredentialDialog())
                {
                    dialog.WindowTitle = "ورود به نرم افزار";
                    dialog.MainInstruction = "لطفا نام کاربری و رمز عبور خود را وارد کنید";
                    //dialog.Content = "";
                    dialog.ShowSaveCheckBox = true;
                    dialog.ShowUIForSavedCredentials = true;
                    // The target is the key under which the credentials will be stored.
                    dialog.Target = "Mahdi72_MoalemYar_www.127.0.0.1.com";

                    try
                    {
                        while (isLogin)
                        {
                            if (dialog.ShowDialog(this))
                            {
                                using (var db = new DataClass.myDbContext())
                                {
                                    var usr = db.Users.Where(x => x.Username == dialog.Credentials.UserName && x.Password == dialog.Credentials.Password);
                                    if (usr.Any())
                                    {
                                        dialog.ConfirmCredentials(true);
                                        isLogin = false;
                                    }
                                    else
                                    {
                                        dialog.Content = "مشخصات اشتباه است دوباره امتحان کنید";
                                    }
                                }
                            }
                            else
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
            }
        }

        #region "Progressbar"

        //private ProgressDialog _sampleProgressDialog = new ProgressDialog()
        //{
        //    WindowTitle = "درحال پردازش",
        //    Text = "درحال پردازش اطلاعات...",
        //    Description = "Processing...",
        //    ShowTimeRemaining = true,
        //    CancellationText = "عملیات لغو شد",
        //};
        //_sampleProgressDialog.DoWork += new System.ComponentModel.DoWorkEventHandler(_sampleProgressDialog_DoWork);
        //private void ShowProgressDialog()
        //{
        //    if (_sampleProgressDialog.IsBusy)
        //        MessageBox.Show(this, "The progress dialog is already displayed.", "Progress dialog sample");
        //    else
        //        _sampleProgressDialog.Show(); // Show a modeless dialog; this is the recommended mode of operation for a progress dialog.
        //}
        //private void _sampleProgressDialog_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    // Implement the operation that the progress bar is showing progress of here, same as you would do with a background worker.
        //    for (int x = 0; x <= 100; ++x)
        //    {
        //        Thread.Sleep(50);
        //        // Periodically check CancellationPending and abort the operation if required.
        //        if (_sampleProgressDialog.CancellationPending)
        //            return;
        //        // ReportProgress can also modify the main text and description; pass null to leave them unchanged.
        //        // If _sampleProgressDialog.ShowTimeRemaining is set to true, the time will automatically be calculated based on
        //        // the frequency of the calls to ReportProgress.
        //        _sampleProgressDialog.ReportProgress(x, null, string.Format(System.Globalization.CultureInfo.CurrentCulture, "Processing: {0}%", x));
        //    }
        //}

        #endregion "Progressbar"

        private void tab_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tab.SelectedIndex)
            {
                case 0:

                    break;

                case 1:

                    break;

                case 2:

                    break;

                case 3:

                    break;
            }
        }

        private void exAddOrUpdateSchool_Click(object sender, EventArgs e)
        {
            exContent.Content = new AddSchool();
        }

        private void exAddOrUpdateClass_Click(object sender, EventArgs e)
        {
            exContent.Content = new About();
        }

        private void MetroExpander_Click(object sender, EventArgs e)
        {
            if (exActivity.IsExpanded)
                exActivity.IsExpanded = false;
        }

        private void exActivity_Click(object sender, EventArgs e)
        {
            if (exBase.IsExpanded)
                exBase.IsExpanded = false;
        }

        private void exAddOrUpdateUser_Click(object sender, EventArgs e)
        {
            exContent.Content = new AddUser();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            getexHint();
        }

        private void exAddOrUpdateStudent_Click(object sender, EventArgs e)
        {
            exContent.Content = new AddStudent();
        }
    }
}