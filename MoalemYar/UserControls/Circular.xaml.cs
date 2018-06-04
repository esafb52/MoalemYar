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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using System.Windows.Threading;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Circular.xaml
    /// </summary>
    public partial class Circular : UserControl
    {
        public Circular()
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
                    WebClient webClient = new WebClientWithTimeout();
                    var page = webClient.DownloadString(FindElement.Settings[AppVariable.DefaultServer].ToString() ?? AppVariable.DefaultServer2);
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(page);

                    var parsedValues = doc.DocumentNode.SelectNodes("//table[@class='table table-striped table-hover']/tr").Skip(1)
                         .Select(r =>
                         {
                             var linkNode = r.SelectSingleNode(".//a");
                             var linkNode2 = r.SelectSingleNode("th|td");
                             return new DelegationLink()
                             {
                                 Row = r.SelectSingleNode(".//td").InnerText,
                                 link = linkNode.GetAttributeValue("href", ""),
                                 Category = r.SelectSingleNode(".//td[2]").InnerText,
                                 Title = r.SelectSingleNode(".//td[3]").InnerText,
                                 Date = r.SelectSingleNode(".//td[4]").InnerText,
                                 Type = r.SelectSingleNode(".//td[5]").InnerText,
                                 SubType = r.SelectSingleNode(".//td[6]").InnerText,
                             };
                         }
                         ).ToList();
                    prgUpdate.Maximum = parsedValues.Count;
                    foreach (var item in parsedValues)
                    {
                        await Task.Delay(100);
                        prgUpdate.Value += 1;
                        prgUpdate.Hint = ((prgUpdate.Value * 100) / parsedValues.Count).ToString("0");
                        _addUser = new MaterialCircular(item.Row, item.Title, item.Category, item.Type, item.SubType, item.Date, item.link, AppVariable.GetBrush(Convert.ToString(FindElement.Settings[AppVariable.ChartColor] ?? AppVariable.CHART_GREEN)));
                        _currentUser = _addUser;
                        waterfallFlow.Children.Add(_currentUser);
                        waterfallFlow.Refresh();
                    }
                }
                catch (WebException)
                {
                    MainWindow.main.ShowRecivedCircularNotification(false);
                }
            }, DispatcherPriority.ContextIdle);

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