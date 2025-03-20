using DriverLicenseApp.BLL.Service;
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

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for ListUser.xaml
    /// </summary>
    public partial class ListUser : Window
    {
        private UserService _userService;

        public ListUser()
        {
            InitializeComponent();
            _userService = new UserService();
            LoadUsers();

        }

        private void LoadUsers()
        {
            var users = _userService.GetAllUsers();
            UserDataGrid.ItemsSource = users;
        }
    }
}
