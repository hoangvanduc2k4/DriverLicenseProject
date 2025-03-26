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

    }
}
