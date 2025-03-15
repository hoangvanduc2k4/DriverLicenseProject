using System;
using System.Collections.Generic;
using System.Linq;
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
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using Microsoft.IdentityModel.Tokens;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        
  
        private int _userId;
        private User _currentUser;
        private UserService _userService;

        // Constructor nhận userId để load thông tin người dùng
        public Profile(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _userService = new UserService();
            LoadUserProfile();
        }

        // Load thông tin người dùng từ database và điền vào các ô TextBox
        private void LoadUserProfile()
        {
            _currentUser = _userService.GetUserProfile(_userId);
            if (_currentUser != null)
            {
                txtName.Text = _currentUser.FullName;
                txtMail.Text = _currentUser.Email;
                txtPhone.Text = _currentUser.Phone;
                txtClass.Text = _currentUser.Class;
                txtSchool.Text = _currentUser.School;
            }
            else
            {
                MessageBox.Show("User not found.");
                this.Close();
            }
        }

        // Sự kiện khi nhấn nút Update Profile
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy dữ liệu và chuẩn hóa
            string fullName = NormalizeName(txtName.Text);
            string email = txtMail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string classInfo = txtClass.Text.Trim();
            string school = txtSchool.Text.Trim();

            // Validate Email (định dạng hợp lệ)
            Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                MessageBox.Show("Invalid email address.");
                return;
            }

            // Validate Phone (chỉ chứa chữ số)
            Regex phoneRegex = new Regex(@"^\d+$");
            if (!phoneRegex.IsMatch(phone))
            {
                MessageBox.Show("Invalid phone number. It must contain digits only.");
                return;
            }

            // Cập nhật thông tin cho đối tượng user hiện tại
            _currentUser.FullName = fullName;
            _currentUser.Email = email;
            _currentUser.Phone = phone;
            _currentUser.Class = classInfo;
            _currentUser.School = school;

            // Gọi service để cập nhật thông tin
            bool isUpdated = _userService.UpdateUserProfile(_currentUser);
            if (isUpdated)
            {
                MessageBox.Show("Profile Updated Successfully!");
            }
            else
            {
                MessageBox.Show("Error updating profile.");
            }
        }

        // Hàm chuẩn hóa tên: loại bỏ khoảng trắng thừa, chỉ cách 1 khoảng trắng giữa các từ,
        // viết hoa chữ cái đầu của mỗi từ, các chữ còn lại viết thường.
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
