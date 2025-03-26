using System;
using System.Collections.Generic;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Repository;
using static DriverLicenseApp.BLL.Service.UserService;
using static DriverLicenseApp.DAL.Repository.UserRepository;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp
{
    public partial class DetailedStatistics : Window
    {
        private IStatisticsService _statisticsService;
        private bool _dataLoaded = false; 

        public DetailedStatistics()
        {
            InitializeComponent();
            var dbContext = new LicenseDriverDbContext();
            _statisticsService = new StatisticsService(new StatisticsRepository(dbContext));

            if (!_dataLoaded)
            {
                LoadStatistics();
                _dataLoaded = true;
            }
            ShowRandomGroupBox();
        }

        private void HideAllGroupBoxes()
        {
            gbUserCourseStats.Visibility = Visibility.Collapsed;
            gbRegistrationExamStats.Visibility = Visibility.Collapsed;
            gbCertificateNotificationStats.Visibility = Visibility.Collapsed;
        }

        private void ShowRandomGroupBox()
        {
            HideAllGroupBoxes();
            Random rnd = new Random();
            int randomIndex = rnd.Next(0, 3);
            switch (randomIndex)
            {
                case 0:
                    gbUserCourseStats.Visibility = Visibility.Visible;
                    break;
                case 1:
                    gbRegistrationExamStats.Visibility = Visibility.Visible;
                    break;
                case 2:
                    gbCertificateNotificationStats.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void btnUserCourseStats_Click(object sender, RoutedEventArgs e)
        {
            HideAllGroupBoxes();
            gbUserCourseStats.Visibility = Visibility.Visible;
        }

        private void btnRegistrationExamStats_Click(object sender, RoutedEventArgs e)
        {
            HideAllGroupBoxes();
            gbRegistrationExamStats.Visibility = Visibility.Visible;
        }
        private void btnCertificateNotificationStats_Click(object sender, RoutedEventArgs e)
        {
            HideAllGroupBoxes();
            gbCertificateNotificationStats.Visibility = Visibility.Visible;
        }

        private void LoadStatistics()
        {
            try
            {
                Dictionary<string, object> stats = _statisticsService.GetStatistics();
                txtTotalUsers.Text = stats["TotalUsers"].ToString();
                txtStudents.Text = stats["TotalStudents"].ToString();
                txtTeachers.Text = stats["TotalTeachers"].ToString();
                txtTrafficPolice.Text = stats["TotalTrafficPolice"].ToString();
                txtAdmins.Text = stats["TotalAdmins"].ToString();

                pieSeriesStudents.Values = new ChartValues<int> { Convert.ToInt32(stats["TotalStudents"]) };
                pieSeriesTeachers.Values = new ChartValues<int> { Convert.ToInt32(stats["TotalTeachers"]) };
                pieSeriesTraffic.Values = new ChartValues<int> { Convert.ToInt32(stats["TotalTrafficPolice"]) };
                pieSeriesAdmins.Values = new ChartValues<int> { Convert.ToInt32(stats["TotalAdmins"]) };

                txtTotalCourses.Text = stats["TotalCourses"].ToString();
                txtActiveCourses.Text = stats["ActiveCourses"].ToString();
                txtClosedCourses.Text = stats["ClosedCourses"].ToString();
                txtCancelledCourses.Text = stats["CancelledCourses"].ToString();

                colSeriesCourses.Values = new ChartValues<int>
                {
                    Convert.ToInt32(stats["ActiveCourses"]),
                    Convert.ToInt32(stats["ClosedCourses"]),
                    Convert.ToInt32(stats["CancelledCourses"])
                };

                txtApprovedRegs.Text = stats["ApprovedRegistrations"].ToString();
                txtPendingRegs.Text = stats["PendingRegistrations"].ToString();
                txtRejectedRegs.Text = stats["RejectedRegistrations"].ToString();

                pieSeriesApproved.Values = new ChartValues<int> { Convert.ToInt32(stats["ApprovedRegistrations"]) };
                pieSeriesPending.Values = new ChartValues<int> { Convert.ToInt32(stats["PendingRegistrations"]) };
                pieSeriesRejected.Values = new ChartValues<int> { Convert.ToInt32(stats["RejectedRegistrations"]) };

                txtUpcomingExams.Text = stats["UpcomingExams"].ToString();
                txtPastExams.Text = stats["PastExams"].ToString();
                txtAvgScore.Text = stats["AverageScore"].ToString();
                txtPassRate.Text = stats["PassRate"].ToString() + " %";

                lineSeriesExams.Values = new ChartValues<int> { 1, 2, 3, 2, 4, 3, 5, 4, 3, 2, 3, 4 };

                txtActiveCertificates.Text = stats["ActiveCertificates"].ToString();
                txtInactiveCertificates.Text = stats["InactiveCertificates"].ToString();

                colSeriesCertificates.Values = new ChartValues<int>
                {
                    Convert.ToInt32(stats["ActiveCertificates"]),
                    Convert.ToInt32(stats["InactiveCertificates"])
                };

                txtReadNotifications.Text = stats["ReadNotifications"].ToString();
                txtUnreadNotifications.Text = stats["UnreadNotifications"].ToString();

                pieSeriesRead.Values = new ChartValues<int> { Convert.ToInt32(stats["ReadNotifications"]) };
                pieSeriesUnread.Values = new ChartValues<int> { Convert.ToInt32(stats["UnreadNotifications"]) };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading statistics: " + ex.Message);
            }
        }
    }
}
