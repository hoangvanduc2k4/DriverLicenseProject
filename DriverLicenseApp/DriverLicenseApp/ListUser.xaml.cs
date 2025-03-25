using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DriverLicenseApp
{
    public partial class ListUser : Window
    {
        private UserService _userService;
        private List<User> _allUsers;  

        private bool isUpdateMode = false;
        private User selectedUserForUpdate;

        public ListUser()
        {
            InitializeComponent();

            _userService = new UserService();
            LoadUsers();

            if (roleColumn.Binding is Binding binding)
            {
                binding.Converter = new RoleToStringConverter();
            }
        }

        private void LoadUsers()
        {
            _allUsers = _userService.GetAllUsers();
            UserDataGrid.ItemsSource = _allUsers;
        }

        #region Validation Methods

        private bool ValidateSearchFields()
        {
            string searchFullName = txtSearchFullName.Text;
            if (!string.IsNullOrWhiteSpace(searchFullName))
            {
                if (!Regex.IsMatch(searchFullName, @"^[\p{L}\s]+$"))
                {
                    MessageBox.Show("Search Full Name may only contain letters and spaces (no digits or special characters allowed).",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(searchFullName.Trim()))
                {
                    MessageBox.Show("Search Full Name cannot consist entirely of spaces.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            string searchEmail = txtSearchEmail.Text;
            if (!string.IsNullOrEmpty(searchEmail) && string.IsNullOrWhiteSpace(searchEmail.Trim()))
            {
                MessageBox.Show("Search Email cannot consist entirely of spaces.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string searchClass = txtSearchClass.Text;
            if (!string.IsNullOrEmpty(searchClass))
            {
                if (!Regex.IsMatch(searchClass, @"^[\p{L}\p{N}\s]+$"))
                {
                    MessageBox.Show("Search Class may only contain letters, digits and spaces (no special characters allowed).",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(searchClass.Trim()))
                {
                    MessageBox.Show("Search Class cannot consist entirely of spaces.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            string searchSchool = txtSearchSchool.Text;
            if (!string.IsNullOrEmpty(searchSchool))
            {
                if (!Regex.IsMatch(searchSchool, @"^[\p{L}\s]+$"))
                {
                    MessageBox.Show("Search School may only contain letters and spaces (no special characters allowed).",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(searchSchool.Trim()))
                {
                    MessageBox.Show("Search School cannot consist entirely of spaces.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            string searchPhone = txtSearchPhone.Text;
            if (!string.IsNullOrEmpty(searchPhone))
            {
                if (string.IsNullOrEmpty(searchPhone.Trim()))
                {
                    MessageBox.Show("Search Phone cannot consist entirely of spaces.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (!Regex.IsMatch(searchPhone, @"^\d{1,10}$"))
                {
                    MessageBox.Show("Search Phone may only contain digits (up to 10 digits) and cannot include negative numbers or special characters.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            return true;
        }

        private bool ValidateUserDetails()
        {
            string fullName = txtFullNameDetail.Text.Trim();
            if (string.IsNullOrEmpty(fullName))
            {
                MessageBox.Show("Full Name cannot be empty.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(fullName, @"^(?!\s*$)[\p{L}]+(?: [\p{L}]+)*$"))
            {
                MessageBox.Show("Full Name may only contain letters and words separated by a single space (no digits or special characters allowed).",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string email = txtEmailDetail.Text.Trim();
            if (string.IsNullOrEmpty(email))
            {
                MessageBox.Show("Email cannot be empty.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    throw new Exception();
                }
            }
            catch
            {
                MessageBox.Show("Email format is invalid.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (cbRoleDetail.SelectedItem == null)
            {
                MessageBox.Show("Role must be selected.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string classInfo = txtClassDetail.Text;
            if (!string.IsNullOrEmpty(classInfo))
            {
                if (!Regex.IsMatch(classInfo, @"^[a-zA-Z0-9\s]+$"))
                {
                    MessageBox.Show("Class may only contain letters, digits, and spaces (no special characters allowed).",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(classInfo))
                {
                    MessageBox.Show("Class cannot consist entirely of spaces.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }


            string school = txtSchoolDetail.Text;
            if (!string.IsNullOrEmpty(school))
            {
                if (!Regex.IsMatch(school, @"^[a-zA-Z0-9\s]+$"))
                {
                    MessageBox.Show("School may only contain letters, digits, and spaces (no special characters allowed).",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
                if (string.IsNullOrEmpty(school.Trim()))
                {
                    MessageBox.Show("School cannot consist entirely of spaces.",
                        "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }

            if (string.IsNullOrEmpty(txtPasswordDetail.Password))
            {
                MessageBox.Show("Password cannot be empty.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            string phone = txtPhoneDetail.Text.Trim();
            if (string.IsNullOrEmpty(phone))
            {
                MessageBox.Show("Phone cannot be empty.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (!Regex.IsMatch(phone, @"^\d{1,10}$"))
            {
                MessageBox.Show("Phone may only contain digits (up to 10 digits) and cannot include special characters or negative numbers.",
                    "Validation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        #endregion

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateSearchFields())
                return;

            string searchFullName = txtSearchFullName.Text?.Trim().ToLower();
            string searchEmail = txtSearchEmail.Text?.Trim().ToLower();
            string selectedRoleTag = (cbSearchRole.SelectedItem as ComboBoxItem)?.Tag?.ToString();
            int selectedRole = 0;
            if (!string.IsNullOrEmpty(selectedRoleTag))
            {
                int.TryParse(selectedRoleTag, out selectedRole);
            }
            string searchClass = txtSearchClass.Text?.Trim().ToLower();
            string searchSchool = txtSearchSchool.Text?.Trim().ToLower();
            string searchPhone = txtSearchPhone.Text?.Trim().ToLower();

            var filtered = _allUsers.Where(u =>
                (string.IsNullOrEmpty(searchFullName) || (u.FullName != null && u.FullName.ToLower().Contains(searchFullName))) &&
                (string.IsNullOrEmpty(searchEmail) || (u.Email != null && u.Email.ToLower().Contains(searchEmail))) &&
                (selectedRole == 0 || u.Role == selectedRole) &&
                (string.IsNullOrEmpty(searchClass) || (u.Class != null && u.Class.ToLower().Contains(searchClass))) &&
                (string.IsNullOrEmpty(searchSchool) || (u.School != null && u.School.ToLower().Contains(searchSchool))) &&
                (string.IsNullOrEmpty(searchPhone) || (u.Phone != null && u.Phone.ToLower().Contains(searchPhone)))
            ).ToList();

            UserDataGrid.ItemsSource = filtered;
        }

        private void AllUsersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadUsers();
            txtSearchFullName.Text = "";
            txtSearchEmail.Text = "";
            cbSearchRole.SelectedIndex = 0;
            txtSearchClass.Text = "";
            txtSearchSchool.Text = "";
            txtSearchPhone.Text = "";
        }

        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            isUpdateMode = false;
            selectedUserForUpdate = null;
            ClearDetailsFields();
            ShowDetailsPanel();
        }

        private void UpdateUserButton_Click(object sender, RoutedEventArgs e)
        {
            if (UserDataGrid.SelectedItem is User user)
            {
                isUpdateMode = true;
                selectedUserForUpdate = user;
                FillDetailsFields(user);
                ShowDetailsPanel();
            }
            else
            {
                MessageBox.Show("Please select a user to update.", "Update User", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowDetailsPanel()
        {
            ListViewPanel.Visibility = Visibility.Collapsed;
            DetailsPanel.Visibility = Visibility.Visible;
        }

        private void HideDetailsPanel()
        {
            DetailsPanel.Visibility = Visibility.Collapsed;
            ListViewPanel.Visibility = Visibility.Visible;
        }

        private void ClearDetailsFields()
        {
            txtFullNameDetail.Text = "";
            txtEmailDetail.Text = "";
            txtPhoneDetail.Text = "";
            txtClassDetail.Text = "";
            txtSchoolDetail.Text = "";
            txtPasswordDetail.Password = ""; 
            cbRoleDetail.SelectedIndex = 0;
        }

        private void FillDetailsFields(User user)
        {
            txtFullNameDetail.Text = user.FullName;
            txtEmailDetail.Text = user.Email;
            txtPhoneDetail.Text = user.Phone;
            txtClassDetail.Text = user.Class;
            txtSchoolDetail.Text = user.School;
            txtPasswordDetail.Password = user.Password ?? "";
            foreach (ComboBoxItem item in cbRoleDetail.Items)
            {
                if (item.Tag != null && int.TryParse(item.Tag.ToString(), out int role) && role == user.Role)
                {
                    cbRoleDetail.SelectedItem = item;
                    break;
                }
            }
        }

        private void SaveDetailButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateUserDetails())
                return;

            string fullName = txtFullNameDetail.Text.Trim();
            string email = txtEmailDetail.Text.Trim();
            string phone = txtPhoneDetail.Text.Trim();
            string classInfo = txtClassDetail.Text.Trim();
            string school = txtSchoolDetail.Text.Trim();
            string password = txtPasswordDetail.Password;
            int role = 0;
            if (cbRoleDetail.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag != null)
            {
                int.TryParse(selectedItem.Tag.ToString(), out role);
            }

            if (isUpdateMode && selectedUserForUpdate != null)
            {
                selectedUserForUpdate.FullName = fullName;
                selectedUserForUpdate.Email = email;
                selectedUserForUpdate.Phone = phone;
                selectedUserForUpdate.Class = classInfo;
                selectedUserForUpdate.School = school;
                selectedUserForUpdate.Password = password;
                selectedUserForUpdate.Role = role;
                _userService.UpdateUser(selectedUserForUpdate);
            }
            else
            {
                User newUser = new User
                {
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Class = classInfo,
                    School = school,
                    Password = password,
                    Role = role
                };
                _userService.AddUser(newUser);
            }

            HideDetailsPanel();
            LoadUsers();
        }

        private void CancelDetailButton_Click(object sender, RoutedEventArgs e)
        {
            HideDetailsPanel();
        }
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            HideDetailsPanel();
        }


        private void TogglePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (txtPasswordDetail.Visibility == Visibility.Visible)
            {
                txtPasswordVisible.Text = txtPasswordDetail.Password;
                txtPasswordVisible.Visibility = Visibility.Visible;
                txtPasswordDetail.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtPasswordDetail.Password = txtPasswordVisible.Text;
                txtPasswordDetail.Visibility = Visibility.Visible;
                txtPasswordVisible.Visibility = Visibility.Collapsed;
            }
        }

    }

    public class RoleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "N/A";

            int role = (int)value;
            return role switch
            {
                1 => "Student",
                2 => "Teacher",
                3 => "TrafficPolice",
                4 => "Admin",
                _ => "N/A",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
