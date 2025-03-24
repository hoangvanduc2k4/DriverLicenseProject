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
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.ApplicationServices;

namespace DriverLicenseApp
{

    /// <summary>
    /// Interaction logic for ListExam.xaml
    /// </summary>
    public partial class ListExam : Window
    {
        private ExamService _examService;
        public int _userId;

        public ListExam(int userId)
        {
            _userId = userId;
            InitializeComponent();
            _examService = new ExamService();
            LoadExams();
        }

        // Load tất cả kỳ thi vào DataGrid
        private void LoadExams()
        {
            try
            {
                var exams = _examService.GetAllExams().Where(x => x.UserId == _userId);
                examDataGrid.ItemsSource = exams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading exams: " + ex.Message);
            }
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            string courseNameFilter = txtCourseNameFilter.Text?.Trim();
            string examDateFilter = txtExamDateFilter.Text?.Trim();
            try
            {
                var filteredExams = _examService.GetAllExams()
                    .Where(x => x.UserId == _userId);

                if (!courseNameFilter.IsNullOrEmpty())
                {
                    filteredExams = filteredExams.Where(x => x.Course.CourseName.ToLower().Contains(courseNameFilter.ToLower()));
                }

                if (!examDateFilter.IsNullOrEmpty())
                {
                    filteredExams = filteredExams.Where(x => x.ExamDate.ToString("yyyy-MM-dd").Contains(examDateFilter));
                }
                   
                  
                examDataGrid.ItemsSource = filteredExams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering exams: " + ex.Message);
            }
        }



        // Xử lý sự kiện khi nhấn nút Fill Mark
        private void FillMarkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (examDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("Please select an exam.");
                    return;
                }

                // Ép kiểu selected item thành dynamic để lấy CourseID
                dynamic selectedExam = examDataGrid.SelectedItem;
                int examId = selectedExam.ExamId;

                // Giả sử có một cửa sổ StudentMarkWindow nhận courseId để load danh sách học sinh và nhập điểm
                FillMark fillMark = new FillMark(examId);
                this.Hide();
                fillMark.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening mark window: " + ex.Message);
            }
        }
    }
}
