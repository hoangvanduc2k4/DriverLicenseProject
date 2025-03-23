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
                return db.Results.Include(u => u.User).Include(e => e.Exam).Where(r => r.ExamId == examId).ToList();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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
