using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverLicenseApp.DAL.Repository;

namespace DriverLicenseApp.BLL.Service
{
    public class CertificatesService
    {
        public void InsertCertificate(int userId)
        {
            CertificatesRepository.InsertCertificate(userId);
        }
    }
}
