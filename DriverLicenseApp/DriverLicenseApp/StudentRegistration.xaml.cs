using System;
using System.Collections.Generic;
using System.Linq;
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
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for StudentRegistration.xaml
    /// </summary>
    public partial class StudentRegistration : Window
    {
        private readonly CourseService courseService = new CourseService();
        private readonly RegistrationService regisService = new RegistrationService();
        private readonly LicenseDriverDbContext context = new LicenseDriverDbContext();
        public int _userId;
        public StudentRegistration(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadDataGridCourse();
        }
        public void LoadDataGridCourse()
        {
            var courses = context.Courses
                .Include(c => c.Registrations)
                .Include(c => c.Teacher)
                .Where(c => c.Status == "Active")
                .Select(c => new
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    TeacherName = c.Teacher.FullName, // Giả sử Teacher có thuộc tính FullName
                    IsRegistered = c.Registrations.Any(r => r.UserId == _userId), // Kiểm tra xem student đã đăng ký chưa
                    RegistrationStatus = c.Registrations
                    .Where(r => r.UserId == _userId)
                    .OrderByDescending(r => r.RegistrationId)
                    .Select(r => r.Status)
                    .FirstOrDefault() // Lấy trạng thái đăng ký nếu có
                })
                .ToList();

            dgCourses.ItemsSource = courses;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourses.SelectedItem is not null)
            {
                int selectedCourseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);
                var registration = context.Registrations.Include(r => r.Course)
                    .Where(r => r.CourseId == selectedCourseId)
                    .Where(r => r.UserId == _userId)
                    .OrderByDescending(r => r.RegistrationId).FirstOrDefault();

                // Nếu đơn đang ở trạng thái "Pending"
                if (registration != null && registration.Status == "Pending")
                {
                    regisService.RemoveRegistration(registration.RegistrationId);

                    MessageBox.Show("Registration canceled successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataGridCourse();
                }
                else if (registration == null)
                {
                    MessageBox.Show("You have not registered for this course!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("Cannot cancel an approved or rejected registration!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a course to cancel!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourses.SelectedItem is not null)
            {
                int selectedCourseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);
                var registration = context.Registrations.Include(r => r.Course)
                    .Where(r => r.CourseId == selectedCourseId)
                    .Where(r => r.UserId == _userId)
                    .OrderByDescending(r => r.RegistrationId).FirstOrDefault();

                if (registration == null || (registration.Status != "Approved" && registration.Status != "Pending"))
                {
                    // Tạo mới đơn đăng ký
                    var newRegistration = new Registration
                    {
                        UserId = _userId,
                        CourseId = selectedCourseId,
                        Status = "Pending", // Mặc định là Pending
                    };
                    regisService.AddRegistration(newRegistration);
                    MessageBox.Show("Registration submitted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataGridCourse();
                }
                else
                {
                    MessageBox.Show("You have already been approved/applied for this course!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select a course to register!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dgCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCourses.SelectedItem is not null)
            {
                int selectedCourseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);
                var registration = context.Registrations.Include(r => r.Course)
                    .Where(r => r.CourseId == selectedCourseId)
                    .Where(r => r.UserId == _userId)
                    .OrderByDescending(r => r.RegistrationId).FirstOrDefault();

                if (registration != null)
                {
                    txtComments.Text = registration.Comments;
                }

            }
        }
    }
}
