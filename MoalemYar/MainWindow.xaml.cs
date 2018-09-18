﻿/****************************** ghost1372.github.io ******************************\
*	Module Name:	MainWindow.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:53 ب.ظ
*
***********************************************************************************/

using Enterwell.Clients.Wpf.Notifications;
using HandyControl.Controls;
using MoalemYar.UserControls;
using MVVMC;
using Ookii.Dialogs.Wpf;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MoalemYar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBorderless
    {
        internal static MainWindow main;
        public INotificationMessageManager Manager { get; } = new NotificationMessageManager();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            main = this;

            ShowCredentialDialog();
        }

        public void ClearScreen()
        {
            region.Content = null;
        }
        public void MenuItemShortCutCommandBackup(Object sender, ExecutedRoutedEventArgs e)
        {
            AppVariable.takeBackup();
        }
        public void MenuItemShortCutCommandRestore(Object sender, ExecutedRoutedEventArgs e)
        {
            AppVariable.dbRestore();
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var navigationService = NavigationServiceProvider.GetNavigationServiceInstance();
            var selectedItem = sender as TreeViewItem;
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
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var navigationService = NavigationServiceProvider.GetNavigationServiceInstance();
            var selectedItem = sender as MenuItem;
            switch (selectedItem.Tag)
            {
                case "Azmon":
                    navigationService.GetController<UserControls.UserControlsController>().Azmon();
                    break;

                case "makeAzmon":
                    navigationService.GetController<UserControls.UserControlsController>().StartAzmon();
                    break;

                case "quesAzmon":
                    navigationService.GetController<UserControls.UserControlsController>().AddQuestions();
                    break;

                case "resultAzmon":
                    navigationService.GetController<UserControls.UserControlsController>().AzmonHistory();
                    break;

                case "settingView":
                    navigationService.GetController<UserControls.UserControlsController>().Settings();
                    break;

                case "aboutView":
                    navigationService.GetController<UserControls.UserControlsController>().About();
                    break;

                case "groupAzmon":
                    navigationService.GetController<UserControls.UserControlsController>().AddAzmonGroup();
                    break;
                case "backup":
                    AppVariable.takeBackup();
                    break;
                case "restore":
                    AppVariable.dbRestore();
                    break;
            }
        }

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

        #region "Notification"

        public void ShowNoDataNotification(string Type)
        {
            var navigationService = NavigationServiceProvider.GetNavigationServiceInstance();
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
                           navigationService.GetController<UserControls.UserControlsController>().AddStudent();
                           break;

                       case "Score":
                           QuestionsListView.main.tabc.SelectedIndex = 0;
                           break;

                       case "TopStudent":
                           navigationService.GetController<UserControls.UserControlsController>().Questionslist();
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

        public void ShowBackupNotification(bool isSuccess, string BackupOrRestore)
        {
            if (isSuccess)
            {
                var builder = this.Manager
                                .CreateMessage()
                               .Accent(AppVariable.GREEN)
                               .Background(AppVariable.BGBLACK)
                               .HasBadge("اطلاعیه")
                               .HasMessage($"{BackupOrRestore} با موفقیت انجام شد")
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
                               .HasMessage($"{BackupOrRestore} با مشکل مواجه شد")
                               .Dismiss().WithButton("باشه", button => { })
                               .Animates(true)
                               .AnimationInDuration(AppVariable.NotificationAnimInDur)
                               .AnimationOutDuration(AppVariable.NotificationAnimOutDur)
                               .Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
                builder.Queue();
            }
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
                         SettingsView.main.resetConfig();
                     else
                         SettingsView.main.resetDatabase();
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
    }
}