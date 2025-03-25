using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApp.DAL.Repository
{
    public class ResultsRepository
    {
        // Lấy danh sách kết quả theo ExamID, include User để lấy FullName
        public static List<Result> GetResultsByExamId(int examId)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                var results = from u in db.Users
                              join reg in db.Registrations on u.UserId equals reg.UserId
                              join c in db.Courses on reg.CourseId equals c.CourseId
                              join e in db.Exams on c.CourseId equals e.CourseId
                              join r in db.Results on new { u.UserId, e.ExamId } equals new { r.UserId, r.ExamId } into resultGroup
                              from r in resultGroup.DefaultIfEmpty()
                              where reg.Status == "Approved" && e.ExamId == examId
                              select new Result
                              {
                                  UserId = u.UserId,
                                  ExamId = examId,
                                  Score = r != null ? r.Score : 0,           // Giá trị mặc định nếu null
                                  Status = r != null ? r.Status : "Not Pass", // Giá trị mặc định
                                  Notes = r != null ? r.Notes : "",           // Giá trị mặc định
                                  User = u                                   // Gán User để lấy FullName
                              };

                return results.ToList();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving results: {ex.Message}");
            }
        }

        // Thêm mới hoặc cập nhật điểm
        public static bool SaveResult(Result result)
        {
            using var db = new LicenseDriverDbContext();
            var existingResult = db.Results.FirstOrDefault(r => r.UserId == result.UserId && r.ExamId == result.ExamId);

            if (existingResult == null)
            {
                db.Results.Add(result);
            }
            else
            {
                existingResult.Score = result.Score;
                existingResult.Status = result.Status;
                existingResult.Notes = result.Notes;
            }

            return db.SaveChanges() > 0; // Trả về true nếu có thay đổi dữ liệu
        }
        public static List<Result> GetResultsDetail()
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                return db.Results.Include(c => c.Exam.Course).Include(u => u.Exam.User).Include(e => e.Exam).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
