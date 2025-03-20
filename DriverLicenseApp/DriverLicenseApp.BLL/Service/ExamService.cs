using System;
using System.Collections.Generic;
using System.Linq;
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

            return exams.Where(x => x.User.FullName.ToLower().Contains(teacherName.ToLower())).ToList();
        }

        public List<Exam> GetAllExamsForAllPolice()
        {
            return ExamRepository.GetAllExamsWithDetails();
        }

        public void AddExam(Exam exam)
        {
            ExamRepository.AddExam(exam);
        }

        public void UpdateExam(Exam exam)
        {
            ExamRepository.UpdateExam(exam);
        }

        public void DeleteExam(int examId)
        {
            ExamRepository.DeleteExam(examId);
        }
    }
}
