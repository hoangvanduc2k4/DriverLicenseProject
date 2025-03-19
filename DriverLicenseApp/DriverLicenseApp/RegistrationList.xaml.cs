using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp
{
    public partial class RegistrationList : Window
    {
        private readonly NotificationsService _notificationsService = new NotificationsService();
        private readonly RegistrationService _registrationService = new RegistrationService();
        private int _courseId;
        private List<Registration> _registrations = new List<Registration>();
        private Registration _selectedRegistration;

        public RegistrationList(int courseId, string courseName)
        {
            InitializeComponent();
            _courseId = courseId;
            txtCourseName.Text = courseName; // Hiển thị tên khóa học
            LoadRegistrations();
        }

        // Load danh sách đăng ký từ Service
        private void LoadRegistrations()
        {
            _registrations = _registrationService.GetRegistrationsByCourse(_courseId) ?? new List<Registration>();
            dgRegistrations.ItemsSource = _registrations;
        }

        // Khi chọn một dòng trong DataGrid
        private void dgRegistrations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedRegistration = dgRegistrations.SelectedItem as Registration;
            if (_selectedRegistration != null)
            {
                txtStudent.Text = _selectedRegistration.User.FullName;
                cbStatus.Text = _selectedRegistration.Status;
                txtComments.Text = _selectedRegistration.Comments;
            }
        }




        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedRegistration != null)
            {
                try
                {
                    // Kiểm tra xem Course có bị null không
                    string courseName = _selectedRegistration.Course?.CourseName ?? "Unknown Course";
                    string newStatus = cbStatus.Text;
                    int studentId = _selectedRegistration.UserId;

                    // Cập nhật trạng thái đăng ký
                    _registrationService.UpdateRegistrationStatus(
                        _selectedRegistration.RegistrationId,
                        newStatus,
                        string.IsNullOrWhiteSpace(txtComments.Text) ? null : txtComments.Text
                    );

                    // Tạo nội dung thông báo
                    string notificationMessage = $"Your course registration in {courseName} has been {newStatus}!";

                    // Gửi thông báo đến học viên
                    _notificationsService.AddNotification(studentId, notificationMessage);

                    MessageBox.Show("Status updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadRegistrations(); // Reload danh sách sau khi cập nhật
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a registration to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        // Đóng cửa sổ và quay lại màn hình khóa học
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (_registrations == null || cbStatusFilter.SelectedItem == null)
                return; // Tránh lỗi nếu dữ liệu chưa load

            string selectedStatus = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();

            // Nếu chọn "All" thì hiển thị toàn bộ danh sách
            if (string.IsNullOrEmpty(selectedStatus) || selectedStatus == "All")
            {
                dgRegistrations.ItemsSource = _registrations;
            }
            else
            {
                dgRegistrations.ItemsSource = _registrations
                    .Where(r => r != null && !string.IsNullOrEmpty(r.Status) && r.Status.Equals(selectedStatus, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
        }
    }
}
