using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApp.DAL.Repository
{
    public class RegistrationRepository
    {
        public static List<Registration> GetAllRegistrations()
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                return db.Registrations
                         .Include(r => r.User)    // Load thông tin User
                         .Include(r => r.Course)  // Load thông tin Course
                         .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching registrations: " + ex.Message);
            }
        }

        // Lấy danh sách đăng ký của một khóa học cụ thể
        public static List<Registration> GetRegistrationsByCourse(int courseId)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                return db.Registrations
                         .Where(r => r.CourseId == courseId)
                         .Include(r => r.User)    // Load thông tin User
                         .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching registrations for course: " + ex.Message);
            }
        }

        // Thêm một đăng ký mới
        public static void AddRegistration(Registration registration)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                db.Registrations.Add(registration);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding registration: " + ex.Message);
            }
        }

        // Cập nhật trạng thái đăng ký (Approve / Reject)
        public static void UpdateRegistrationStatus(int registrationId, string status, string comments = null)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                var registration = db.Registrations.SingleOrDefault(r => r.RegistrationId == registrationId);
                if (registration != null)
                {
                    registration.Status = status;
                    registration.Comments = comments;
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating registration status: " + ex.Message);
            }
        }

        // Xóa một đăng ký
        public static void DeleteRegistration(int registrationId)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                var registration = db.Registrations.SingleOrDefault(r => r.RegistrationId == registrationId);
                if (registration != null)
                {
                    db.Registrations.Remove(registration);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error deleting registration: " + ex.Message);
            }
        }

    }
}
