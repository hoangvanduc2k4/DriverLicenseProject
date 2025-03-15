using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;
using Microsoft.Identity.Client;

namespace DriverLicenseApp.BLL.Service
{
    public class CourseService
    {
        

        public List<Course> GetCourse()
        {
            return CourseRepository.GetAllCourses();
        }


        public void AddCourse(Course course)
        {
            CourseRepository.AddCourse(course);
        }


        public void RemoveCourse(Course course)
        {
            CourseRepository.DeleteCourse(course);
        }


        public void UpdateCourse(Course course)
        {
            CourseRepository.UpdateCourse(course);
        }
    }
}
