using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        public Register()
        {
            InitializeComponent();
        }
        private void RegisterUser(string fullName, string email, string password, int role)
        {
            string hashedPassword = HashPassword(password);
            using LicenseDriverDbContext context = new();

            var newUser = new User
            {
                FullName = fullName,
                Email = email,
                Password = hashedPassword,
                Role = role
            };
            context.Users.Add(newUser);
            context.SaveChanges();

            MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

        }


        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string fullName = txtFullName.Text;
            string confirmPassword = txtConfirmPassword.Password;
            int role = Convert.ToInt32(((ComboBoxItem)cbRole.SelectedItem).Tag);

            if (password != confirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            RegisterUser(fullName, email, password, role);
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        private void ToggleLoginMode(object sender, MouseButtonEventArgs e)
        {
            this.Hide(); // Ẩn 
            Window login = new Login();
            login.ShowDialog();
        }
    }
}
