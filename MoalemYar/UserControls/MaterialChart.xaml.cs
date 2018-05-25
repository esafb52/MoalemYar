using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;
using System.Windows.Media;

namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for MaterialChart.xaml
    /// </summary>
    public partial class MaterialChart : UserControl
    {
        public Brush BorderColor { get; set; }

        public MaterialChart(string Book, string Name, string[] Label, double[] values, string Average, string AverageStatus, Series series, Brush Background)
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
            }
            else if (series.GetType() == typeof(LineSeries))
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
            txtAverageDouble.Text = Average;
            txtAverage.Text = AverageStatus;
            txtBook.Text = Book;
            txtName.Text = Name;
        }
    }
}