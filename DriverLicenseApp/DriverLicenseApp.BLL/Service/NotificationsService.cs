using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;

namespace DriverLicenseApp.BLL.Service
{
    public class NotificationsService
    {
        // Lấy danh sách thông báo của một user
        public List<Notification> GetAllNotifications(int userId)
        {
            return NotificationsRepository.GetAllNotifications(userId);
        }

        // Thêm thông báo mới
        public void AddNotification(int userId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Message = message,
                SentDate = DateTime.Now,
                IsRead = false
            };

            NotificationsRepository.AddNotification(notification);
        }

    }
}
