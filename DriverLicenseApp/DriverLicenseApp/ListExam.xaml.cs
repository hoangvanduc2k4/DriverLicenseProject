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

namespace DriverLicenseApp
{

    /// <summary>
    /// Interaction logic for ListExam.xaml
    /// </summary>
    public partial class ListExam : Window
    {
        private ExamService _examService;

        public ListExam()
        {
            InitializeComponent();
            _examService = new ExamService();
            LoadExams();
        }

        // Load tất cả kỳ thi vào DataGrid
        private void LoadExams()
        {
            try
            {
                var exams = _examService.GetAllExams();
                examDataGrid.ItemsSource = exams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading exams: " + ex.Message);
            }
        }

        // Xử lý sự kiện khi nhấn nút Search
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string teacherName = NormalizeString(txtSearch.Text.Trim().ToLower());
                var exams = _examService.GetExamsByTeacher(teacherName);
                examDataGrid.ItemsSource = exams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching exams: " + ex.Message);
            }
        }

        private string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            return string.Join(" ", input.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).ToLower();
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
                int courseId = selectedExam.CourseId;

                // Giả sử có một cửa sổ StudentMarkWindow nhận courseId để load danh sách học sinh và nhập điểm
                FillMark fillMark = new FillMark(courseId);
                fillMark.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening mark window: " + ex.Message);
            }
        }
    }
}
