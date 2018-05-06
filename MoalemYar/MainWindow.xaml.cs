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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
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
        private PersianCalendar pc = new PersianCalendar();
        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            main = this;

            //Todo: Add Defualt School and Set Default Combo School
            //Todo: Remove RunAction
            appTitle = AppVariable.getAppTitle + AppVariable.getAppVersion + AppVariable.RunActionMeasurePerformance(() => getexHint()); // App Title with Version

            ShowCredentialDialog();

            LoadSettings();

            LogifyCrashReport();

        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Todo: Enable this
            //getexHint();
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
                exAttendancelist.Hint = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");
            }
            catch (Exception)
            {
            }
        }

        #endregion Func get Query Wait"

        public void LogifyCrashReport()
        {
            try
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
            catch (Exception)
            {

            }
        }

        private void LoadSettings()
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
                var brush = new SolidColorBrush(color);
                BorderBrush = brush;

                var hb_Menu = AppVariable.ReadBoolSetting(AppVariable.HamburgerMenu);
                MainWindow.main.tab.IconMode = !hb_Menu;

                var vCode = AppVariable.ReadSetting(AppVariable.VersionCode);
                if (!vCode.Equals(AppVariable.getAppVersion))
                    AppVariable.InitializeSettings();
                //Todo: Import Database to new Version
            }
            catch (Exception)
            {
                 
            }
            

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

                       case "Attendance":
                           Attendancelist.main.tabc.SelectedIndex = 0;
                           break;

                       case "Question":
                           exAddOrUpdateStudent_Click(null, null);
                           break;

                       case "Score":
                           QuestionsList.main.tabc.SelectedIndex = 0;
                           break;
                   }
               })
               .Dismiss().WithButton("بیخیال", button => { })
                .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
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
               .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
            .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
            builder.Queue();
        }

        public void ShowDeleteExistNotification(string Type, string Type2)
        {
            var builder = this.Manager
                .CreateMessage()
               .Accent(AppVariable.RED)
               .Background(AppVariable.BGBLACK)
               .HasBadge("هشدار")
               .HasMessage(string.Format("نمی توان این {0} را حذف کرد، ابتدا {1} این {0} را حذف کنید", Type, Type2))
               .Dismiss().WithButton("باشه", button => { })
               .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
            .Dismiss().WithDelay(TimeSpan.FromSeconds(5));
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
               .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
            .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
            builder.Queue();
        }

        public void ShowUpdateDataNotification(bool isUpdateSuccess, string Name, string Type)
        {
            if (isUpdateSuccess)
            {
                var builder = this.Manager
                .CreateMessage()
               .Accent(AppVariable.ORANGE)
               .Background(AppVariable.BGBLACK)
               .HasBadge("اطلاعیه")
               .HasMessage(string.Format("{1} {0} با موفقیت ویرایش شد", Name, Type))
               .Dismiss().WithButton("باشه", button => { })
               .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
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
               .HasMessage(string.Format("ویرایش {1} {0} با خطا مواجه شد", Name, Type))
               .Dismiss().WithButton("دوباره امتحان کنید", button => { })
               .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
        }

        public void ShowDeletedNotification(bool isDeleteSuccess, string Name, string Type)
        {
            if (isDeleteSuccess)
            {
                var builder = this.Manager
                   .CreateMessage()
                   .Accent(AppVariable.BLUE)
                   .Background(AppVariable.BGBLACK)
                   .HasBadge("اطلاعیه")
                   .HasMessage(string.Format("{1} {0} با موفقیت حذف شد", Name, Type))
                   .Dismiss().WithButton("باشه", button => { })
                   .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
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
                   .HasMessage(string.Format("حذف {1} {0} با خطا مواجه شد", Name, Type))
                   .Dismiss().WithButton("دوباره امتحان کنید", button => { })
                   .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
                .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                builder.Queue();
            }
        }

        public void ShowAddDataNotification(bool isAddSuccess, string Name, string Type)
        {
            if (isAddSuccess)
            {
                var builder = this.Manager
               .CreateMessage()
               .Accent(AppVariable.GREEN)
               .Background(AppVariable.BGBLACK)
               .HasBadge("اطلاعیه")
               .HasMessage(string.Format("{1} {0} با موفقیت ثبت شد", Name, Type))
               .Dismiss().WithButton("باشه", button => { })
               .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
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
               .HasMessage(string.Format("ثبت {1} {0} با خطا مواجه شد", Name, Type))
               .Dismiss().WithButton("دوباره امتحان کنید", button => { })
               .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
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
                 .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
                 .Dismiss().WithDelay(TimeSpan.FromSeconds(3));
            builder.Queue();
        }

        public void DataResetDeletedNotification(string Type)
        {
            var builder = this.Manager
                   .CreateMessage()
                   .Accent(AppVariable.GREEN)
                   .Background(AppVariable.BGBLACK)
                   .HasBadge("اطلاعیه")
                   .HasMessage(string.Format("{0} به حالت پیشفرض تغییر یافت، برنامه را دوباره راه اندازی کنید", Type))
                    .WithButton("راه اندازی", button =>
                    {
                        Application.Current.Shutdown();
                        System.Windows.Forms.Application.Restart();
                    })
                 .Dismiss().WithButton("بیخیال", button => { })
                 .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
                 .Dismiss().WithDelay(TimeSpan.FromSeconds(5));
            builder.Queue();
        }

        public void ResetDataConfirmNotification(string Type)
        {
            var builder = this.Manager
                  .CreateMessage()
                 .Accent(AppVariable.RED)
                 .Background(AppVariable.BGBLACK)
                 .HasBadge("هشدار")
                 .HasHeader(string.Format("آیا برای بازیابی {0} اطمینان دارید؟", Type))
                 .Dismiss().WithButton("بله", button =>
                 {
                     if (Type == "تنظیمات برنامه")
                         Settings.main.resetConfig();
                     else
                         Settings.main.resetDatabase();
                 })
                 .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
                 .Dismiss().WithButton("خیر", button => { });
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
                    .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
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
                   .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
                   .Dismiss().WithButton("تایید", button => { });

                builder.Queue();
            }
        }

        public void ShowDeleteConfirmNotification(string Name, string Type)
        {
            var builder = this.Manager
                  .CreateMessage()
                 .Accent(AppVariable.RED)
                 .Background(AppVariable.BGBLACK)
                 .HasBadge("هشدار")
                 .HasHeader(string.Format("آیا برای حذف {1} {0} اطمینان دارید؟", Name, Type))
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

                         case "حضورغیاب":
                             Attendancelist.main.deleteAttendance();
                             break;

                         case "نمره":
                             QuestionsList.main.deleteScore();
                             break;
                     }
                 })
                 .Animates(true)
               .AnimationInDuration(0.75)
               .AnimationOutDuration(0.5)
                 .Dismiss().WithButton("خیر", button => { });
            builder.Queue();
        }

        #endregion "Notification"

        private void ShowCredentialDialog()
        {
            try
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
            catch (Exception)
            {

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

        private void exAddOrUpdateStudent_Click(object sender, EventArgs e)
        {
            exContent.Content = new AddStudent();
        }

        private void exAttendancelist_Click(object sender, EventArgs e)
        {
            exContent.Content = new Attendancelist();
        }

        private void exQuestionsList_Click(object sender, EventArgs e)
        {
            exContent.Content = new QuestionsList();
        }
    }
}