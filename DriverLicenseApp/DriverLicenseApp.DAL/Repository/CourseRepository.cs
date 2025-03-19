using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp.DAL.Repository
{
    public class CourseRepository
    {

        public static List<Course> GetAllCourses()
        {
            var list = new List<Course>();
            try
            {
                using var db = new LicenseDriverDbContext();
                list = db.Courses.ToList();

            }catch (Exception ex) { }

            return list;
        }


        public static void AddCourse(Course course)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                db.Courses.Add(course);
                db.SaveChanges();

            }
            catch (Exception ex) {
                throw new Exception(ex.Message);           
            }

        }


        public static void UpdateCourse(Course course)
        {
            try
            {
                using var db = new LicenseDriverDbContext();
                db.Entry<Course>(course).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }



        public static void DeleteCourse(Course course)
        {
            try
            {
                using var db = new LicenseDriverDbContext();

                var d = db.Courses.SingleOrDefault(d => d.CourseId == course.CourseId);
                db.Courses.Remove(d);
               db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }





    }
}
