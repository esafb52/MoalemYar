/****************************** ghost1372.github.io ******************************\
*	Module Name:	Circular.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 1, 11:27 ب.ظ
*
***********************************************************************************/

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Circular.xaml
    /// </summary>
    public partial class CircularView : UserControl
    {
        private System.Collections.Generic.List<DelegationLink> myClass;
        private bool Permission = true;
        private bool Limited = false;

        public CircularView()
        {
            InitializeComponent();
        }

        private void prgUpdate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (prgUpdate.Value == prgUpdate.Maximum)
                MainWindow.main.ShowRecivedCircularNotification(true);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke<Task>(async () =>
        {
            waterfallFlow.Children.Clear();
            MaterialCircular _addUser;
            Control _currentUser;
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
                    var linkNode2 = r.SelectSingleNode("th|td");
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
                    foreach (var item in parsedValues)
                    {
                        currentIndex += 1;
                        if (!Permission)
                            return;

                        await Task.Delay(10);
                        //Todo: progressbar not update
                        prgUpdate.Value = (((currentIndex) / parsedValues.Count) * 100);
                        _addUser = new MaterialCircular(item.Row, item.Title, item.Category, item.Type, item.SubType, item.Date, item.link);
                        _currentUser = _addUser;
                        waterfallFlow.Children.Add(_currentUser);
                    }
                    if (prgUpdate.Value == 100)
                    {
                        Permission = false;
                        txtStop.Text = "دریافت";
                        Style style = this.FindResource("ButtonPrimary") as Style;
                        btnStop.Style = style;
                        img.Source = new BitmapImage(new Uri("pack://application:,,,/MoalemYar;component/Resources/start.png", UriKind.Absolute));
                        txtSearch.IsEnabled = true;
                    }
                }
            }
            catch (WebException)
            {
                MainWindow.main.ShowRecivedCircularNotification(false);
            }
        }, DispatcherPriority.ContextIdle);
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Permission = false;
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (Permission)
            {
                Permission = false;
                txtStop.Text = "دریافت";
                Style style = this.FindResource("ButtonPrimary") as Style;
                btnStop.Style = style;
                img.Source = new BitmapImage(new Uri("pack://application:,,,/MoalemYar;component/Resources/start.png", UriKind.Absolute));
            }
            else
            {
                Permission = true;
                txtStop.Text = "توقف";
                Style style = this.FindResource("ButtonDanger") as Style;
                btnStop.Style = style;
                img.Source = new BitmapImage(new Uri("pack://application:,,,/MoalemYar;component/Resources/stop.png", UriKind.Absolute));
                UserControl_Loaded(null, null);
            }
        }
        private void MetroTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Dispatcher.Invoke<Task>(async () =>
            {
                waterfallFlow.Children.Clear();
                MaterialCircular _addUser;
                Control _currentUser;
                var parsedValues = myClass.Where(x => x.Title.Contains(txtSearch.Text) || x.Date.Contains(txtSearch.Text));
                foreach (var item in parsedValues)
                {
                    await Task.Delay(10);
                    _addUser = new MaterialCircular(item.Row, item.Title, item.Category, item.Type, item.SubType, item.Date, item.link);
                    _currentUser = _addUser;
                    waterfallFlow.Children.Add(_currentUser);
                }
            }, DispatcherPriority.ContextIdle);
        }

        private void swLimit_Checked(object sender, RoutedEventArgs e)
        {
            if (swLimit.IsChecked == true)
                Limited = true;
            else
                Limited = false;
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