﻿/****************************** ghost1372.github.io ******************************\
*	Module Name:	Circular.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 11:27 ب.ظ
*
***********************************************************************************/

using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Circular.xaml
    /// </summary>
    public partial class CircularView : UserControl
    {
        private bool Limited = false;
        private System.Collections.Generic.List<DelegationLink> myClass;

        public CircularView()
        {
            InitializeComponent();
        }

        private void prgUpdate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (prgUpdate.Value == prgUpdate.Maximum)
            {
                btnStart.IsEnabled = true;
                MainWindow.main.showGrowlNotification(AppVariable.Recived_Circular_KEY, true);
                txtSearch.IsEnabled = true;
            }
        }

        private void CalculateMyOperation()
        {
            try
            {
                using (WebClient webClient = new WebClientWithTimeout())
                {
                    webClient.Headers.Add("User-Agent: Other");
                    var page = webClient.DownloadString(FindElement.Settings.DefaultServer ?? AppVariable.DefaultServer2);
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(page);
                    var query = doc.DocumentNode.SelectNodes("//table[@class='table table-striped table-hover']/tbody/tr")
                .Select(r =>
                {
                    var linkNode = r.Descendants("a").Select(node => node.Attributes["href"].Value).ToArray();
                    return new DelegationLink()
                    {
                        Row = r.SelectSingleNode(".//td").InnerText,
                        link = FindElement.Settings.DefaultServer + linkNode.FirstOrDefault(),
                        Category = r.SelectSingleNode(".//td[2]").InnerText,
                        Title = r.SelectSingleNode(".//td[3]").InnerText,
                        Date = r.SelectSingleNode(".//td[4]").InnerText,
                        Type = r.SelectSingleNode(".//td[5]").InnerText,
                        SubType = r.SelectSingleNode(".//td[6]").InnerText
                    };
                }
                ).ToList();
                    var parsedValues = query.Take(Limited ? 20 : query.Count).ToList();
                    myClass = parsedValues;
                    int currentIndex = 0;
                    Dispatcher.Invoke(() =>
                    {
                        lst.Items.Clear();
                        btnStart.IsEnabled = false;
                        prgLoading.Visibility = Visibility.Hidden;
                        prgUpdate.Visibility = Visibility.Visible;
                    });
                    foreach (var i in parsedValues)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            currentIndex += 1;
                            lst.Items.Add(i);

                            prgUpdate.Value = (Convert.ToDouble(((double)currentIndex / (double)parsedValues.Count).ToString("N2")) * 100);
                        }), DispatcherPriority.Background);
                    }
                }
            }
            catch (ArgumentNullException) { }
            catch (WebException)
            {
                Dispatcher.BeginInvoke(new Action(() =>{ MainWindow.main.showGrowlNotification(AppVariable.Recived_Circular_KEY, false);}),DispatcherPriority.Background);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() => CalculateMyOperation());
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            UserControl_Loaded(null, null);
        }

        private void swLimit_Checked(object sender, RoutedEventArgs e)
        {
            if (swLimit.IsChecked == true)
                Limited = true;
            else
                Limited = false;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dynamic selectedItem = lst.SelectedItems[0];
                string row = selectedItem.Row;
                string title = selectedItem.Title;
                string Dlink = selectedItem.link;

                if (!System.IO.Directory.Exists(AppVariable.fileNameBakhsh + row + title))
                {
                    prgLoading.Visibility = Visibility.Visible;
                    prgUpdate.Visibility = Visibility.Hidden;

                    WebClient wc = new WebClient();
                    wc.Headers.Add("User-Agent: Other");
                    using (WebClient client = new WebClient())
                    {
                        client.Headers.Add("User-Agent: Other");
                        if (Dlink.Contains("//portal/"))
                            Dlink = Dlink.Replace("//portal/", "/portal/");
                        Uri ur = new Uri(Dlink);
                        var data = wc.DownloadData(Dlink);

                        string fileExt = "";
                        if (!String.IsNullOrEmpty(wc.ResponseHeaders["Content-Disposition"]))
                        {
                            fileExt = System.IO.Path.GetExtension(wc.ResponseHeaders["Content-Disposition"].Substring(wc.ResponseHeaders["Content-Disposition"].IndexOf("filename=") + 9).Replace("\"", ""));
                        }

                        //client.DownloadProgressChanged += (o, ex) =>
                        //{
                        //    Dispatcher.Invoke(() =>
                        //    {
                        //        //Todo: mybe add progressbar
                        //    });
                        //};

                        string TotPath = AppVariable.fileNameBakhsh + row + title;
                        string dPath = string.Empty;

                        if (fileExt.Equals(".rar") || fileExt.Equals(".zip"))
                        {
                            dPath = AppVariable.fileNameBakhsh + @"\" + row + title + fileExt;
                        }
                        else
                        {
                            if (!System.IO.Directory.Exists(TotPath))
                                System.IO.Directory.CreateDirectory(TotPath);
                            dPath = TotPath + @"\" + row + title + fileExt;
                        }

                        client.DownloadFileAsync(ur, dPath);

                        client.DownloadFileCompleted += (o, ex) =>
                        {
                            prgLoading.Visibility = Visibility.Hidden;
                            prgUpdate.Visibility = Visibility.Visible;

                            if (fileExt.Equals(".rar") || fileExt.Equals(".zip"))
                            {
                                UnCompress(AppVariable.fileNameBakhsh + @"\" + row + title + fileExt, AppVariable.fileNameBakhsh + @"\" + row + title, fileExt);
                            }

                            try
                            {
                                System.Diagnostics.Process.Start(AppVariable.fileNameBakhsh + row + title);
                            }
                            catch (Win32Exception)
                            {
                                System.Diagnostics.Process.Start(AppVariable.fileNameBakhsh + title);
                            }
                            catch (FileNotFoundException)
                            {
                            }
                        };
                    }
                }
                else
                {
                    try
                    {
                        System.Diagnostics.Process.Start(AppVariable.fileNameBakhsh + row + title);
                    }
                    catch (Win32Exception)
                    {
                        System.Diagnostics.Process.Start(AppVariable.fileNameBakhsh + title);
                    }
                    catch (FileNotFoundException)
                    {
                    }
                }
            }
            catch (Exception)
            {
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

        private void lst_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            btnSave_Click(null, null);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            lst.Items.Clear();
            var parsedValues = myClass.Where(x => x.Title.Contains(txtSearch.Text) || x.Date.Contains(txtSearch.Text));
            foreach (var item in parsedValues)
            {
                lst.Items.Add(item);
            }
        }
    }
}

public class WebClientWithTimeout : WebClient
{
    protected override WebRequest GetWebRequest(Uri address)
    {
        WebRequest wr = base.GetWebRequest(address);
        wr.Timeout = 20000; // timeout in milliseconds (ms)
        return wr;
    }
}

public class DelegationLink
{
    public string Row { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public string Date { get; set; }
    public string Type { get; set; }
    public string SubType { get; set; }
    public string link { get; set; }
}