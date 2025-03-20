using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace DriverLicenseApp
{
    public partial class ExamPolice : Window
    {
        private ExamService _examService = new ExamService();
        private bool isUpdateMode = false;
        private Exam currentExam = null;

        public ExamPolice()
        {
            InitializeComponent();
            LoadExams();
        }

        private void LoadExams()
        {
            ExamDataGrid.ItemsSource = _examService.GetAllExams();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            isUpdateMode = false;
            currentExam = new Exam();
            ShowExamForm();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (ExamDataGrid.SelectedItem is Exam selectedExam)
            {
                isUpdateMode = true;
                currentExam = selectedExam;
                ShowExamForm();
            }
            else
            {
                MessageBox.Show("Please select an exam to update.");
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ExamDataGrid.SelectedItem is Exam selectedExam)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete {selectedExam.Course.CourseName}?",
                    "Confirm Delete",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _examService.DeleteExam(selectedExam.ExamId);
                        MessageBox.Show($"{selectedExam.Course.CourseName} deleted!");
                        LoadExams();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting exam: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an exam to delete.");
            }
        }

        private void ShowExamForm()
        {
            // Ẩn panel danh sách, hiển thị panel form
            listPanel.Visibility = Visibility.Collapsed;
            examFormPanel.Visibility = Visibility.Visible;

            // Nếu đang ở chế độ Update, khóa không cho chỉnh sửa Course
            if (isUpdateMode)
            {
                cbCourseForm.ItemsSource = new List<Course> { currentExam.Course };
                cbCourseForm.SelectedIndex = 0;
                cbCourseForm.IsEnabled = false;
            }
            else
            {
                cbCourseForm.ItemsSource = CourseRepository.GetAllCourses();
                cbCourseForm.DisplayMemberPath = "CourseName";
                cbCourseForm.SelectedValuePath = "CourseId";
                cbCourseForm.IsEnabled = true;
            }

            // Load danh sách supervisor (User) vào combobox
            var users = UserRepository.GetAllUsers().Where(u => u.Role == 2);
            // Ở chế độ Update, loại bỏ user có Id trùng với TeacherId của Course
            if (isUpdateMode)
            {
                int teacherId = currentExam.Course.TeacherId;
                users = users.Where(u => u.UserId != teacherId).ToList();
            }
            cbUserForm.ItemsSource = users;
            cbUserForm.DisplayMemberPath = "FullName";
            cbUserForm.SelectedValuePath = "UserId";

            // Nếu Update thì điền thông tin hiện có, nếu Add thì để trống
            if (isUpdateMode)
            {
                dpExamDateForm.SelectedDate = currentExam.ExamDate.ToDateTime(new TimeOnly(0, 0));
                txtExamTimeForm.Text = currentExam.ExamTime.ToString(@"hh\:mm");
                txtDurationForm.Text = currentExam.DurationMinutes.ToString();
                txtRoomForm.Text = currentExam.Room;
                if (currentExam.User != null)
                    cbUserForm.SelectedValue = currentExam.User.UserId;
            }
            else
            {
                dpExamDateForm.SelectedDate = null;
                txtExamTimeForm.Text = "";
                txtDurationForm.Text = "";
                txtRoomForm.Text = "";
                cbUserForm.SelectedIndex = -1;
            }
        }

        private void FormSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate đầu vào
            if (!isUpdateMode && cbCourseForm.SelectedItem == null)
            {
                MessageBox.Show("Please select a course.");
                return;
            }
            if (dpExamDateForm.SelectedDate == null)
            {
                MessageBox.Show("Please select an exam date.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtExamTimeForm.Text))
            {
                MessageBox.Show("Please enter exam time.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtDurationForm.Text) || !int.TryParse(txtDurationForm.Text, out int duration))
            {
                MessageBox.Show("Please enter a valid duration in minutes.");
                return;
            }
            if (string.IsNullOrWhiteSpace(txtRoomForm.Text))
            {
                MessageBox.Show("Please enter a room.");
                return;
            }
            if (cbUserForm.SelectedItem == null)
            {
                MessageBox.Show("Please select a supervisor.");
                return;
            }
            if (!TimeSpan.TryParse(txtExamTimeForm.Text, out TimeSpan examTime))
            {
                MessageBox.Show("Exam time format is invalid. Use HH:mm format.");
                return;
            }

            // Ở chế độ Add, gán Course từ combobox
            if (!isUpdateMode)
            {
                currentExam.Course = cbCourseForm.SelectedItem as Course;
            }
            // Cập nhật các trường khác
            currentExam.ExamDate = DateOnly.FromDateTime(dpExamDateForm.SelectedDate.Value);
            currentExam.ExamTime = TimeOnly.FromTimeSpan(examTime);
            currentExam.DurationMinutes = duration;
            currentExam.Room = txtRoomForm.Text;
            currentExam.User = cbUserForm.SelectedItem as User;

            try
            {
                if (isUpdateMode)
                {
                    _examService.UpdateExam(currentExam);
                    MessageBox.Show("Exam updated successfully.");
                }
                else
                {
                    _examService.AddExam(currentExam);
                    MessageBox.Show("Exam added successfully.");
                }
                // Sau khi lưu, quay lại danh sách
                examFormPanel.Visibility = Visibility.Collapsed;
                listPanel.Visibility = Visibility.Visible;
                LoadExams();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving exam: " + ex.Message);
            }
        }

        private void FormCancel_Click(object sender, RoutedEventArgs e)
        {
            // Quay lại danh sách khi ấn Cancel
            examFormPanel.Visibility = Visibility.Collapsed;
            listPanel.Visibility = Visibility.Visible;
        }
    }
}
