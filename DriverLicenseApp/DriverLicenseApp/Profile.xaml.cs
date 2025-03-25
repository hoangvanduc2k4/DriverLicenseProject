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

        public Profile(int userId)
        {
            InitializeComponent();
            _userId = userId;
            _userService = new UserService();
            LoadUserProfile();
        }

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

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = NormalizeName(txtName.Text);
            string email = txtMail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string classInfo = txtClass.Text.Trim();
            string school = txtSchool.Text.Trim();

            Regex emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
            {
                MessageBox.Show("Invalid email address.");
                return;
            }

            Regex phoneRegex = new Regex(@"^\d+$");
            if (!phoneRegex.IsMatch(phone))
            {
                MessageBox.Show("Invalid phone number. It must contain digits only.");
                return;
            }

            _currentUser.FullName = fullName;
            _currentUser.Email = email;
            _currentUser.Phone = phone;
            _currentUser.Class = classInfo;
            _currentUser.School = school;

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

        private string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            name = name.Trim();

            string[] words = name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i].ToLower();
                words[i] = char.ToUpper(word[0]) + word.Substring(1);
            }

            return string.Join(" ", words);
        }
    }
}
