
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

namespace MoalemYar
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public string appTitle { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            appTitle = AppVariable.getAppTitle + AppVariable.getAppVersion; // App Title with Version

           // ShowCredentialDialog();

            #region "Automatic error reporting"

            LogifyAlert client = LogifyAlert.Instance;
            client.ApiKey = AppVariable.LogifyAPIKey;
            client.AppName = AppVariable.getAppName;
            client.AppVersion = AppVariable.getAppVersion;
            client.OfflineReportsEnabled = true;
            client.OfflineReportsCount = 20;
            client.OfflineReportsDirectory = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            client.SendOfflineReports();
            client.StartExceptionsHandling();

            #endregion
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
                        dialog.ConfirmCredentials(true);
                    else
                    {
                        dialog.ConfirmCredentials(true);
                        Environment.Exit(0);

                    }
                }
                else
                {
                    Environment.Exit(0);
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
        #endregion
    }
}
