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
        public List<Exam> GetAllExamsWithResults(int uID)
        {
            return ExamRepository.GetAllExamsByStudentWithResults(uID);
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

        // Phương thức tìm kiếm theo nhiều tiêu chí
        public List<Exam> SearchExams(int? courseId, DateTime? examDate, TimeSpan? examTime, int? durationMinutes, string room, int? teacherId)
        {
            var exams = ExamRepository.GetAllExams();

            if (courseId.HasValue)
            {
                exams = exams.Where(e => e.Course.CourseId == courseId.Value).ToList();
            }
            if (examDate.HasValue)
            {
                var dateOnly = DateOnly.FromDateTime(examDate.Value);
                exams = exams.Where(e => e.ExamDate == dateOnly).ToList();
            }
            if (examTime.HasValue)
            {
                var timeOnly = TimeOnly.FromTimeSpan(examTime.Value);
                exams = exams.Where(e => e.ExamTime == timeOnly).ToList();
            }
            if (durationMinutes.HasValue)
            {
                exams = exams.Where(e => e.DurationMinutes == durationMinutes.Value).ToList();
            }
            if (!string.IsNullOrWhiteSpace(room))
            {
                exams = exams.Where(e => e.Room.IndexOf(room, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            }
            if (teacherId.HasValue)
            {
                exams = exams.Where(e => e.User.UserId == teacherId.Value).ToList();
            }
            return exams;
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

        public bool IsExamReferenced(int examId)
        {
            using (var context = new LicenseDriverDbContext())
            {
                // Kiểm tra nếu có bất kỳ kết quả nào trong bảng Results tham chiếu đến examId
                return context.Results.Any(r => r.ExamId == examId);
            }
        }

    }
}
