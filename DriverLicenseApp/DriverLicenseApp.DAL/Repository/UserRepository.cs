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
    }
}
