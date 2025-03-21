using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using Microsoft.Win32; 
namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for FillMark.xaml
    /// </summary>
    public partial class FillMark : Window
    {
        private readonly ExamService _examService = new ExamService();
        private readonly ResultsService _resultsService = new ResultsService();
        private readonly CertificatesService _certificatesService = new CertificatesService();
        private readonly NotificationsService _notificationsService = new NotificationsService();
        public int _examId;

        public FillMark(int examId)
        {
            InitializeComponent();
            _examId = examId;
            string title = _examService.GetAllExams().FirstOrDefault(x => x.ExamId == _examId).Course.CourseName;
            lblName.Content = title;
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

                // Tạo SaveFileDialog
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                saveFileDialog.FileName = $"Course_{lblName.Content}_Results.csv"; // Tên file mặc định
                saveFileDialog.Title = "Save CSV File";

                // Hiển thị dialog và kiểm tra nếu người dùng chọn OK
                if (saveFileDialog.ShowDialog() == true)
                {
                    StringBuilder csvContent = new StringBuilder();
                    csvContent.AppendLine("StudentID,Full Name,Score,Status,Notes");

                    foreach (var result in results)
                    {
                        csvContent.AppendLine($"{result.UserId},{result.User.FullName},{result.Score},{result.Status},{result.Notes}");
                    }

                    // Ghi file vào vị trí người dùng chọn
                    File.WriteAllText(saveFileDialog.FileName, csvContent.ToString());
                    MessageBox.Show($"Exported successfully: {saveFileDialog.FileName}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting CSV: " + ex.Message);
            }
        }

        private void btnImportCSV_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Select a CSV file to import results"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (var reader = new StreamReader(openFileDialog.FileName))
                    {
                        // Bỏ qua header
                        bool isFirstLine = true;

                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            if (isFirstLine)
                            {
                                isFirstLine = false;
                                continue; // Bỏ qua dòng header: StudentId,Name,Score,Notes
                            }

                            // Split dòng CSV với 4 cột
                            var values = line.Split(',');

                            if (values.Length < 3) // Cần ít nhất StudentId, Score
                            {
                                MessageBox.Show($"Invalid data format in line: {line}");
                                continue;
                            }

                            // Validate StudentId (UserId)
                            if (!int.TryParse(values[0].Trim(), out int userId))
                            {
                                MessageBox.Show($"Invalid Student ID in line: {line}");
                                continue;
                            }

                            // Validate Score
                            if (!decimal.TryParse(values[2].Trim(), out decimal score) || score < 0 || score > 130)
                            {
                                MessageBox.Show($"Score must be between 0 and 130 in line: {line}");
                                continue;
                            }

                            // Xác định trạng thái Pass/Not Pass
                            string status = score >= 107 ? "Pass" : "Not Pass";

                            // Lấy Notes nếu có, nếu không thì để empty
                            string notes = values.Length > 3 ? values[3].Trim() : "";

                            // Tạo object Result
                            var result = new Result
                            {
                                UserId = userId,
                                ExamId = _examId,
                                Score = score,
                                Status = status,
                                Notes = notes
                            };

                            // Sử dụng hàm SaveResult có sẵn
                            bool isSaved = _resultsService.SaveResult(result);

                            if (isSaved)
                            {
                                // Thêm notification
                                var notification = new Notification
                                {
                                    UserId = userId,
                                    Message = "Your exam results have been published!"
                                };
                                _notificationsService.AddNotification(notification.UserId, notification.Message);

                                // Nếu Pass thì thêm certificate
                                if (score >= 107)
                                {
                                    _certificatesService.InsertCertificate(userId);
                                }
                            }
                        }
                    }

                    MessageBox.Show("Import completed successfully!");
                    LoadStudents(); // Refresh DataGrid
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during import: {ex.Message}");
                }
            }
        }
    }
}
