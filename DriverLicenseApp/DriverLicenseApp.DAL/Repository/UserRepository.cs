using System;
using System.Collections.Generic;
using System.Linq;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp.DAL.Repository
{

    public class UserRepository
    {
        public static List<User> GetAllUsers()
        {
            using (var context = new LicenseDriverDbContext())
            {
                return context.Users.ToList();
            }
        }

        public static List<User> GetAllTeacher()
        {
            using (var context = new LicenseDriverDbContext())
            {
                return context.Users.Where(x => x.Role == 2).ToList();
            }
        }

        // Tìm kiếm người dùng theo UserID
        public static User GetUserById(int userId)
        {
            using (var context = new LicenseDriverDbContext())
            {
                return context.Users.FirstOrDefault(u => u.UserId == userId);
            }
        }

        // Thêm người dùng mới
        public static bool AddUser(User newUser)
        {
            if (newUser == null)
                throw new ArgumentNullException(nameof(newUser));

            using (var context = new LicenseDriverDbContext())
            {
                context.Users.Add(newUser);
                context.SaveChanges();
                return true;
            }
        }

        public static bool UpdateUser(User updatedUser)
        {
            if (updatedUser == null)
                throw new ArgumentNullException(nameof(updatedUser));

            using (var context = new LicenseDriverDbContext())
            {
                var user = context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
                if (user == null)
                {
                    return false; // Không tìm thấy người dùng cần cập nhật
                }

                // Cập nhật các trường dữ liệu, bao gồm Password
                user.FullName = updatedUser.FullName;
                user.Email = updatedUser.Email;
                user.Phone = updatedUser.Phone;
                user.Class = updatedUser.Class;
                user.School = updatedUser.School;
                user.Password = updatedUser.Password; // <-- Thêm dòng này để cập nhật password
                user.Role = updatedUser.Role;

                context.SaveChanges();
                return true;
            }
        }

        public static bool IsEmailTaken(string email, int currentUserId)
        {
            LicenseDriverDbContext context = new LicenseDriverDbContext();
            try
            {
                return context.Users.Any(u => u.Email == email && u.UserId != currentUserId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error checking email: {ex.Message}");
            }
        }


        public interface IStatisticsRepository
        {
            Dictionary<string, object> GetStatistics();
        }

        public class StatisticsRepository : IStatisticsRepository
        {
            private readonly LicenseDriverDbContext _context;

            public StatisticsRepository(LicenseDriverDbContext context)
            {
                _context = context;
            }

            public Dictionary<string, object> GetStatistics()
            {
                var stats = new Dictionary<string, object>();

                // --- Thống kê Người dùng ---
                stats["TotalUsers"] = _context.Users.Count();
                stats["TotalStudents"] = _context.Users.Count(u => u.Role == 1);
                stats["TotalTeachers"] = _context.Users.Count(u => u.Role == 2);
                stats["TotalTrafficPolice"] = _context.Users.Count(u => u.Role == 3);
                stats["TotalAdmins"] = _context.Users.Count(u => u.Role == 4);

                // --- Thống kê Khóa học ---
                stats["TotalCourses"] = _context.Courses.Count();
                stats["ActiveCourses"] = _context.Courses.Count(c => c.Status == "Active");
                stats["ClosedCourses"] = _context.Courses.Count(c => c.Status == "Closed");
                stats["CancelledCourses"] = _context.Courses.Count(c => c.Status == "Cancelled");

                // --- Thống kê Đăng ký ---
                stats["ApprovedRegistrations"] = _context.Registrations.Count(r => r.Status == "Approved");
                stats["PendingRegistrations"] = _context.Registrations.Count(r => r.Status == "Pending");
                stats["RejectedRegistrations"] = _context.Registrations.Count(r => r.Status == "Rejected");

                var today = DateOnly.FromDateTime(DateTime.Today);
                // So sánh
                stats["UpcomingExams"] = _context.Exams.Count(e => e.ExamDate >= today);
                stats["PastExams"] = _context.Exams.Count(e => e.ExamDate < today);

                decimal avgScore = _context.Results.Any()
     ? _context.Results.Average(r => r.Score)
     : 0m;
                stats["AverageScore"] = avgScore;


                // Tỷ lệ đỗ: số kết quả "Pass" chia cho tổng số kết quả
                int totalResults = _context.Results.Count();
                int passResults = _context.Results.Count(r => r.Status == "Pass");
                stats["PassRate"] = (totalResults > 0) ? (double)passResults / totalResults * 100 : 0.0;

                // --- Thống kê Chứng chỉ ---
                stats["ActiveCertificates"] = _context.Certificates.Count(c => c.Status == "Active");
                stats["InactiveCertificates"] = _context.Certificates.Count(c => c.Status == "Inactive");
                // Đếm thông báo "đã đọc" (IsRead = true)
                stats["ReadNotifications"] = _context.Notifications.Count(n => n.IsRead == true);

                // Đếm thông báo "chưa đọc" (IsRead = false hoặc null, tùy logic)
                stats["UnreadNotifications"] = _context.Notifications.Count(n => n.IsRead == false);

                return stats;
            }
        }
    }
}
