
/****************************** ghost1372.github.io ******************************\
*	Module Name:	MaterialCircular.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 2, 01:57 ب.ظ
*	
***********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for MaterialCircular.xaml
    /// </summary>
    public partial class MaterialCircular : UserControl
    {
        public Brush BorderColor { get; set; }
        string Dlink;
        public MaterialCircular(string Row ,string Title, string Category, string Type, string SubType, string Date, string Link, Brush Background)
        {
            InitializeComponent();
            DataContext = this;
            Dlink = Link;
            BorderColor = Background;
            txtCategory.Text = Category;
            txtDate.Text = Date;
            txtSubType.Text = SubType;
            txtTitle.Text = Title;
            txtType.Text = Type;
            txtRow.Text = Row;

            if (!System.IO.Directory.Exists(AppVariable.fileNameBakhsh + txtRow.Text + txtTitle.Text))
            {
                btnOpen.IsEnabled = false;
                btnSave.IsEnabled = true;
            }
            else
            {
                btnOpen.IsEnabled = true;
                btnSave.IsEnabled = false;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            prgDownload.Visibility = Visibility.Visible;
            WebClient wc = new WebClient();
                   
            using (WebClient client = new WebClient())
            {
                Uri ur = new Uri(Dlink);
                var data = wc.DownloadData(Dlink);

                string fileExt = "";
                if (!String.IsNullOrEmpty(wc.ResponseHeaders["Content-Disposition"]))
                {
                    fileExt = System.IO.Path.GetExtension(wc.ResponseHeaders["Content-Disposition"].Substring(wc.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", ""));
                }
                
                client.DownloadProgressChanged += (o, ex) =>
                {
                    // updating the UI
                    Dispatcher.Invoke(() =>
                    {
                        prgDownload.Value = ex.ProgressPercentage;
                    });
                };
                client.DownloadFileCompleted += (o, ex) =>
                {
                    btnSave.IsEnabled = false;
                    btnOpen.IsEnabled = true;
                    prgDownload.Visibility = Visibility.Hidden;

                };
                
                string TotPath = AppVariable.fileNameBakhsh + txtRow.Text + txtTitle.Text;
                if(!System.IO.Directory.Exists(TotPath))
                    System.IO.Directory.CreateDirectory(TotPath);
                client.DownloadFileAsync(ur, TotPath + @"\" + txtRow.Text + txtTitle.Text + fileExt);

            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(AppVariable.fileNameBakhsh + txtRow.Text + txtTitle.Text);
        }
    }
}
