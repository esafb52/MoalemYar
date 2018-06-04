/****************************** ghost1372.github.io ******************************\
*	Module Name:	Dashboard.xaml.cs
*	Project:		MoalemYar
*	Copyright (C) 2017 Mahdi Hosseini, All rights reserved.
*	This software may be modified and distributed under the terms of the MIT license.  See LICENSE file for details.
*
*	Written by Mahdi Hosseini <Mahdidvb72@gmail.com>,  2018, 6, 3, 03:23 ب.ظ
*
***********************************************************************************/

using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : UserControl
    {
        public Brush BorderColor { get; set; }

        public Dashboard()
        {
            InitializeComponent();

            DataContext = this;
            BorderColor = AppVariable.GetBrush(FindElement.Settings.ChartColor ?? AppVariable.CHART_GREEN);
            AchievementChart.Series.Add(new LineSeries
            {
                Values = new ChartValues<double>(new double[] { 10, 25, 65, 57, 15, 70 }),
                StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
            });
            AchievementChart.AxisX.Add(new Axis
            {
                Labels = new string[] { "Item", "Item", "Item", "Item", "Item", "Item" },
                Separator = new LiveCharts.Wpf.Separator { }
            });
        }
    }
}