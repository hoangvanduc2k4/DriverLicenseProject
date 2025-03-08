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
    }
}
