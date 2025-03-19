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

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Window
    {
        private string UserRole;

        public DashBoard(string role)
        {
            InitializeComponent();
            UserRole = role;
            SetupMenu();
        }
        private void SetupMenu()
        {

            // Xóa tất cả các nút trước khi thêm mới
            StackPanelMenu.Children.Clear();
            // Nút chung cho tất cả role
            AddMenuButton("Profile", Profile_Click);
            AddMenuButton("Logout", Logout_Click);

            // Hiển thị nút theo từng role
            switch (UserRole)
            {
                case "Student":
                    AddMenuButton("Take Exam", TakeExam_Click);
                    break;
                case "Teacher":
                    AddMenuButton("Manage Students", ManageStudents_Click);
                    AddMenuButton("Create Exam", CreateExam_Click);
                    break;
                case "TrafficPolice":
                    AddMenuButton("Check License", CheckLicense_Click);
                    break;
                case "Admin":
                    AddMenuButton("Manage Users", ManageUsers_Click);
                    AddMenuButton("System Settings", SystemSettings_Click);
                    break;
            }
        }
        private void AddMenuButton(string text, RoutedEventHandler clickEvent)
        {
            Button button = new Button
            {
                Content = text,
                Width = 200,
                Height = 50,
                Margin = new Thickness(10),
            };
            button.Click += clickEvent;
            StackPanelMenu.Children.Add(button);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new ProfileWindom());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void TakeExam_Click(object sender, RoutedEventArgs e)
        {
            //OpenWindow(new TakeExamWindow());
        }

        private void ManageStudents_Click(object sender, RoutedEventArgs e)
        {
            //OpenWindow(new ManageStudentsWindow());
        }

        private void CreateExam_Click(object sender, RoutedEventArgs e)
        {
            //OpenWindow(new CreateExamWindow());
        }

        private void CheckLicense_Click(object sender, RoutedEventArgs e)
        {
            //OpenWindow(new CheckLicenseWindow());
        }

        private void ManageUsers_Click(object sender, RoutedEventArgs e)
        {
            //OpenWindow(new ManageUsersWindow());
        }

        private void SystemSettings_Click(object sender, RoutedEventArgs e)
        {
            //OpenWindow(new SystemSettingsWindow());
        }
        private void OpenWindow(Window window)
        {
            this.Hide(); // Ẩn Dashboard
            window.ShowDialog(); // Mở màn hình con và chờ đóng
            this.Show(); // Hiển thị lại Dashboard sau khi đóng
        }
    }
}
