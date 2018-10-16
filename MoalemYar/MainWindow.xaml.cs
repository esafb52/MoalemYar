/****************************** ghost1372.github.io ******************************\
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
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private void ShowCredentialDialog()
        {
            var isLogin = FindElement.Settings.CredentialLogin;
            if (isLogin)
            {
                PopupWindow popupLogin = new PopupWindow()
                {
                    MinWidth = 400,
                    FontFamily = TryFindResource("TeacherYar.Fonts.IRANSans") as FontFamily,
                    FontSize = 14,
                    Title = "ورود به نرم افزار",
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    ShowInTaskbar = true,
                    AllowsTransparency = true
                };
                popupLogin.Closing += (s, e) =>
                {
                    e.Cancel = isLogin;

                    if (e.Cancel = isLogin)
                        Environment.Exit(0);
                };
                StackPanel mainStack = new StackPanel()
                {
                    FlowDirection = FlowDirection.RightToLeft
                };
                TextBox txtUsername = new TextBox() { TabIndex = 0, Style = TryFindResource("TextBoxExtend") as Style, Margin = new System.Windows.Thickness(10) };
                PasswordBox txtPassword = new PasswordBox() { TabIndex = 1, Style = TryFindResource("PasswordBoxExtend") as Style, Margin = new System.Windows.Thickness(10) };

                InfoElement.SetContentHeight(txtUsername, 35);
                InfoElement.SetContentHeight(txtPassword, 35);

                InfoElement.SetTitle(txtUsername, "نام کاربری و رمز عبور خود را وارد کنید");
                InfoElement.SetTitleAlignment(txtUsername, HandyControl.Data.Enum.TitleAlignment.Top);

                InfoElement.SetPlaceholder(txtUsername, "نام کاربری");
                InfoElement.SetPlaceholder(txtPassword, "رمز عبور ");

                Style buttonStyle = TryFindResource("ButtonPrimary") as Style;
                Button btnCancel = new Button { TabIndex = 3, IsCancel = true, Margin = new System.Windows.Thickness(10, 0, 10, 0), Style = buttonStyle, Content = "انصراف", Width = 100, HorizontalContentAlignment = HorizontalAlignment.Center };
                Button btnLogin = new Button { TabIndex = 2, IsDefault = true, Margin = new System.Windows.Thickness(10, 0, 10, 0), Style = buttonStyle, Content = "ورود", Width = 100, HorizontalContentAlignment = HorizontalAlignment.Center };

                btnCancel.Click += (s, e) => { Environment.Exit(0); };
                btnLogin.Click += (s, e) =>
                {
                    try
                    {
                        using (var db = new DataClass.myDbContext())
                        {
                            var usr = db.Users.Where(x => x.Username == txtUsername.Text && x.Password == txtPassword.Password);
                            if (usr.Any())
                            {
                                isLogin = false;
                                popupLogin.Close();
                            }
                            else
                            {
                                txtPassword.Focus();
                                txtPassword.Password = string.Empty;
                                InfoElement.SetPlaceholder(txtPassword, "مشخصات اشتباه است دوباره امتحان کنید");
                            }
                        }
                    }
                    catch (InvalidOperationException)
                    {
                    }
                };

                StackPanel btnStack = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10), HorizontalAlignment = HorizontalAlignment.Center };

                btnStack.Children.Add(btnLogin);
                btnStack.Children.Add(btnCancel);

                mainStack.Children.Add(txtUsername);
                mainStack.Children.Add(txtPassword);
                mainStack.Children.Add(btnStack);

                popupLogin.Child = mainStack;

                popupLogin.ShowDialog();
            }
        }

        #region "Notification"
        public void showNotification(string NotificationKEY, bool isAvailableOrSuccess = false, params string[] param)
        {
            var builder = NotificationMessageBuilder.CreateMessage();
            builder.Manager = this.Manager;
            builder.Message = this.Manager.Factory.GetMessage();
            builder.Background(AppVariable.BGBLACK);

            builder.Animates(false);
            builder.Dismiss().WithDelay(TimeSpan.FromSeconds(AppVariable.NotificationDelay));
            builder.AnimationInDuration(AppVariable.NotificationAnimInDur);
            builder.AnimationOutDuration(AppVariable.NotificationAnimOutDur);

            //Delete Confirm
            if (NotificationKEY.Equals(AppVariable.Delete_Confirm_KEY))
            {
                builder.HasBadge("هشدار");
                builder.Accent(AppVariable.RED);
                builder.HasHeader($"آیا برای حذف {param[1]} {param[0]} اطمینان دارید؟");
                builder.Dismiss().WithButton("بله", button => {
                    switch (param[1])
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
                });
                builder.Dismiss().WithButton("خیر", button => { });
            }
            //Reset Data Confirm
            else if (NotificationKEY.Equals(AppVariable.Reset_Data_Confirm_KEY))
            {
                builder.HasBadge("هشدار");
                builder.Accent(AppVariable.RED);
                builder.HasHeader($"آیا برای بازیابی {param[0]} اطمینان دارید؟");
                builder.Dismiss().WithButton("بله", button =>
                 {
                     if (param[0] == "تنظیمات برنامه")
                         SettingsView.main.resetConfig();
                     else
                         SettingsView.main.resetDatabase();
                 });
                builder.Dismiss().WithButton("خیر", button => { });
            }

            //Reset Data Deleted
            else if (NotificationKEY.Equals(AppVariable.Data_Reset_Deleted_KEY))
            {
                builder.Accent(AppVariable.GREEN);
                builder.HasBadge("اطلاعیه");
                builder.HasMessage($"{param[0]} به حالت پیشفرض تغییر یافت، برنامه را دوباره راه اندازی کنید");
                builder.WithButton("راه اندازی", button =>
                {
                    Application.Current.Shutdown();
                    System.Windows.Forms.Application.Restart();
                });
            }

            //Password Same
            else if (NotificationKEY.Equals(AppVariable.Same_Password_KEY))
            {
                builder.Accent(AppVariable.RED);
                builder.HasBadge("هشدار");
                builder.HasMessage("رمز های عبور باید یکسان باشند");
                builder.Dismiss().WithButton("باشه", button => { });
            }

            //Delete Exist
            else if (NotificationKEY.Equals(AppVariable.Delete_Exist_KEY))
            {
                builder.Accent(AppVariable.RED);
                builder.HasBadge("هشدار");
                builder.HasMessage($"نمی توان این {param[0]} را حذف کرد، ابتدا {param[1]} این {param[0]} را حذف کنید");
                builder.Dismiss().WithButton("باشه", button => { });
            }

            //Azmon
            else if (NotificationKEY.Equals(AppVariable.Azmon_KEY))
            {
                builder.Accent(AppVariable.RED);
                builder.HasBadge("هشدار");
                builder.HasMessage("تعداد سوالات وارد شده بیشتر از سوالات موجود است");
                builder.Dismiss().WithButton("باشه", button => { });
            }

            //Fill All Data
            else if (NotificationKEY.Equals(AppVariable.Fill_All_Data_KEY))
            {
                builder.Accent(AppVariable.RED);
                builder.HasBadge("هشدار");
                builder.HasMessage("لطفا تمام فیلدها را پر کنید");
                builder.Dismiss().WithButton("باشه", button => { });
            }

            //No Data
            else if (NotificationKEY.Equals(AppVariable.No_Data_KEY))
            {
                var navigationService = NavigationServiceProvider.GetNavigationServiceInstance();

                builder.Accent(AppVariable.RED);
                builder.HasBadge("هشدار");
                builder.HasMessage("اطلاعاتی در پایگاه داده یافت نشد");
                builder.Dismiss().WithButton("ثبت اطلاعات جدید", button =>
                {
                    switch (param[0
])
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
                });
                builder.Dismiss().WithButton("بیخیال", button => { });
            }

            //Backup
            else if (NotificationKEY.Equals(AppVariable.Backup_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    builder.Accent(AppVariable.GREEN);
                    builder.HasBadge("اطلاعیه");
                    builder.HasMessage($"{param[0]} با موفقیت انجام شد");
                    builder.Dismiss().WithButton("باشه", button => { });
                }
                else
                {
                    builder.Accent(AppVariable.RED);
                    builder.HasBadge("هشدار");
                    builder.HasMessage($"{param[0]} با مشکل مواجه شد");
                    builder.Dismiss().WithButton("باشه", button => { });
                }
            }

            //Circular
            else if (NotificationKEY.Equals(AppVariable.Recived_Circular_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    builder.Accent(AppVariable.GREEN);
                    builder.HasBadge("اطلاعیه");
                    builder.HasMessage("تمامی بخشنامه ها با موفقیت دریافت شد");
                    builder.Dismiss().WithButton("باشه", button => { });
                }
                else
                {
                    builder.Accent(AppVariable.RED);
                    builder.HasBadge("هشدار");
                    builder.HasMessage("درحال حاظر سرور در دسترس نیست! لطفا در صورت فعال بودن، VPN خود را غیرفعال کنید");
                    builder.Dismiss().WithButton("باشه", button => { });
                }
            }

            //Update Data
            else if (NotificationKEY.Equals(AppVariable.Update_Data_KEY))
            {
                if (isAvailableOrSuccess)
                {

                    builder.Accent(AppVariable.ORANGE);
                    builder.HasBadge("اطلاعیه");
                    builder.HasMessage($"{param[1]} {param[0]} با موفقیت ویرایش شد");
                    builder.Dismiss().WithButton("باشه", button => { });
                }
                else
                {
                    builder.Accent(AppVariable.RED);
                    builder.HasBadge("هشدار");
                    builder.HasMessage($"ویرایش {param[1]} {param[0]} با خطا مواجه شد");
                    builder.Dismiss().WithButton("دوباره امتحان کنید", button => { });
                }
            }

            //Deleted
            else if (NotificationKEY.Equals(AppVariable.Deleted_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    builder.Accent(AppVariable.BLUE);
                    builder.HasBadge("اطلاعیه");
                    builder.HasMessage($"{param[1]} {param[0]} با موفقیت حذف شد");
                    builder.Dismiss().WithButton("باشه", button => { });
                }
                else
                {
                    builder.Accent(AppVariable.RED);
                    builder.HasBadge("هشدار");
                    builder.HasMessage($"حذف {param[1]} {param[0]} با خطا مواجه شد");
                    builder.Dismiss().WithButton("دوباره امتحان کنید", button => { });
                }
            }

            //Add Data
            else if (NotificationKEY.Equals(AppVariable.Add_Data_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    builder.Accent(AppVariable.GREEN);
                    builder.HasBadge("اطلاعیه");
                    builder.HasMessage($"{param[1]} {param[0]} با موفقیت ثبت شد");
                    builder.Dismiss().WithButton("باشه", button => { });
                }
                else
                {
                    builder.Accent(AppVariable.RED);
                    builder.HasBadge("هشدار");
                    builder.HasMessage($"ثبت {param[1]} {param[0]} با خطا مواجه شد");
                    builder.Dismiss().WithButton("دوباره امتحان کنید", button => { });
                }
            }

            //Update
            else if (NotificationKEY.Equals(AppVariable.Update_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    builder.Accent(AppVariable.GREEN);
                    builder.HasBadge("اطلاعیه");
                    builder.HasHeader($"نسخه جدید {param[0]} پیدا شد،همین حالا به آخرین نسخه بروزرسانی کنید");
                    builder.WithButton("ارتقا", button =>
                    {
                        System.Diagnostics.Process.Start(param[1]);
                    });
                    builder.Dismiss().WithButton("بیخیال", button => { });
                }
                else
                {
                    builder.Accent(AppVariable.RED);
                    builder.HasBadge("هشدار");
                    builder.HasHeader($"شما از آخرین نسخه {AppVariable.getAppVersion} استفاده می کنید");
                    builder.Dismiss().WithDelay(TimeSpan.FromSeconds(3));
                    builder.Dismiss().WithButton("تایید", button => { });
                }
            }
           
            builder.Queue();
        }
        #endregion "Notification"
    }
}