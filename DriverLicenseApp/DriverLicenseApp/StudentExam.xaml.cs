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

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for StudentExam.xaml
    /// </summary>
    public partial class StudentExam : Window
    {
        private ExamService _examService;
        public int _courseID;

        public StudentExam(int courseID)
        {
            _courseID = courseID;
            InitializeComponent();
            _examService = new ExamService();
            LoadExams();
        }

        // Load tất cả kỳ thi vào DataGrid
        private void LoadExams()
        {
            try
            {
                var exams = _examService.GetAllExams().Where(x => x.CourseId == _courseID);
                examDataGrid.ItemsSource = exams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading exams: " + ex.Message);
            }
        }
    }
}
