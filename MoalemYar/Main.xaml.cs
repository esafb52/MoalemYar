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
using System.Windows.Shapes;
using HandyControl.Controls;
using MVVMC;

namespace MoalemYar
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : WindowBorderless
    {
        public Main()
        {
            InitializeComponent();
        }

        private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
        {
            var navigationService = NavigationServiceProvider.GetNavigationServiceInstance();
            var selectedItem = sender as TreeViewItem;
            switch (selectedItem.Tag)
            {
                case "initial":
                    navigationService.GetController<UserControls.UserControlsController>().Initial();
                    break;
                case "addOrUpdateSchool":
                    navigationService.GetController<UserControls.UserControlsController>().AddSchool();
                    break;
                case "exAddOrUpdateStudent":
                    navigationService.GetController<UserControls.UserControlsController>().AddStudent();
                    break;
                case "exAddOrUpdateUser":
                    navigationService.GetController<UserControls.UserControlsController>().AddUser();
                    break;
                case "exAttendancelist":
                    navigationService.GetController<UserControls.UserControlsController>().Attendancelist();
                    break;
                case "exQuestionsList":
                    navigationService.GetController<UserControls.UserControlsController>().Questionslist();
                    break;
                case "exTopStudents":
                    navigationService.GetController<UserControls.UserControlsController>().TopStudents();
                    break;
                case "exAchievement":
                    navigationService.GetController<UserControls.UserControlsController>().Achievement();
                    break;
                case "exCircular":
                    navigationService.GetController<UserControls.UserControlsController>().Circular();
                    break;
                case "exBook":
                    navigationService.GetController<UserControls.UserControlsController>().Books();
                    break;
                case "exRoshd":
                    navigationService.GetController<UserControls.UserControlsController>().Magazine();
                    break;
            }
        }
    }
}
