using System;
using System.Collections.Generic;
using System.Linq;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApp.DAL.Repository
{
    public class ExamRepository
    {
        private readonly LicenseDriverDbContext _context = new LicenseDriverDbContext();

        public static List<Exam> GetAllExams()
        {
            using (var context = new LicenseDriverDbContext())
            {
                return context.Exams.Include(c => c.Course).Include(u => u.User).ToList();
            }
        }

        public static List<Exam> GetAllExamsWithDetails()
        {
            using (var context = new LicenseDriverDbContext())
            {
                return context.Exams.Include(e => e.Course).Include(e => e.User).Select(e => new Exam
                {
                    ExamId = e.ExamId,
                    ExamDate = e.ExamDate,
                    ExamTime = e.ExamTime,
                    DurationMinutes = e.DurationMinutes,
                    Room = e.Room,
                    Course = new Course { CourseName = e.Course.CourseName },
                    User = new User { FullName = e.User.FullName }
                }
                ).ToList();
            }
        }

        public static void AddExam(Exam exam)
        {
            using (var context = new LicenseDriverDbContext())
            {
                context.Exams.Add(exam);
                context.SaveChanges();
            }
        }

        public static void UpdateExam(Exam exam)
        {
            using (var context = new LicenseDriverDbContext())
            {
                context.Exams.Update(exam);
                context.SaveChanges();
            }
        }

        public static void DeleteExam(int examId)
        {
            using (var context = new LicenseDriverDbContext())
            {
                var exam = context.Exams.Find(examId);
                if (exam != null)
                {
                    context.Exams.Remove(exam);
                    context.SaveChanges();
                }
            }
        }

        public static List<Exam> GetAllExamsByStudentWithResults(int uId)
        {
            using (var context = new LicenseDriverDbContext())
            {
                return context.Exams
                           .Where(e => e.Results.Any(r => r.UserId == uId) || !e.Results.Any())
                           .ToList();
            }
        }
    }
}
