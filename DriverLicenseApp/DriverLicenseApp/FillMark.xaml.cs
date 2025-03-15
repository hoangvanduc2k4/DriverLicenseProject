using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for FillMark.xaml
    /// </summary>
    public partial class FillMark : Window
    {
        private readonly ResultsService _resultsService = new ResultsService();
        private readonly CertificatesService _certificatesService = new CertificatesService();
        private readonly NotificationsService _notificationsService = new NotificationsService();
        public int _examId;

        public FillMark(int examId)
        {
            InitializeComponent();
            _examId = examId;
            LoadStudents();
        }

        /// <summary>
        /// Load danh sách học sinh của khóa học vào DataGrid
        /// </summary>
        private void LoadStudents()
        {
            dgStudents.ItemsSource = _resultsService.GetResults(_examId);
        }

        /// <summary>
        /// Khi chọn một học sinh, hiển thị thông tin vào TextBox
        /// </summary>
        private void dgStudents_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgStudents.SelectedItem is Result selectedResult)
            {
                txtStudentID.Text = selectedResult.UserId.ToString();
                txtFullName.Text = selectedResult.User.FullName;
                txtScore.Text = selectedResult.Score.ToString();
                txtNotes.Text = selectedResult.Notes;
            }
        }
        /// <summary>
        /// Lưu điểm số của học sinh
        /// </summary>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(txtStudentID.Text, out int userId))
            {
                MessageBox.Show("Invalid Student ID!");
                return;
            }

            if (!decimal.TryParse(txtScore.Text, out decimal score) || score < 0 || score > 130)
            {
                MessageBox.Show("Score must be between 0 and 130!");
                return;
            }
            string status = "";
            // Xác định trạng thái Pass / Not Pass
            if (score >= 107)
            {
               status = "Pass";
            }
            else
            {
                status = "Not Pass";
            }

            var result = new Result
            {
                UserId = userId,
                ExamId = _examId,
                Score = score,
                Status = status,
                Notes = txtNotes.Text
            };

            bool isSaved = _resultsService.SaveResult(result);

            if (isSaved)
            {
                // Nếu lưu điểm thành công → Chèn vào Notifications
                var notification = new Notification
                {
                    UserId = userId,
                    Message = "Your exam results have been published!",
                };

                _notificationsService.AddNotification(notification.UserId, notification.Message);

                // Nếu đậu (Pass) thì chèn vào Certificates
                if (score >= 107)
                {
                    _certificatesService.InsertCertificate(userId);
                }
            }

            MessageBox.Show("Score saved successfully!");
            LoadStudents();
        }


        /// <summary>
        /// Xuất danh sách điểm ra file CSV
        /// </summary>
        private void btnExportCSV_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var results = _resultsService.GetResults(_examId);
                if (results == null || !results.Any())
                {
                    MessageBox.Show("No data to export.");
                    return;
                }

                StringBuilder csvContent = new StringBuilder();
                csvContent.AppendLine("StudentID,Full Name,Score,Status,Notes");

                foreach (var result in results)
                {
                    csvContent.AppendLine($"{result.UserId},{result.User.FullName},{result.Score},{result.Status},{result.Notes}");
                }

                string filePath = $"Course_{_examId}_Results.csv";
                File.WriteAllText(filePath, csvContent.ToString());

                MessageBox.Show($"Exported successfully: {filePath}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting CSV: " + ex.Message);
            }
        }
    }
}
