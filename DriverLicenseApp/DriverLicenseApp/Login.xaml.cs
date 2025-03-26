using System.Data;
using System.Security.Cryptography;
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
using DriverLicenseApp.DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        UserService service = new();
        public Login()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            // 2. Kiểm tra mật khẩu hợp lệ không
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            User? user = AuthenticateUser(email, password);
            if (user != null)
            {
                OpenDashboard(user);
            }
            else
            {
                MessageBox.Show("Invalid email or password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private User? AuthenticateUser(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            LicenseDriverDbContext context = new();
            var user = context.Users
                .Where(u => u.Email == email && u.Password == hashedPassword)
                .FirstOrDefault();

            return user;
        }

        private void OpenDashboard(User user)
        {
            Window dashboard = new DashBoard(user);

            if (dashboard != null)
            {
                dashboard.Show();
                this.Close();
            }
        }


        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        private void ToggleRegisterMode(object sender, MouseButtonEventArgs e)
        {
            OpenWindow(new Register());
        }
        private void OpenWindow(Window window)
        {
            this.Hide(); // Ẩn 
            window.ShowDialog(); // Mở màn hình con và chờ đóng
        }
    }
}