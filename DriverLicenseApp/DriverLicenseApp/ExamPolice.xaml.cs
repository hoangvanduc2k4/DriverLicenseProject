using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DriverLicenseApp
{
    public partial class ExamPolice : Window
    {
        private ExamService _examService = new ExamService();
        private Exam currentExam = null;

        public ExamPolice()
        {
            InitializeComponent();
            LoadSearchFilters();
            LoadExams();
        }

        private void allExamButton_Click(object sender, RoutedEventArgs e)
        {
            // Load lại toàn bộ danh sách kỳ thi
            LoadExams();

            // Nếu muốn reset lại các filter tìm kiếm, bạn có thể đặt lại giá trị cho các control tìm kiếm:
            courseComboBox.SelectedIndex = -1;
            examDatePicker.SelectedDate = null;
            examTimeTextBox.Text = "";
            durationTextBox.Text = "";
            roomTextBox.Text = "";
            teacherComboBox.SelectedIndex = -1;
        }


        private void LoadExams()
        {
            // Load danh sách kỳ thi vào DataGrid
            ExamDataGrid.ItemsSource = _examService.GetAllExams();
        }

        private void LoadSearchFilters()
        {
            using (var context = new LicenseDriverDbContext())
            {
                // Load khóa học
                var courses = context.Courses.ToList();
                courseComboBox.ItemsSource = courses;
                courseComboBox.DisplayMemberPath = "CourseName";
                courseComboBox.SelectedValuePath = "CourseId";

                // Load giáo viên (Role = 2)
                var teachers = context.Users.Where(u => u.Role == 2).ToList();
                teacherComboBox.ItemsSource = teachers;
                teacherComboBox.DisplayMemberPath = "FullName";
                teacherComboBox.SelectedValuePath = "UserId";
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(durationTextBox.Text))
            {
                if (!int.TryParse(durationTextBox.Text, out int parsedDuration) || parsedDuration < 0)
                {
                    MessageBox.Show("Duration must be a non-negative number.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(roomTextBox.Text))
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(roomTextBox.Text, @"[^a-zA-Z0-9\s]"))
                {
                    MessageBox.Show("Room must not contain special characters.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            int? courseId = courseComboBox.SelectedValue as int?;
            DateTime? examDate = examDatePicker.SelectedDate;

            TimeSpan? examTime = examTimeTextBox.Value.HasValue
                ? examTimeTextBox.Value.Value.TimeOfDay
                : (TimeSpan?)null;

            int? duration = null;
            if (int.TryParse(durationTextBox.Text, out int validDuration))
                duration = validDuration;

            string room = roomTextBox.Text;
            int? teacherId = teacherComboBox.SelectedValue as int?;

            var filteredExams = _examService.SearchExams(courseId, examDate, examTime, duration, room, teacherId);
            ExamDataGrid.ItemsSource = filteredExams;
        }


        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            // Chuyển sang chế độ thêm mới (currentExam = null)
            ShowDetailsView(null);
        }

        private void updateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExamDataGrid.SelectedItem is Exam selectedExam)
            {
                ShowDetailsView(selectedExam);
            }
            else
            {
                MessageBox.Show("Please select an exam to update.", "Update Exam", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ExamDataGrid.SelectedItem is Exam selectedExam)
            {
                MessageBoxResult result = MessageBox.Show("Do you really want to delete this exam?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    _examService.DeleteExam(selectedExam.ExamId);
                    LoadExams();
                }
            }
            else
            {
                MessageBox.Show("Please select an exam to delete.", "Delete Exam", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ShowDetailsView(Exam exam)
        {
            currentExam = exam;
            ListViewPanel.Visibility = Visibility.Collapsed;
            DetailsPanel.Visibility = Visibility.Visible;

            using (var context = new LicenseDriverDbContext())
            {
                var courses = context.Courses.ToList();
                detailsCourseComboBox.ItemsSource = courses;
                detailsCourseComboBox.DisplayMemberPath = "CourseName";
                detailsCourseComboBox.SelectedValuePath = "CourseId";
            }

            if (exam != null)
            {
                detailsTitleLabel.Content = "Update Exam";
                detailsCourseComboBox.SelectedValue = exam.Course.CourseId;
                detailsExamDatePicker.SelectedDate = exam.ExamDate.ToDateTime(new TimeOnly(0, 0));
                detailsExamTimePicker.Value = DateTime.Today.Add(exam.ExamTime.ToTimeSpan());
                detailsDurationTextBox.Text = exam.DurationMinutes.ToString();
                detailsRoomTextBox.Text = exam.Room;
            }
            else
            {
                detailsTitleLabel.Content = "Add New Exam";
                detailsCourseComboBox.SelectedIndex = -1;
                detailsExamDatePicker.SelectedDate = null;
                detailsExamTimePicker.Value = null;
                detailsDurationTextBox.Text = "";
                detailsRoomTextBox.Text = "";
                detailsTeacherComboBox.ItemsSource = null;
            }
        }

        private void detailsCourseComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (detailsCourseComboBox.SelectedItem is DAL.Models.Course selectedCourse)
            {
                using (var context = new LicenseDriverDbContext())
                {
                    var teachers = context.Users
                                          .Where(u => u.Role == 2 && u.UserId != selectedCourse.TeacherId)
                                          .ToList();
                    detailsTeacherComboBox.ItemsSource = teachers;
                    detailsTeacherComboBox.DisplayMemberPath = "FullName";
                    detailsTeacherComboBox.SelectedValuePath = "UserId";

                    if (currentExam != null)
                    {                 
                        int examTeacherId = currentExam.User?.UserId ?? -1;
                        if (teachers.Any(t => t.UserId == examTeacherId))
                        {
                            detailsTeacherComboBox.SelectedValue = examTeacherId;
                        }
                        else
                        {
                            detailsTeacherComboBox.SelectedIndex = -1;
                        }
                    }
                    else
                    {
                        detailsTeacherComboBox.SelectedIndex = -1;
                    }
                }
            }
        }



        private void detailsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(detailsCourseComboBox.SelectedItem is DAL.Models.Course selectedCourse))
            {
                MessageBox.Show("Please select a course.");
                return;
            }

            if (!detailsExamDatePicker.SelectedDate.HasValue || detailsExamDatePicker.SelectedDate.Value.Date < DateTime.Today)
            {
                MessageBox.Show("Exam date cannot be in the past.", "Invalid Date", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime dateTimeValue;
            if (detailsExamTimePicker.Value.HasValue)
            {
                dateTimeValue = detailsExamTimePicker.Value.Value;
            }
            else if (DateTime.TryParse(detailsExamTimePicker.Text, out DateTime parsedTime))
            {
                dateTimeValue = parsedTime;
            }
            else
            {
                MessageBox.Show("Please select a valid exam time.", "Invalid Exam Time", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(detailsDurationTextBox.Text) ||
                !int.TryParse(detailsDurationTextBox.Text, out int duration) ||
                duration < 0)
            {
                MessageBox.Show("Duration must be a non-negative number and cannot be empty.", "Invalid Duration", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(detailsRoomTextBox.Text))
            {
                MessageBox.Show("Please enter room.", "Invalid Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (System.Text.RegularExpressions.Regex.IsMatch(detailsRoomTextBox.Text, @"[^a-zA-Z0-9\s]"))
            {
                MessageBox.Show("Room must not contain special characters.", "Invalid Room", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!(detailsTeacherComboBox.SelectedItem is DAL.Models.User selectedTeacher))
            {
                MessageBox.Show("Please select a teacher.");
                return;
            }
            if (selectedTeacher.UserId == selectedCourse.TeacherId)
            {
                MessageBox.Show("The selected teacher cannot be the same as the teacher in charge of the course.");
                return;
            }

            if (currentExam != null)
            {
                currentExam.Course = selectedCourse;
                currentExam.ExamDate = DateOnly.FromDateTime(detailsExamDatePicker.SelectedDate.Value);

                currentExam.ExamTime = TimeOnly.FromTimeSpan(dateTimeValue.TimeOfDay);

                currentExam.DurationMinutes = duration;
                currentExam.Room = detailsRoomTextBox.Text;
                currentExam.User = selectedTeacher;
                _examService.UpdateExam(currentExam);
            }
            else
            {
                Exam newExam = new Exam
                {
                    CourseId = selectedCourse.CourseId,  
                    ExamDate = DateOnly.FromDateTime(detailsExamDatePicker.SelectedDate.Value),
                    ExamTime = TimeOnly.FromTimeSpan(dateTimeValue.TimeOfDay),
                    DurationMinutes = duration,
                    Room = detailsRoomTextBox.Text,
                    UserId = selectedTeacher.UserId       
                };
                _examService.AddExam(newExam);
            }

            ShowListView();
        }

        private void detailsCancelButton_Click(object sender, RoutedEventArgs e)
        {
            ShowListView();
        }

        private void ShowListView()
        {
            DetailsPanel.Visibility = Visibility.Collapsed;
            ListViewPanel.Visibility = Visibility.Visible;
            LoadExams();
        }
    }
}

