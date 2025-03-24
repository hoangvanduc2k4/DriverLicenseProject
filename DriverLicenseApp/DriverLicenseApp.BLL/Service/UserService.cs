using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;

namespace DriverLicenseApp.BLL.Service
{
    public class UserService
    {
        public List<User> GetAllUsers()
        {
            return UserRepository.GetAllUsers();
        }

        public List<User> GetAllTeacher()
        {
            return UserRepository.GetAllTeacher();
        }

        // Lấy thông tin người dùng theo userId
        public User GetUserProfile(int userId)
        {
            return UserRepository.GetUserById(userId);
        }

        // Cập nhật thông tin người dùng
        public bool UpdateUserProfile(User updatedUser)
        {
            // Thực hiện validation cơ bản
            if (string.IsNullOrWhiteSpace(updatedUser.FullName) || string.IsNullOrWhiteSpace(updatedUser.Email))
            {
                throw new ArgumentException("FullName and Email are required.");
            }

            return UserRepository.UpdateUser(updatedUser);
        }


        // Thêm người dùng mới
        public bool AddUser(User newUser)
        {
            // Validation cơ bản
            if (newUser == null)
                throw new ArgumentNullException(nameof(newUser));

            if (string.IsNullOrWhiteSpace(newUser.FullName) || string.IsNullOrWhiteSpace(newUser.Email))
            {
                throw new ArgumentException("FullName and Email are required.");
            }

            return UserRepository.AddUser(newUser);
        }

        // Cập nhật thông tin người dùng (UpdateUser mới)
        public bool UpdateUser(User updatedUser)
        {
            // Validation cơ bản cho cập nhật
            if (updatedUser == null)
                throw new ArgumentNullException(nameof(updatedUser));

            if (updatedUser.UserId <= 0)
            {
                throw new ArgumentException("A valid UserId is required for update.");
            }

            if (string.IsNullOrWhiteSpace(updatedUser.FullName) || string.IsNullOrWhiteSpace(updatedUser.Email))
            {
                throw new ArgumentException("FullName and Email are required.");
            }

            return UserRepository.UpdateUser(updatedUser);
        }


    }
}
