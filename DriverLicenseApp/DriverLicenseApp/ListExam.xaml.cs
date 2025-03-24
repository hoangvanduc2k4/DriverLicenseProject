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
            DateOnly? examDateFilter = dpExamDate.SelectedDate.HasValue ? DateOnly.FromDateTime(dpExamDate.SelectedDate.Value) : null;

            try
            {
                var filteredExams = _examService.GetAllExams().Where(x => x.UserId == _userId);

                if (!string.IsNullOrEmpty(courseNameFilter))
                {
                    filteredExams = filteredExams.Where(x => x.Course.CourseName.ToLower().Contains(courseNameFilter.ToLower()));
                }

                if (examDateFilter.HasValue)
                {
                    filteredExams = filteredExams.Where(x => x.ExamDate >= examDateFilter);
                }

                examDataGrid.ItemsSource = filteredExams.ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering exams: " + ex.Message);
            }
        }



        private void FillMarkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (examDataGrid.SelectedItem == null)
                {
                    MessageBox.Show("Please select an exam.");
                    return;
                }

                dynamic selectedExam = examDataGrid.SelectedItem;
                int examId = selectedExam.ExamId;

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
