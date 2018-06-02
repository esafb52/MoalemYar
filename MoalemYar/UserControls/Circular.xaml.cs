
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

            Dispatcher.Invoke(new Action(() => {
                waterfallFlow.Children.Clear();
                MaterialCircular _addUser;
                Control _currentUser;

                WebClient webClient = new WebClient();
                var page = webClient.DownloadString("http://5743.zanjan.medu.ir/regulation/archive?ocode=100038170");

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
                     ).OrderByDescending(x => x.Row).ToList();
                prgUpdate.Maximum = parsedValues.Count;
                foreach (var item in parsedValues)
                {
                    Task.Delay(200).ContinueWith(ctx => {
                        prgUpdate.Value += 1;
                        prgUpdate.Hint = ((prgUpdate.Value * 100) / parsedValues.Count).ToString("0");
                        _addUser = new MaterialCircular(item.Row, item.Title, item.Category, item.Type, item.SubType, item.Date, AppVariable.GetBrush(Convert.ToString(FindElement.Settings[AppVariable.ChartColor] ?? AppVariable.CHART_GREEN)));
                        _currentUser = _addUser;
                        waterfallFlow.Children.Add(_currentUser);
                        waterfallFlow.Refresh();
                    }, TaskScheduler.FromCurrentSynchronizationContext());

                }
            }), DispatcherPriority.ContextIdle, null);
        }
      
        private void prgUpdate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(prgUpdate.Value == prgUpdate.Maximum)
                MainWindow.main.ShowRecivedCircularNotification();
        }
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