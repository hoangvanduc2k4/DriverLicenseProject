using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DriverLicenseApp.BLL.Service;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserService service = new();
        public MainWindow()
        {
            InitializeComponent();
            var users = service.GetAllUsers();
            foreach (var user in users)
            {
                MessageBox.Show($"{user.Email}");
            }
        }
    }
}