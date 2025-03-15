using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp.DAL.Repository
{
    public class UserRepository
    {
        public static List<User> GetAllUsers()
        {
            LicenseDriverDbContext context = new();
            return context.Users.ToList();
        }

        public static List<User> GetAllTeacher()
        {
            LicenseDriverDbContext context = new();
            return context.Users.Where(x => x.Role == 2).ToList();
        }

        // Tìm kiếm người dùng theo UserID
        public static User GetUserById(int userId)
        {
            LicenseDriverDbContext context = new();
            return context.Users.FirstOrDefault(u => u.UserId == userId);
        }

        // Cập nhật thông tin người dùng
        public static bool UpdateUser(User updatedUser)
        {
            LicenseDriverDbContext context = new();
            var user = context.Users.FirstOrDefault(u => u.UserId == updatedUser.UserId);
            if (user == null)
            {
                return false; // Không tìm thấy người dùng cần cập nhật
            }

            // Cập nhật các trường dữ liệu
            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;
            user.Phone = updatedUser.Phone;
            user.Class = updatedUser.Class;
            user.School = updatedUser.School;
            user.Role = updatedUser.Role;
            // Nếu cần cập nhật các trường khác (ví dụ: Password) thì thêm tại đây

            context.SaveChanges();
            return true;
        }
    }
}
