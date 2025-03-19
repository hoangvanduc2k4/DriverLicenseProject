using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;

namespace DriverLicenseApp.BLL.Service
{
    public class ExamService
    {
        public List<Exam> GetAllExams()
        {
            return ExamRepository.GetAllExams();
        }


        public List<Exam> GetExamsByTeacher(string teacherName)
        {
            var exams = ExamRepository.GetAllExams();

            if (string.IsNullOrWhiteSpace(teacherName))
                return exams;  

            return exams.Where(x => x.User.FullName.ToLower().Contains(teacherName)).ToList();
        }


    }
}
