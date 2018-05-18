using LiveCharts;
using LiveCharts.Wpf;
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
    /// Interaction logic for MaterialChart.xaml
    /// </summary>
    public partial class MaterialChart : UserControl
    {
        public Brush BorderColor { get; set; }
        public MaterialChart(string Book, string Name, string[] Label, double[] values, Series series, Brush Background)
        {
            InitializeComponent();
            DataContext = this;
            BorderColor = Background;
            if (series.GetType() == typeof(ColumnSeries))
            {
                AchievementChart.Series.Add(new ColumnSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }else if (series.GetType() == typeof(LineSeries))
            {
                AchievementChart.Series.Add(new LineSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            else if (series.GetType() == typeof(StackedAreaSeries))
            {
                AchievementChart.Series.Add(new StackedAreaSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            else if (series.GetType() == typeof(StackedColumnSeries))
            {
                AchievementChart.Series.Add(new StackedColumnSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            else if (series.GetType() == typeof(StepLineSeries))
            {
                AchievementChart.Series.Add(new StepLineSeries
                {
                    Values = new ChartValues<double>(values),
                    StrokeDashArray = new System.Windows.Media.DoubleCollection(20)
                });
            }
            AchievementChart.AxisX.Add(new Axis
            {
                Labels = Label,
                Separator = new LiveCharts.Wpf.Separator { }
            });
            txtBook.Text = Book;
            txtName.Text = Name;
        }
    }
}
