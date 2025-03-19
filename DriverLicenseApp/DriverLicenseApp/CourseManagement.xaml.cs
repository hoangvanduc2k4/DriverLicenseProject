﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApp
{
    public partial class CourseManagement : Window
    {
        private readonly CourseService courseService = new CourseService();
        private readonly LicenseDriverDbContext context = new LicenseDriverDbContext();
        public int _userId;
        public CourseManagement()
        {
            _userId = 1;
            InitializeComponent();
            LoadDataGridCourse();
   
        }


        public void LoadDataGridCourse()
        {
            var courses = context.Courses
                .Include(c => c.Registrations)
                .Include(c => c.Teacher)
                .Where(c => c.TeacherId == _userId) // Lọc trước khi Select
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName,
                    TeacherName = c.Teacher.FullName,
                    c.StartDate,
                    c.EndDate,
                    c.Status,
                    ApprovedCount = c.Registrations.Count(r => r.Status == "Approved"),
                    PendingCount = c.Registrations.Count(r => r.Status == "Pending"),
                    RejectedCount = c.Registrations.Count(r => r.Status == "Rejected")
                })
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


        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInputs(out string courseName, out DateOnly startDate, out DateOnly endDate, out string status))
                return;

            var newCourse = new Course
            {
                CourseName = courseName,
                TeacherId = _userId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status
            };

            courseService.AddCourse(newCourse);
            MessageBox.Show("Course added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadDataGridCourse();
            ResetForm();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourses.SelectedItem is null)
            {
                MessageBox.Show("Please select a course to update.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedCourseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);

            if (!ValidateInputs(out string courseName, out DateOnly startDate, out DateOnly endDate, out string status))
                return;

            var existingCourse = context.Courses.Find(selectedCourseId);
            if (existingCourse != null)
            {
                existingCourse.CourseName = courseName;
                existingCourse.StartDate = startDate;
                existingCourse.EndDate = endDate;
                existingCourse.Status = status;
                existingCourse.TeacherId = _userId;

                courseService.UpdateCourse(existingCourse);
                MessageBox.Show("Course updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDataGridCourse();
                ResetForm();
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourses.SelectedItem is null)
            {
                MessageBox.Show("Please select a course to delete.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int selectedCourseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);

            var courseToDelete = context.Courses.Include(c => c.Registrations).FirstOrDefault(c => c.CourseId == selectedCourseId);
            if (courseToDelete != null)
            {
                if (courseToDelete.Registrations.Any())
                {
                    MessageBox.Show("Cannot delete course with existing registrations.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this course?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    courseService.RemoveCourse(courseToDelete);
                    MessageBox.Show("Course deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadDataGridCourse();
                    ResetForm();
                }
            }
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();
        }

        private void ResetForm()
        {
            txtCourseName.Clear();
            dpStartDate.SelectedDate = null;
            dpEndDate.SelectedDate = null;
            cbStatus.SelectedIndex = -1;
        }

        private bool ValidateInputs(out string courseName, out DateOnly startDate, out DateOnly endDate, out string status)
        {
            courseName = txtCourseName.Text.Trim();
            startDate = default;
            endDate = default;
            status = cbStatus.SelectedItem is ComboBoxItem item ? item.Content.ToString() : cbStatus.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(courseName))
            {
                MessageBox.Show("Course Name is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (!dpStartDate.SelectedDate.HasValue || !dpEndDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Start Date and End Date are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            startDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value);
            endDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value);

            if (startDate > endDate)
            {
                MessageBox.Show("Start Date cannot be later than End Date.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            if (string.IsNullOrEmpty(status))
            {
                MessageBox.Show("Please select a status.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

     


        private string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return string.Join(" ", input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).ToLower();
        }

        private void btnRegistration_Click(object sender, RoutedEventArgs e)
        {
            if (dgCourses.SelectedItem is not null)
            {
                // Lấy CourseId từ anonymous object
                int courseId = (int)dgCourses.SelectedItem.GetType().GetProperty("CourseId")?.GetValue(dgCourses.SelectedItem);
                string courseName = (string)dgCourses.SelectedItem.GetType().GetProperty("CourseName")?.GetValue(dgCourses.SelectedItem);

                var registrationList = new RegistrationList(courseId, courseName);
                registrationList.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a course first!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}