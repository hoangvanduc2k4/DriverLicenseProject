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
    /// Interaction logic for StudentNotifications.xaml
    /// </summary>
    public partial class StudentNotifications : Window
    {
        private int _userId;
        private readonly LicenseDriverDbContext context = new LicenseDriverDbContext();
        private readonly NotificationsService _notificationsService = new NotificationsService();
        public StudentNotifications(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadNotifications();
        }

        // Load danh sách đăng ký từ Service
        private void LoadNotifications()
        {
            var _notifications = _notificationsService.GetAllNotifications(_userId);
            dgNotifications.ItemsSource = _notifications;
        }

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            if (cbStatusFilter.SelectedItem == null)
                return; // Tránh lỗi nếu dữ liệu chưa load
            int filter = Convert.ToInt32(((ComboBoxItem)cbStatusFilter.SelectedItem).Tag);
            if (filter == 0)
            {
                dgNotifications.ItemsSource = _notificationsService.GetAllNotifications(_userId)
                                    .Where(x => x.IsRead == false);
            }
            else dgNotifications.ItemsSource = _notificationsService.GetAllNotifications(_userId);
        }

        private void dgNotifications_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgNotifications.SelectedItem != null)
            {
                int selectedNotificationId = (int)dgNotifications.SelectedItem.GetType().GetProperty("NotificationId")?.GetValue(dgNotifications.SelectedItem);
                var _selectedNotification = context.Notifications.Find(selectedNotificationId) as Notification;
                if (_selectedNotification != null && _selectedNotification.IsRead == false)
                {
                    _selectedNotification.IsRead = true;
                    _notificationsService.UpdateNotification(_selectedNotification);
                    int filter = Convert.ToInt32(((ComboBoxItem)cbStatusFilter.SelectedItem).Tag);
                    if (filter == 0)
                    {
                        dgNotifications.ItemsSource = _notificationsService.GetAllNotifications(_userId)
                                            .Where(x => x.IsRead == false);
                    }
                    else dgNotifications.ItemsSource = _notificationsService.GetAllNotifications(_userId);
                }
            }
        }

    }
}
