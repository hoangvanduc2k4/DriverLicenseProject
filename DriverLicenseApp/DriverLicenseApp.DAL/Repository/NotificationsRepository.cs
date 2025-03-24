using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp.DAL.Repository
{
    public class NotificationsRepository
    {
        public static List<Notification> GetAllNotifications(int userId)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                return db.Notifications
                         .Where(n => n.UserId == userId)
                         .OrderByDescending(n => n.SentDate)
                         .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching notifications: " + ex.Message);
            }
        }

        // Thêm thông báo mới
        public static void AddNotification(Notification notification)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                db.Notifications.Add(notification);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding notification: " + ex.Message);
            }
        }

        public static bool UpdateNotification(Notification _notification)
        {
            LicenseDriverDbContext context = new();
            var notification = context.Notifications.FirstOrDefault(u => u.NotificationId == _notification.NotificationId);
            if (notification == null)
            {
                return false;
            }
            notification.IsRead = _notification.IsRead;
            context.SaveChanges();
            return true;
        }
    }
}
