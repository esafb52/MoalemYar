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
using LiveCharts;
using LiveCharts.Wpf;
namespace MoalemYar.UserControls
{
    /// <summary>
    /// Interaction logic for Achievement.xaml
    /// </summary>
    public partial class Achievement : UserControl
    {
        public Achievement()
        {
            InitializeComponent();
            for (int i = 0; i < 7; i++)
            {
                MaterialChart _addUser;
                Control _currentUser;
                _addUser = new MaterialChart("کتاب" + i, "مهدی",new string[] { "ok","no", "ok", "no", "ok", "no" },new double[] { 10,20,100,60 }, new ColumnSeries { },AppVariable.GetBrush(AppVariable.CHART_GREEN));
                _currentUser = _addUser;
                waterfallFlow.Children.Add(_currentUser);
            }
        }
    }
}
