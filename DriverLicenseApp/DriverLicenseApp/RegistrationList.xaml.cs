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
        private Registration _selectedRegistration;

        public RegistrationList(int courseId, string courseName)
        {
            InitializeComponent();
            _courseId = courseId;
            txtCourseName.Text = courseName;
            LoadRegistrations();
        }

        private void LoadRegistrations()
        {
           var _registrations = _registrationService.GetRegistrationsByCourse(_courseId) ?? new List<Registration>();
            dgRegistrations.ItemsSource = _registrations;
        }

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
                    string courseName = _selectedRegistration.Course?.CourseName ?? "Unknown Course";
                    string newStatus = cbStatus.Text;
                    int studentId = _selectedRegistration.UserId;

                    _registrationService.UpdateRegistrationStatus(
                        _selectedRegistration.RegistrationId,
                        newStatus,
                        string.IsNullOrWhiteSpace(txtComments.Text) ? null : txtComments.Text
                    );

                    string notificationMessage = $"Your course registration in {courseName} has been {newStatus}!";
                    _notificationsService.AddNotification(studentId, notificationMessage);

                    MessageBox.Show("Status updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadRegistrations();
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

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string selectedStatus = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();
            string studentNameFilter = txtStudentNameFilter.Text?.Trim();

            try
            {
                var filteredRegistrations = _registrationService.GetRegistrationsByCourse(_courseId).ToList();

                if (!string.IsNullOrEmpty(selectedStatus) && selectedStatus != "All")
                {
                    filteredRegistrations = filteredRegistrations.Where(r => r.Status == selectedStatus).ToList();
                }

                if (!string.IsNullOrEmpty(studentNameFilter))
                {
                    filteredRegistrations = filteredRegistrations.Where(r => r.User.FullName.ToLower().Contains(studentNameFilter.ToLower())).ToList();
                }

                dgRegistrations.ItemsSource = filteredRegistrations;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering registrations: " + ex.Message);
            }
        }

    }
}