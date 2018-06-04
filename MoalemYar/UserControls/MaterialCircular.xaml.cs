/****************************** ghost1372.github.io ******************************\
*	Module Name:	MaterialCircular.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 2, 01:57 ب.ظ
*
***********************************************************************************/

using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Readers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for MaterialCircular.xaml
    /// </summary>
    public partial class MaterialCircular : UserControl
    {
        public Brush BorderColor { get; set; }
        private string Dlink;

        public MaterialCircular(string Row, string Title, string Category, string Type, string SubType, string Date, string Link, Brush Background)
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
                    Dispatcher.Invoke(() =>
                    {
                        prgDownload.Value = ex.ProgressPercentage;
                    });
                };

                string TotPath = AppVariable.fileNameBakhsh + txtRow.Text + txtTitle.Text;
                string dPath = string.Empty;

                if (fileExt.Equals(".rar") || fileExt.Equals(".zip"))
                {
                    dPath = AppVariable.fileNameBakhsh + @"\" + txtRow.Text + txtTitle.Text + fileExt;
                }
                else
                {
                    if (!System.IO.Directory.Exists(TotPath))
                        System.IO.Directory.CreateDirectory(TotPath);
                    dPath = TotPath + @"\" + txtRow.Text + txtTitle.Text + fileExt;
                }

                client.DownloadFileAsync(ur, dPath);

                client.DownloadFileCompleted += (o, ex) =>
                {
                    btnSave.IsEnabled = false;
                    btnOpen.IsEnabled = true;
                    prgDownload.Visibility = Visibility.Hidden;

                    if (fileExt.Equals(".rar") || fileExt.Equals(".zip"))
                    {
                        UnCompress(AppVariable.fileNameBakhsh + @"\" + txtRow.Text + txtTitle.Text + fileExt, AppVariable.fileNameBakhsh + @"\" + txtRow.Text + txtTitle.Text, fileExt);
                    }
                };
            }
        }
        public void UnCompress(string Open, string Write, string FileExt)
        {
            try
            {
                if (FileExt.Equals(".rar"))
                {
                    using (var archive = RarArchive.Open(Open))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            entry.WriteToDirectory(Write, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
                else
                {
                    using (var archive = ZipArchive.Open(Open))
                    {
                        foreach (var entry in archive.Entries.Where(entry => !entry.IsDirectory))
                        {
                            entry.WriteToDirectory(Write, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true
                            });
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                System.IO.File.Delete(Open);
            };
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(AppVariable.fileNameBakhsh + txtRow.Text + txtTitle.Text);
            }
            catch (Win32Exception)
            {
                System.Diagnostics.Process.Start(AppVariable.fileNameBakhsh +  txtTitle.Text);
            }
            catch (FileNotFoundException)
            {
            }
        }
    }
}