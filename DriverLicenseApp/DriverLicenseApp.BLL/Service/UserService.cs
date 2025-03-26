using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;
using static DriverLicenseApp.DAL.Repository.UserRepository;

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

        public User GetUserProfile(int userId)
        {
            return UserRepository.GetUserById(userId);
        }

        public bool UpdateUserProfile(User updatedUser)
        {
            if (string.IsNullOrWhiteSpace(updatedUser.FullName) || string.IsNullOrWhiteSpace(updatedUser.Email))
            {
                throw new ArgumentException("FullName and Email are required.");
            }

            return UserRepository.UpdateUser(updatedUser);
        }


        public bool AddUser(User newUser)
        {
            if (newUser == null)
                throw new ArgumentNullException(nameof(newUser));

            if (string.IsNullOrWhiteSpace(newUser.FullName) || string.IsNullOrWhiteSpace(newUser.Email))
            {
                throw new ArgumentException("FullName and Email are required.");
            }

            return UserRepository.AddUser(newUser);
        }

        public bool UpdateUser(User updatedUser)
        {
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


        public bool IsEmailTaken(string email, int currentUserId)
        {
            return UserRepository.IsEmailTaken(email, currentUserId);
        }


        public interface IStatisticsService
        {
            Dictionary<string, object> GetStatistics();
        }

        public class StatisticsService : IStatisticsService
        {
            private readonly IStatisticsRepository _repository;

            public StatisticsService(IStatisticsRepository repository)
            {
                _repository = repository;
            }

            public Dictionary<string, object> GetStatistics()
            {
                return _repository.GetStatistics();
            }
        }

    }
}
