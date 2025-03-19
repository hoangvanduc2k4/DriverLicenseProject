﻿using System.Data;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserService service = new();
        private bool isRegistering = false; // Trạng thái đăng ký
        public MainWindow()
        {
            InitializeComponent();
        }
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;

            if (isRegistering) // Nếu đang ở chế độ đăng ký
            {
                string fullName = txtFullName.Text;
                string confirmPassword = txtConfirmPassword.Password;
                int role = Convert.ToInt32(((ComboBoxItem)cbRole.SelectedItem).Tag);


                if (password != confirmPassword)
                {
                    MessageBox.Show("Mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                RegisterUser(fullName, email, password, role);
                MessageBox.Show("Đăng ký thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                isRegistering = false;
                ToggleRegisterMode(null, null);
            }
            else // Nếu đang ở chế độ đăng nhập
            {
                string role = AuthenticateUser(email, password);
                if (!string.IsNullOrEmpty(role))
                {
                    OpenDashboard(role);
                }
                else
                {
                    MessageBox.Show("Sai tài khoản hoặc mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        private void LoginUser()
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string role = AuthenticateUser(email, password);

            if (!string.IsNullOrEmpty(role))
            {
                OpenDashboard(role);
            }
            else
            {
                MessageBox.Show("Sai tài khoản hoặc mật khẩu.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string AuthenticateUser(string email, string password)
        {
            string hashedPassword = HashPassword(password);
            LicenseDriverDbContext context = new();
            var user = context.Users
                .Where(u => u.Email == email && u.Password == hashedPassword)
                .Select(u => u.Role)
                .FirstOrDefault();

            return user switch
            {
                1 => "Student",
                2 => "Teacher",
                3 => "TrafficPolice",
                4 => "Admin",
                _ => string.Empty
            };
        }

        private void OpenDashboard(string role)
        {
            Window dashboard = new DashBoard(role);

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

        // ========== XỬ LÝ ĐĂNG KÝ ==========
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
            ToggleRegisterMode(null, null); // Quay lại màn hình đăng nhập
        }

        private void ToggleRegisterMode(object sender, MouseButtonEventArgs e)
        {
            isRegistering = !isRegistering;
            if (isRegistering)
            {
                FullNameLabel.Visibility = Visibility.Visible;
                txtFullName.Visibility = Visibility.Visible;
            }
            else
            {
                FullNameLabel.Visibility = Visibility.Collapsed;
                txtFullName.Visibility = Visibility.Collapsed;
            }
            if (isRegistering)
            {
                TitleText.Text = "Đăng Ký";
                btnLogin.Content = "Đăng ký";
                ConfirmPasswordLabel.Visibility = Visibility.Visible;
                txtConfirmPassword.Visibility = Visibility.Visible;
                RoleLabel.Visibility = Visibility.Visible;
                cbRole.Visibility = Visibility.Visible;
                SwitchModeText.Text = "Đã có tài khoản? ";
                SwitchModeAction.Text = "Đăng nhập";
            }
            else
            {
                TitleText.Text = "Đăng Nhập";
                btnLogin.Content = "Đăng nhập";
                ConfirmPasswordLabel.Visibility = Visibility.Collapsed;
                txtConfirmPassword.Visibility = Visibility.Collapsed;
                RoleLabel.Visibility = Visibility.Collapsed;
                cbRole.Visibility = Visibility.Collapsed;
                SwitchModeText.Text = "Chưa có tài khoản? ";
                SwitchModeAction.Text = "Đăng ký";
            }
        }

    }
}