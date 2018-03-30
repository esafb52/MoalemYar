
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
using System;
using System.Collections.Generic;
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
using DevExpress.Logify.WPF;
using Ookii.Dialogs.Wpf;
using Enterwell.Clients.Wpf.Notifications;
using MoalemYar.UserControls;

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

            //Todo: Enable Credential
            //ShowCredentialDialog();

            LogifyCrashReport();


            LoadSettings();

        }

       
        public void LogifyCrashReport()
        {
            var isEnabledReport = AppVariable.ReadSetting(AppVariable.AutoSendReport);
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

            var hb_Menu = Convert.ToBoolean(AppVariable.ReadSetting(AppVariable.HamburgerMenu));
            MainWindow.main.tab.IconMode = !hb_Menu;
        }
        public void ShowNotification()
        {
            this.Manager
               .CreateMessage()
               .Accent("#1751C3")
               .Background("#333")
               .HasBadge("Info")
               .HasMessage("Update will be installed on next application restart.")
               .Dismiss().WithButton("Update now", button => { tab.SelectedIndex = 2; })
               .Dismiss().WithButton("Release notes", button => { })
               .Dismiss().WithButton("Later", button => { })
               .WithOverlay(new ProgressBar
               {
                   VerticalAlignment = VerticalAlignment.Bottom,
                   HorizontalAlignment = HorizontalAlignment.Stretch,
                   Height = 3,
                   BorderThickness = new Thickness(0),
                   Foreground = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255)),
                   Background = Brushes.Transparent,
                   IsIndeterminate = false,
                   IsHitTestVisible = false,
                   Value = 18
               })
               .Queue();
        }
        public void RestartNotification()
        {
            this.Manager
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
                 .Queue();
        }
        public void ShowUpdateNotification(bool isAvailable, string Version, string URL)
        {
            if (isAvailable)
            {
                this.Manager
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
                    .Queue();
            }
            else
            {
                this.Manager
                   .CreateMessage()
                   .Accent(AppVariable.RED)
                   .Background(AppVariable.BGBLACK)
                   .HasBadge("هشدار")
                   .HasHeader(string.Format("شما از آخرین نسخه {0} استفاده می کنید", AppVariable.getAppVersion))
                   .Dismiss().WithButton("تایید", button => { })
                   .Queue();
            }

        }
        //Todo: Add Login
        private void ShowCredentialDialog()
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

                if (dialog.ShowDialog(this))
                {
                    if (dialog.Credentials.UserName == "mahdi" && dialog.Credentials.Password == "123")
                        dialog.MainInstruction = "رمز عبور اشتباه است";

                    //else
                    //{
                    //    dialog.ConfirmCredentials(true);
                    //    Environment.Exit(0);

                    //}
                }
                //else
                //{
                //    Environment.Exit(0);
                //}
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
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowNotification();

        }

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
    }
}
