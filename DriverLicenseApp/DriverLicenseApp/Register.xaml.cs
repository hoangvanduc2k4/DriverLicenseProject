using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

            MessageBox.Show("Register Successfully!", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

        }


        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPassword.Password;
            string fullName = NormalizeName(txtFullName.Text);
            string confirmPassword = txtConfirmPassword.Password;
            // Kiểm tra xem người dùng đã chọn role trong ComboBox chưa
            if (cbRole.SelectedItem == null)
            {
                MessageBox.Show("Please choose a role.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int role = Convert.ToInt32(((ComboBoxItem)cbRole.SelectedItem).Tag);

            // Validate Email (định dạng hợp lệ)
            Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                MessageBox.Show("Invalid email address.");
                return;
            }
            // 2. Kiểm tra mật khẩu hợp lệ không
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Confirm password not match!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using LicenseDriverDbContext context = new();
            if (context.Users.Any(u => u.Email == email)) // Giả sử bảng Users có cột Email
            {
                MessageBox.Show("Email existed, please cho other email!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            // Loại bỏ khoảng trắng ở đầu và cuối
            name = name.Trim();

            // Tách các từ, loại bỏ khoảng trắng thừa
            string[] words = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Viết hoa chữ cái đầu của mỗi từ, phần còn lại viết thường
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i].ToLower();
                words[i] = char.ToUpper(word[0]) + word.Substring(1);
            }

            // Nối lại các từ với một khoảng trắng
            return string.Join(" ", words);
        }
    }
}
