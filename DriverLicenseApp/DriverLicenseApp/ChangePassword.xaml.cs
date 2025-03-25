using System;
using System.Collections.Generic;
using System.Formats.Asn1;
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
using System.Xml.Linq;
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for ChangePassword.xaml
    /// </summary>
    public partial class ChangePassword : Window
    {
        private int _userId;
        private readonly LicenseDriverDbContext context = new LicenseDriverDbContext();
        private User _currentUser;
        private UserService _userService;
        public ChangePassword(int userId)
        {
            InitializeComponent();
            _userService = new UserService();
            _userId = userId;
            _currentUser = _userService.GetUserProfile(_userId);
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }


        private void ChangePass_Click(object sender, RoutedEventArgs e)
        {
            string currentPassword = txtOldPass.Password;
            string newPassword = txtNewPass.Password;
            string confirmPassword = txtConfirmNewPass.Password;

            // 1. Kiểm tra mật khẩu cũ có đúng không
            if (HashPassword(currentPassword) != _currentUser.Password)
            {
                MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Kiểm tra mật khẩu mới hợp lệ không
            if (string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Please enter new password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Kiểm tra xác nhận mật khẩu
            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New password and confirmation do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4. Kiểm tra mật khẩu cũ và mới có trùng nhau không
            if (HashPassword(newPassword) == _currentUser.Password)
            {
                MessageBox.Show("New password must be different from the current password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _currentUser.Password = HashPassword(newPassword);
            // Gọi service để cập nhật thông tin
            bool isUpdated = _userService.UpdateUserProfile(_currentUser);
            if (isUpdated)
            {
                MessageBox.Show("Password changed successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Đóng cửa sổ sau khi đổi mật khẩu
            }
            else
            {
                MessageBox.Show("Error change password.");
            }
        }
    }
}
