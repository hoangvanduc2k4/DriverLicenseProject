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

                    break;
                //Teacher
                case 2:
                    AddMenuButton("Course Management", CourseManagement_Click);
                    AddMenuButton("List Exam", ListExam_Click);
                    break;
                //Police
                case 3:

                    break;
                //Admin
                case 4:

                    break;
            }
            AddMenuButton("Logout", Logout_Click);
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
            OpenWindow(new CourseManagement());
        }
        private void ListExam_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new ListExam());
        }

        private void OpenWindow(Window window)
        {
            this.Hide(); // Ẩn Dashboard
            window.ShowDialog(); // Mở màn hình con và chờ đóng
            this.Show(); // Hiển thị lại Dashboard sau khi đóng
        }
    }
}
