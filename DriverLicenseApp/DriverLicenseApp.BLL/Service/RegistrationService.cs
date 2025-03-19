using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;
using DriverLicenseApp.DAL.Repository;

namespace DriverLicenseApp.BLL.Service
{
    public class RegistrationService
    {
        // Lấy danh sách tất cả đăng ký
        public List<Registration> GetAllRegistrations()
        {
            return RegistrationRepository.GetAllRegistrations();
        }

        // Lấy danh sách đăng ký theo khóa học
        public List<Registration> GetRegistrationsByCourse(int courseId)
        {
            return RegistrationRepository.GetRegistrationsByCourse(courseId);
        }

        // Thêm đăng ký mới
        public void AddRegistration(Registration registration)
        {
            RegistrationRepository.AddRegistration(registration);
        }

        // Cập nhật trạng thái đăng ký (Approve / Reject)
        public void UpdateRegistrationStatus(int registrationId, string status, string comments = null)
        {
            RegistrationRepository.UpdateRegistrationStatus(registrationId, status, comments);
        }

        // Xóa đăng ký
        public void RemoveRegistration(int registrationId)
        {
            RegistrationRepository.DeleteRegistration(registrationId);
        }
    }
}
