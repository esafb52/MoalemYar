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
using Enterwell.Clients.Wpf.Notifications;
using MoalemYar.UserControls;
using MVVMC;
using Ookii.Dialogs.Wpf;
using System;
using System.Globalization;
using System.Linq;
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
        private int _sch = 0;
        private int _stu = 0;
        private int _usr = 0;
        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            main = this;

            appTitle = AppVariable.getAppTitle + AppVariable.getAppVersion; // App Title with Version
            ShowCredentialDialog();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSettings();
            getexHint();
            
        }

        #region Query

        public void getexHint()
        {
            try
            {
                using (var db = new DataClass.myDbContext())
                {
                    var query = db.Schools.Select(x => x);
                    exAddOrUpdateSchool.Hint = query.Count().ToString();

                    var query2 = db.Users.Select(x => x);
                    exAddOrUpdateUser.Hint = query2.Count().ToString();

                    var query3 = db.Students.Select(x => x);
                    exAddOrUpdateStudent.Hint = query3.Count().ToString();
                    _sch = query.Count();
                    _stu = query3.Count();
                    _usr = query2.Count();
                }

                exAttendancelist.Hint = pc.GetYear(DateTime.Now).ToString("0000") + "/" + pc.GetMonth(DateTime.Now).ToString("00") + "/" + pc.GetDayOfMonth(DateTime.Now).ToString("00");
            }
            catch (Exception)
            {
            }
        }

        #endregion Query

        private void LoadSettings()
        {
            try
            {
                var color = (Color)ColorConverter.ConvertFromString(FindElement.Settings.SkinCode ?? AppVariable.DEFAULT_BORDER_BRUSH);
                var brush = new SolidColorBrush(color);
                BorderBrush = brush;

                var hb_Menu = FindElement.Settings.HamburgerMenu ?? true;
                tab.IconMode = !hb_Menu;
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
                           AddSchoolView.main.tabc.SelectedIndex = 0;
                           break;

                       case "User":
                           AddUserView.main.tabc.SelectedIndex = 0;
                           break;

                       case "Student":
                           AddStudentView.main.tabc.SelectedIndex = 0;
                           break;

                       case "Attendance":
                           AttendancelistView.main.tabc.SelectedIndex = 0;
                           break;

                       case "Question":
                           //exAddOrUpdateStudent_Click(null, null);
                           break;

                       case "Score":
                           QuestionsListView.main.tabc.SelectedIndex = 0;
                           break;

                       case "TopStudent":
                           //exQuestionsList_Click(null, null);
                           break;

                       case "Group":
                           AddAzmonGroupView.main.tabc.SelectedIndex = 0;
                           break;

                       case "AQuestions":
                           AddQuestionsView.main.tabc.SelectedIndex = 0;
                           break;
                   }
               })
               .Dismiss().WithButton("بیخیال", button => { })
                .Animates(true)
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
            builder.Queue();
        }

        public void ShowAzmonNotification()
        {
            var builder = this.Manager
                .CreateMessage()
               .Accent(AppVariable.RED)
               .Background(AppVariable.BGBLACK)
               .HasBadge("هشدار")
               .HasMessage("تعداد سوالات وارد شده بیشتر از سوالات موجود است")
               .Dismiss().WithButton("باشه", button => { })
               .Animates(true)
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
            builder.Queue();
        }

        public void ShowRecivedCircularNotification(bool isSuccess)
        {
            if (isSuccess)
            {
                var builder = this.Manager
                                .CreateMessage()
                               .Accent(AppVariable.GREEN)
                               .Background(AppVariable.BGBLACK)
                               .HasBadge("اطلاعیه")
                               .HasMessage("تمامی بخشنامه ها با موفقیت دریافت شد")
                               .Dismiss().WithButton("باشه", button => { })
                               .Animates(true)
                               .AnimationInDuration(AppVariable.NotificationAnimInDur)
                               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
                               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
                builder.Queue();
            }
            else
            {
                var builder = this.Manager
                                .CreateMessage()
                               .Accent(AppVariable.RED)
                               .Background(AppVariable.BGBLACK)
                               .HasBadge("هشدار")
                               .HasMessage("درحال حاظر سرور در دسترس نیست! لطفا در صورت فعال بودن، VPN خود را غیرفعال کنید")
                               .Dismiss().WithButton("باشه", button => { })
                               .Animates(true)
                               .AnimationInDuration(AppVariable.NotificationAnimInDur)
                               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
                               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
                builder.Queue();
            }
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
                builder.Queue();
            }
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
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
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
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
                             AddSchoolView.main.deleteSchool();
                             break;

                         case "دانش آموز":
                             AddStudentView.main.deleteStudent();
                             break;

                         case "کاربر":
                             AddUserView.main.deleteUser();
                             break;

                         case "حضورغیاب":
                             AttendancelistView.main.deleteAttendance();
                             break;

                         case "نمره":
                             QuestionsListView.main.deleteScore();
                             break;

                         case "گروه":
                             AddAzmonGroupView.main.deleteGroup();
                             break;

                         case "سوال":
                             AddQuestionsView.main.deleteGroup();
                             break;
                     }
                 })
                 .Animates(true)
               .AnimationInDuration(AppVariable.NotificationAnimInDur)
               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
                 .Dismiss().WithButton("خیر", button => { });
            builder.Queue();
        }

        #endregion "Notification"

        private void ShowCredentialDialog()
        {
            try
            {
                var isLogin = FindElement.Settings.CredentialLogin;
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

        private void NavigationMenuItem_Click(object sender, EventArgs e)
        {
            var navigationService = NavigationServiceProvider.GetNavigationServiceInstance();
            var selectedItem = sender as MetroExpander;
            switch (selectedItem.Tag)
            {
                case "initial":
                    navigationService.GetController<UserControls.UserControlsController>().Initial();
                    break;
                case "addOrUpdateSchool":
                    navigationService.GetController<UserControls.UserControlsController>().AddSchool();
                    break;
                case "exAddOrUpdateStudent":
                    navigationService.GetController<UserControls.UserControlsController>().AddStudent();
                    break;
                case "exAddOrUpdateUser":
                    navigationService.GetController<UserControls.UserControlsController>().AddUser();
                    break;
                case "exAttendancelist":
                    navigationService.GetController<UserControls.UserControlsController>().Attendancelist();
                    break;
                case "exQuestionsList":
                    navigationService.GetController<UserControls.UserControlsController>().Questionslist();
                    break;
                case "exTopStudents":
                    navigationService.GetController<UserControls.UserControlsController>().TopStudents();
                    break;
                case "exAchievement":
                    navigationService.GetController<UserControls.UserControlsController>().Achievement();
                    break;
                case "exCircular":
                    navigationService.GetController<UserControls.UserControlsController>().Circular();
                    break;
                case "exBook":
                    navigationService.GetController<UserControls.UserControlsController>().Books();
                    break;
                case "exRoshd":
                    navigationService.GetController<UserControls.UserControlsController>().Magazine();
                    break;
            }
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
    }
}