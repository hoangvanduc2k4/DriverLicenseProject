﻿using DriverLicenseApp.DAL.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DriverLicenseApp
{
    public partial class CertificateManage : Window
    {
        private int currentUserId = 0;

        public CertificateManage()
        {
            InitializeComponent();
            LoadCertificates();
        }

        private void LoadCertificates()
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                var query = from cert in db.Certificates
                            join user in db.Users on cert.UserId equals user.UserId
                            select new
                            {
                                CertificateID = cert.CertificateId,
                                UserID = user.UserId, 
                                UserName = user.FullName,
                                IssuedDate = cert.IssuedDate.ToString("yyyy-MM-dd"),
                                ExpirationDate = cert.ExpirationDate.ToString("yyyy-MM-dd"),
                                CertificateCode = cert.CertificateCode,
                                StatusDisplay = cert.Status.ToLower() == "active" ? "Certificate Issued" : "Certificate Not Issued",
                                Status = cert.Status.ToLower()
                            };

                var list = query.ToList();
                if (!string.IsNullOrWhiteSpace(txtSearchUserName.Text))
                {
                    list = list.Where(x => x.UserName.IndexOf(txtSearchUserName.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }
                if (dpSearchIssuedDate.SelectedDate.HasValue)
                {
                    string searchIssued = dpSearchIssuedDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                    list = list.Where(x => x.IssuedDate == searchIssued).ToList();
                }
                if (dpSearchExpirationDate.SelectedDate.HasValue)
                {
                    string searchExp = dpSearchExpirationDate.SelectedDate.Value.ToString("yyyy-MM-dd");
                    list = list.Where(x => x.ExpirationDate == searchExp).ToList();
                }
                if (!string.IsNullOrWhiteSpace(txtSearchCertificateCode.Text))
                {
                    list = list.Where(x => x.CertificateCode.IndexOf(txtSearchCertificateCode.Text, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                }
                if (cbSearchStatus.SelectedItem is ComboBoxItem statusItem &&
                    statusItem.Tag != null &&
                    !string.IsNullOrEmpty(statusItem.Tag.ToString()))
                {
                    string statusFilter = statusItem.Tag.ToString();
                    list = list.Where(x => x.Status == statusFilter).ToList();
                }

                dgCertificates.ItemsSource = list;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading certificates: " + ex.Message);
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCertificates();
        }

        private void ResetSearchButton_Click(object sender, RoutedEventArgs e)
        {
            txtSearchUserName.Text = "";
            dpSearchIssuedDate.SelectedDate = null;
            dpSearchExpirationDate.SelectedDate = null;
            txtSearchCertificateCode.Text = "";
            if (cbSearchStatus.SelectedIndex != 0)
                cbSearchStatus.SelectedIndex = 0;
            LoadCertificates();
        }

        private void ViewDetailButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = dgCertificates.SelectedItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Please select a row to view details.");
                return;
            }

            dynamic item = selectedItem;
            int userId = item.UserID;
            currentUserId = userId;

            LoadDetail(userId);

            ListPanel.Visibility = Visibility.Collapsed;
            DetailPanel.Visibility = Visibility.Visible;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            DetailPanel.Visibility = Visibility.Collapsed;
            ListPanel.Visibility = Visibility.Visible;
        }

        private void IssueCertificateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                var certificate = db.Certificates.FirstOrDefault(c => c.UserId == currentUserId);
                if (certificate != null)
                {
                    certificate.Status = "Active";

                    var notification = new Notification
                    {
                        UserId = certificate.UserId,
                        Message = "Congratulations! Your driving certificate has been issued.",
                        SentDate = DateTime.Now,
                        IsRead = false
                    };

                    db.Notifications.Add(notification);
                    db.SaveChanges();

                    MessageBox.Show("Certificate has been issued successfully.");
                    LoadDetail(currentUserId);
                }
                else
                {
                    MessageBox.Show("Certificate to update not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error issuing certificate: " + ex.Message);
            }
        }

        private void LoadDetail(int userId)
        {
            try
            {
                using var db = new LicenseDriverDbContext();

                var certificate = db.Certificates.FirstOrDefault(c => c.UserId == userId);
                if (certificate != null)
                {
                    txtCertificateInfo.Text = $"Issued Date: {certificate.IssuedDate:yyyy-MM-dd}\n" +
                                              $"Expiration Date: {certificate.ExpirationDate:yyyy-MM-dd}\n" +
                                              $"Certificate Code: {certificate.CertificateCode}\n" +
                                              $"Status: {(certificate.Status.ToLower() == "active" ? "Certificate Issued" : "Certificate Not Issued")}";
                }
                else
                {
                    txtCertificateInfo.Text = "Certificate information not found.";
                }

                var results = db.Results.Where(r => r.UserId == userId).ToList();
                if (results.Any())
                {
                    txtResultInfo.Text = string.Join("\n\n", results.Select((r, index) =>
                        $"Result {index + 1}:\n" +
                        $"Score: {r.Score}\n" +
                        $"Status: {r.Status}\n" +
                        $"Notes: {r.Notes}"));
                }
                else
                {
                    txtResultInfo.Text = "No exam results available.";
                }

                var examInfos = (from r in db.Results
                                 join ex in db.Exams on r.ExamId equals ex.ExamId
                                 where r.UserId == userId
                                 select new
                                 {
                                     ex.ExamDate,
                                     ex.ExamTime,
                                     ex.DurationMinutes,
                                     ex.Room
                                 }).ToList();

                if (examInfos.Any())
                {
                    txtExamInfo.Text = string.Join("\n\n", examInfos.Select((e, index) =>
                        $"Exam {index + 1}:\n" +
                        $"Exam Date: {e.ExamDate:yyyy-MM-dd}\n" +
                        $"Exam Time: {e.ExamTime}\n" +
                        $"Duration: {e.DurationMinutes} minutes\n" +
                        $"Room: {e.Room}"));
                }
                else
                {
                    txtExamInfo.Text = "No exam information available.";
                }

                var courseInfos = (from reg in db.Registrations
                                   join course in db.Courses on reg.CourseId equals course.CourseId
                                   join teacher in db.Users on course.TeacherId equals teacher.UserId
                                   where reg.UserId == userId && reg.Status == "Approved"
                                   select new
                                   {
                                       course.CourseName,
                                       course.StartDate,
                                       course.EndDate,
                                       course.Status,
                                       TeacherName = teacher.FullName
                                   }).ToList();


                if (courseInfos.Any())
                {
                    txtCourseInfo.Text = string.Join("\n\n", courseInfos.Select((c, index) =>
                        $"Course {index + 1}:\n" +
                        $"Course Name: {c.CourseName}\n" +
                        $"Teacher: {c.TeacherName}\n" +
                        $"Start Date: {c.StartDate:yyyy-MM-dd}\n" +
                        $"End Date: {c.EndDate:yyyy-MM-dd}\n" +
                        $"Status: {c.Status}"));
                }
                else
                {
                    txtCourseInfo.Text = "No course information available.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading details: " + ex.Message);
            }
        }
    }
}
