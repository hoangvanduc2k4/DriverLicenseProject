using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Models;

namespace DriverLicenseApp.DAL.Repository
{
    public class CertificatesRepository
    {
        public static void InsertCertificate(int userId)
        {
            try
            {
                using var db = new LicenseDriverDbContext();

                var certificate = new Certificate
                {
                    UserId = userId,
                    IssuedDate = DateOnly.FromDateTime(DateTime.Now), // Chuyển đổi DateTime -> DateOnly
                    ExpirationDate = DateOnly.FromDateTime(DateTime.Now.AddYears(5)), // Chuyển đổi DateTime -> DateOnly
                    CertificateCode = GenerateCertificateCode(userId),
                    Status = "Inactive"
                };

                db.Certificates.Add(certificate);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Error inserting certificate: " + ex.Message);
            }
        }

        // Hàm sinh mã chứng chỉ ngẫu nhiên (UserID + Random 4 số)
        private static string GenerateCertificateCode(int userId)
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 9999);
            return $"CERT-{userId}-{randomNumber}";
        }
    }
}
