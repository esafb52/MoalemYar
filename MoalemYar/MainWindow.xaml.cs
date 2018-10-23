/****************************** ghost1372.github.io ******************************\
*	Module Name:	MainWindow.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 22, 05:53 ب.ظ
*
***********************************************************************************/

using HandyControl.Controls;
using MoalemYar.UserControls;
using MVVMC;
using System;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            main = this;

            loadSettings();
            ShowCredentialDialog();
            Growl.SetFlowDirection(PanelMessage, FlowDirection.RightToLeft);
            Growl.SetGrowlPanel(PanelMessage);
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

        private void loadSettings()
        {
            if (FindElement.Settings.IsMaximize)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
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
                    AllowsTransparency = true,
                    WindowStyle = WindowStyle.None
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

                popupLogin.PopupElement = mainStack;
                popupLogin.ShowDialog();
            }
        }

        #region "Notification"

        public void showGrowlNotification(string NotificationKEY, bool isAvailableOrSuccess = false, params string[] param)
        {
            //Delete Confirm
            if (NotificationKEY.Equals(AppVariable.Delete_Confirm_KEY))
            {
                Growl.Ask($"آیا برای حذف {param[1]} {param[0]} اطمینان دارید؟", isConfirm =>
                {
                    if (isConfirm)
                    {
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
                    }
                    return true;
                });
            }
            //Reset Data Confirm
            else if (NotificationKEY.Equals(AppVariable.Reset_Data_Confirm_KEY))
            {
                Growl.Ask($"آیا برای بازیابی {param[0]} اطمینان دارید؟", isConfirm =>
                {
                    if (isConfirm)
                    {
                        if (param[0] == "تنظیمات برنامه")
                            SettingsView.main.resetConfig();
                        else
                            SettingsView.main.resetDatabase();
                    }
                    return true;
                });
            }

            //Reset Data Deleted
            else if (NotificationKEY.Equals(AppVariable.Data_Reset_Deleted_KEY))
            {
                //Todo: change button text to راه اندازی
                Growl.Ask($"{param[0]} به حالت پیشفرض تغییر یافت، برنامه را دوباره راه اندازی کنید", isConfirm =>
                {
                    if (isConfirm)
                    {
                        Application.Current.Shutdown();
                        System.Windows.Forms.Application.Restart();
                    }
                    return true;
                });
            }

            //Password Same
            else if (NotificationKEY.Equals(AppVariable.Same_Password_KEY))
            {
                Growl.Warning("رمز های عبور باید یکسان باشند");
            }

            //Delete Exist
            else if (NotificationKEY.Equals(AppVariable.Delete_Exist_KEY))
            {
                Growl.Warning($"نمی توان این {param[0]} را حذف کرد، ابتدا {param[1]} این {param[0]} را حذف کنید");
            }

            //Azmon
            else if (NotificationKEY.Equals(AppVariable.Azmon_KEY))
            {
                Growl.Warning("تعداد سوالات وارد شده بیشتر از سوالات موجود است");
            }

            //Fill All Data
            else if (NotificationKEY.Equals(AppVariable.Fill_All_Data_KEY))
            {
                Growl.Warning("لطفا تمام فیلدها را پر کنید");
            }

            //No Data
            else if (NotificationKEY.Equals(AppVariable.No_Data_KEY))
            {
                var navigationService = NavigationServiceProvider.GetNavigationServiceInstance();

                //Todo: change button text to ثبت اطلاعات جدید
                Growl.Ask("اطلاعاتی در پایگاه داده یافت نشد", isConfirm =>
                {
                    if (isConfirm)
                    {
                        switch (param[0])
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
                    }
                    return true;
                });
            }

            //Backup
            else if (NotificationKEY.Equals(AppVariable.Backup_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    Growl.Success($"{param[0]} با موفقیت انجام شد");

                }
                else
                {
                    Growl.Error($"{param[0]} با مشکل مواجه شد");
                }
            }

            //Circular
            else if (NotificationKEY.Equals(AppVariable.Recived_Circular_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    Growl.Success("تمامی بخشنامه ها با موفقیت دریافت شد");
                }
                else
                {
                    Growl.Error("درحال حاظر سرور در دسترس نیست! لطفا در صورت فعال بودن، VPN خود را غیرفعال کنید");
                }
            }

            //Update Data
            else if (NotificationKEY.Equals(AppVariable.Update_Data_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    Growl.Success($"{param[1]} {param[0]} با موفقیت ویرایش شد");
                }
                else
                {
                    Growl.Error($"ویرایش {param[1]} {param[0]} با خطا مواجه شد");
                }
            }

            //Deleted
            else if (NotificationKEY.Equals(AppVariable.Deleted_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    Growl.Success($"{param[1]} {param[0]} با موفقیت حذف شد");
                }
                else
                {
                    Growl.Error($"حذف {param[1]} {param[0]} با خطا مواجه شد");
                }
            }

            //Add Data
            else if (NotificationKEY.Equals(AppVariable.Add_Data_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    Growl.Success($"{param[1]} {param[0]} با موفقیت ثبت شد");
                }
                else
                {
                    Growl.Error($"ثبت {param[1]} {param[0]} با خطا مواجه شد");
                }
            }

            //Update
            else if (NotificationKEY.Equals(AppVariable.Update_KEY))
            {
                if (isAvailableOrSuccess)
                {
                    Growl.Ask($"نسخه جدید {param[0]} پیدا شد،همین حالا به آخرین نسخه بروزرسانی کنید", isClosed =>
                    {
                        if (isClosed)
                            System.Diagnostics.Process.Start(param[1]);

                        return true;
                    });
                }
                else
                {
                    Growl.Info($"شما از آخرین نسخه {AppVariable.getAppVersion} استفاده می کنید");
                }
            }
        }

        #endregion "Notification"
    }
}