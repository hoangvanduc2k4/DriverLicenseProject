using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for StudentCertificates.xaml
    /// </summary>
    public partial class StudentCertificates : Window
    {
        private int _userId;
        private readonly LicenseDriverDbContext context = new LicenseDriverDbContext();
        public StudentCertificates(int userId)
        {
            InitializeComponent();
            _userId = userId;
            LoadDataGridCertificates();
        }

        public void LoadDataGridCertificates()
        {
            var certificates = context.Certificates
                .Where(r => r.UserId == _userId)
                .ToList();
            certificatesDataGrid.ItemsSource = certificates;
        }
    }
}
