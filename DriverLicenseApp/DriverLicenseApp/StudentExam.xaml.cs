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
    /// Interaction logic for StudentExam.xaml
    /// </summary>
    public partial class StudentExam : Window
    {
        private ExamService _examService;
        private ResultsService _resultsService;
        public int _courseID;
        private int _userId;
        private readonly LicenseDriverDbContext context = new LicenseDriverDbContext();
        public StudentExam(int courseID, int userId)
        {
            _courseID = courseID;
            _userId = userId;
            InitializeComponent();
            _examService = new ExamService();
            _resultsService = new ResultsService();
            LoadExams();
        }

        // Load tất cả kỳ thi vào DataGrid
        private void LoadExams()
        {
            try
            {
                var exams = _examService.GetAllExams().Where(a => a.CourseId == _courseID);

                examDataGrid.ItemsSource = exams;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading exams: " + ex.Message);
            }
        }

        private void examDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (examDataGrid.SelectedItem is not null)
            {
                int selectedExamId = (int)examDataGrid.SelectedItem.GetType().GetProperty("ExamId")?.GetValue(examDataGrid.SelectedItem);
                var _result = context.Results.Where(r => r.ExamId == selectedExamId).Where(r => r.UserId == _userId).FirstOrDefault();

                if (_result != null)
                {
                    txtScore.Text = _result.Score.ToString();
                    txtStatus.Text = _result.Status;
                    txtNote.Text = _result.Notes;
                }
                else
                {
                    txtScore.Text = "N/A";
                    txtStatus.Text = "N/A";
                    txtNote.Text = "N/A";
                }
            }
        }
    }
}
