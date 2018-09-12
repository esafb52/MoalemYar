/****************************** ghost1372.github.io ******************************\
*	Module Name:	About.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 3, 28, 04:30 ب.ظ
*
***********************************************************************************/

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Linq;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutView : UserControl
    {
        //XmlTextReader reader;
        private string newVersion = string.Empty;

        private string ChangeLog = string.Empty;
        private string url = "";

        public AboutView()
        {
            InitializeComponent();
            DataContext = this;
            txtHistory.Text = Properties.Resources.History;
            AppName.Content = AppVariable.getAppTitle;
            AppVersion.Content = AppVariable.getAppVersion;
        }

        private void CheckUpdate()
        {
            try
            {
                newVersion = string.Empty;
                ChangeLog = string.Empty;
                url = "";

                XDocument doc = XDocument.Load(AppVariable.UpdateServer);
                var items = doc
                    .Element(XName.Get(AppVariable.UpdateXmlTag))
                    .Elements(XName.Get(AppVariable.UpdateXmlChildTag));
                var versionItem = items.Select(ele => ele.Element(XName.Get(AppVariable.UpdateVersionTag)).Value);
                var urlItem = items.Select(ele => ele.Element(XName.Get(AppVariable.UpdateUrlTag)).Value);
                var changelogItem = items.Select(ele => ele.Element(XName.Get(AppVariable.UpdateChangeLogTag)).Value);

                newVersion = versionItem.FirstOrDefault();
                url = urlItem.FirstOrDefault();
                ChangeLog = changelogItem.FirstOrDefault();
                CompareVersions();
            }
            catch (Exception)
            {
            }
        }

        private void CompareVersions()
        {
            if (IsVersionLater(newVersion, AppVariable.getAppVersion.ToString()))
            {
                txtNewVersion.Content = newVersion;
                txtNewVersion.Foreground = new SolidColorBrush(Colors.Green);
                txtChangeLog.Visibility = Visibility.Visible;
                txtChangeLog.Text = ChangeLog;
                MainWindow.main.ShowUpdateNotification(true, newVersion, url);
            }
            else
            {
                MainWindow.main.ShowUpdateNotification(false, null, null);
                txtNewVersion.Foreground = new SolidColorBrush(Colors.Red);
                txtNewVersion.Content = newVersion;
            }
        }

        public static bool IsVersionLater(string newVersion, string oldVersion)
        {
            // split into groups
            string[] cur = newVersion.Split('.');
            string[] cmp = oldVersion.Split('.');
            // get max length and fill the shorter one with zeros
            int len = Math.Max(cur.Length, cmp.Length);
            int[] vs = new int[len];
            int[] cs = new int[len];
            Array.Clear(vs, 0, len);
            Array.Clear(cs, 0, len);
            int idx = 0;
            // skip non digits
            foreach (string n in cur)
            {
                if (!Int32.TryParse(n, out vs[idx]))
                {
                    vs[idx] = -999; // mark for skip later
                }
                idx++;
            }
            idx = 0;
            foreach (string n in cmp)
            {
                if (!Int32.TryParse(n, out cs[idx]))
                {
                    cs[idx] = -999; // mark for skip later
                }
                idx++;
            }
            for (int i = 0; i < len; i++)
            {
                // skip non digits
                if ((vs[i] == -999) || (cs[i] == -999))
                {
                    continue;
                }
                if (vs[i] < cs[i])
                {
                    return (false);
                }
                else if (vs[i] > cs[i])
                {
                    return (true);
                }
            }
            return (false);
        }

        private void btnCheckUpdate_Click(object sender, RoutedEventArgs e)
        {
            CheckUpdate();
        }
    }
}