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
