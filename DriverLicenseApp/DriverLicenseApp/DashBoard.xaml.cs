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

namespace DriverLicenseApp
{
    /// <summary>
    /// Interaction logic for DashBoard.xaml
    /// </summary>
    public partial class DashBoard : Window
    {
        private User currentUser;

        public DashBoard(User user)
        {
            InitializeComponent();
            currentUser = user;
            SetupMenu();
        }
        private void SetupMenu()
        {

            // Xóa tất cả các nút trước khi thêm mới
            StackPanelMenu.Children.Clear();
            // Nút chung cho tất cả role
            AddMenuButton("Profile", Profile_Click);

            // Hiển thị nút theo từng role
            switch (currentUser.Role)
            {
                //Student
                case 1:
                    AddMenuButton("My Courses", ViewCoursesOfStudent_Click);
                    break;
                //Teacher
                case 2:
                    AddMenuButton("Course Management", CourseManagement_Click);
                    AddMenuButton("List Exam", ListExam_Click);
                    break;
                //Police
                case 3:
                    AddMenuButton("Exam Management", EC_Click);
                    break;
                //Admin
                case 4:
                    AddMenuButton("User Management", EC1_Click);
                    break;
            }
            AddMenuButton("Logout", Logout_Click);
        }

        private void EC_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new ExamPolice());
        }

        private void EC1_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new ListUser());
        }

        private void AddMenuButton(string text, RoutedEventHandler clickEvent)
        {
            Button button = new Button
            {
                Content = text,
                Width = 150,
                Height = 50,
                Margin = new Thickness(10),

                Background = new SolidColorBrush(Color.FromRgb(136, 211, 187)), // Màu nền (xanh teal nhạt)
                Foreground = Brushes.White, // Màu chữ (trắng)
                BorderBrush = new SolidColorBrush(Color.FromRgb(88, 177, 159)), // Màu viền (đậm hơn nền)
                BorderThickness = new Thickness(2), // Độ dày viền
                FontWeight = FontWeights.Bold, // Độ đậm của chữ
                Cursor = Cursors.Hand, // Đổi con trỏ chuột khi hover
            };
            button.Click += clickEvent;
            StackPanelMenu.Children.Add(button);
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new Profile(currentUser.UserId));
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Login loginWindow = new Login();
            loginWindow.Show();
            this.Close();
        }
        private void CourseManagement_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new CourseManagement(currentUser.UserId));
        }
        private void ListExam_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new ListExam(currentUser.UserId));
        }

        private void ViewCoursesOfStudent_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new StudentCourses(currentUser.UserId));
        }

        private void OpenWindow(Window window)
        {
            this.Hide(); // Ẩn Dashboard
            window.ShowDialog(); // Mở màn hình con và chờ đóng
            this.Show(); // Hiển thị lại Dashboard sau khi đóng
        }
    }
}
