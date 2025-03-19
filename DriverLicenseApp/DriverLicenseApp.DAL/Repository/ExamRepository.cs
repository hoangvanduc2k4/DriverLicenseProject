using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApp.DAL.Repository
{
    public class ExamRepository
    {
        public static List<Exam> GetAllExams()
        {
            using (var context = new LicenseDriverDbContext())
            {
                return context.Exams.Include(c => c.Course).Include(u => u.User).ToList();
            }
        }
    }
}
