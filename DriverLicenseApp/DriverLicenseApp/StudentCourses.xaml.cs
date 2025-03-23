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
    /// Interaction logic for StudentCourses.xaml
    /// </summary>
    public partial class StudentCourses : Window
    {
        private int _userId;
        private UserService _userService = new UserService();
        private readonly CourseService courseService = new CourseService();
        private readonly LicenseDriverDbContext context = new LicenseDriverDbContext();
        public StudentCourses(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadDataGridCourse();
        }

        public void LoadDataGridCourse()
        {
            var courses = context.Registrations
                .Include(r => r.Course)
                .Include(r => r.Course.Teacher)
                .Where(r => r.UserId == _userId)
                .Where(r => r.Status == "Approved")
                .ToList();

            dgCourses.ItemsSource = courses;
        }


        private void dgCourses_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgCourses.SelectedItem is not null)
            {
                int selectedCourseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);
                var course = context.Courses.Find(selectedCourseId);

                if (course != null)
                {
                    txtCourseName.Text = course.CourseName;
                    dpStartDate.SelectedDate = course.StartDate.ToDateTime(TimeOnly.MinValue);
                    dpEndDate.SelectedDate = course.EndDate.ToDateTime(TimeOnly.MinValue);

                    // Cách 1: Dùng SelectedItem
                    cbStatus.SelectedItem = cbStatus.Items.Cast<ComboBoxItem>()
                        .FirstOrDefault(item => item.Content.ToString() == course.Status);

                    // Cách 2: Dùng SelectedIndex
                    cbStatus.SelectedIndex = course.Status switch
                    {
                        "Active" => 0,
                        "Closed" => 1,
                        "Cancelled" => 2,
                        _ => -1
                    };

                }
            }
        }

        private void btnCertificate_Click(object sender, RoutedEventArgs e)
        {

            // Lấy CourseId từ anonymous object
            StudentCertificates studentCertificatesWindom = new StudentCertificates(_userId);
            studentCertificatesWindom.ShowDialog();
        }

        private void btnExam_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourses.SelectedItem is not null)
            {
                // Lấy CourseId từ anonymous object
                int courseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);
                StudentExam studentExamByCourseWindom = new StudentExam(courseId, _userId);
                studentExamByCourseWindom.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a course first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
