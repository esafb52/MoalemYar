using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for ChartTemplate.xaml
    /// </summary>
    public partial class ChartTemplate : UserControl
    {
        public ChartTemplate(string Book, string Name, List<DataClass.DataTransferObjects.myChartTemplate> templates, string Average, string AverageStatus)
        {
            InitializeComponent();

            chart.Caption = AverageStatus + Environment.NewLine + "میانگین: " + Average;
            chartColumn.ChartTitle = Book;
            chartColumn.ChartSubTitle = Name;
            chart.ItemsSource = templates;
        }
    }
}