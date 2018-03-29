
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

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : UserControl
    {
        public string History { get; set; }
        public string AppName { get; set; }
        public string AppVersion { get; set; }


        public Brush TimeColor { get; set; }

        public About()
        {
            InitializeComponent();
            this.DataContext = this;

            History = Properties.Resources.History;
            AppName = AppVariable.getAppTitle;
            AppVersion = AppVariable.getAppVersion;
            var color = (Color)ColorConverter.ConvertFromString(AppVariable.ReadSetting(AppVariable.SkinCode));
            var brush = new SolidColorBrush(color);
            TimeColor = brush;
        }
        
        public void ReadHistory()
        {
            //txtHistory.Text = History;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
