using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DriverLicenseApp.BLL.Service;
using DriverLicenseApp.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DriverLicenseApp
{
    public partial class ChatBot : Window
    {
        private static readonly HttpClient client = new HttpClient();
        private const string API_KEY = "AIzaSyCvmTLF-r9y3OSvF7VEKm_OhAXUb5UPxhQ";
        private const string API_URL = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
        private readonly LicenseDriverDbContext dbContext;

        public ChatBot()
        {
            InitializeComponent();
            dbContext = new LicenseDriverDbContext();
            ChatHistory.Items.Add("Gemini: Yo yo, chào bạn! Mình là trợ lý chill của ứng dụng Quản lý thi chứng chỉ lái xe an toàn. Hỏi mình gì cũng được nha, từ nghiêm túc đến chill chill luôn!");
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await SendMessage(UserInput.Text.Trim());
        }

        private async void UserInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await SendMessage(UserInput.Text.Trim());
            }
        }

        private async void SampleQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                string question = button.Content.ToString();
                await SendMessage(question);
            }
        }

        private async Task SendMessage(string userMessage)
        {
            if (string.IsNullOrEmpty(userMessage)) return;

            ChatHistory.Items.Add($"You: {userMessage}");
            UserInput.Text = "";

            string response = await ProcessMessage(userMessage);
            ChatHistory.Items.Add($"Gemini: {response}");
            ChatHistory.ScrollIntoView(ChatHistory.Items[ChatHistory.Items.Count - 1]);
        }

        private async Task<string> ProcessMessage(string message)
        {
            // Xử lý câu hỏi liên quan đến database
            if (message.Contains("Hôm nay có khóa học nào không"))
            {
                var today = DateOnly.FromDateTime(DateTime.Today); // Chuyển DateTime.Today thành DateOnly
                var activeCourses = GetAllCourses() // Lấy tất cả khóa học
                    .Where(c => c.Status == "Active" && c.StartDate <= today && c.EndDate >= today)
                    .ToList();

                return activeCourses.Any()
                    ? $"Hôm nay có {activeCourses.Count} khóa học đang diễn ra nha, chill lắm! Đây là danh sách: {string.Join(", ", activeCourses.Select(c => c.CourseName))}"
                    : "Hôm nay không có khóa nào active đâu, nghỉ xả hơi đi!";
            }
            else if (message.Contains("Đã có bao nhiêu người đăng ký"))
            {
                var courses = GetAllCourses()
                    .Where(c => c.Status == "Active")
                    .Select(c => new
                    {
                        c.CourseId,
                        c.CourseName,
                        c.StartDate,
                        c.EndDate,
                        c.Status,
                        NumberStudent = c.Registrations.Count(r => r.Status == "Approved" || r.Status == "Pending")
                    })
                    .ToList();

                var totalStudents = courses.Sum(c => c.NumberStudent);
                return $"Đã có {totalStudents} người đăng ký các khóa active rồi, đông vui phết! Có {courses.Count} khóa đang chạy nha.";
            }

            // Gửi các câu hỏi khác đến Gemini
            return await GetGeminiResponse(message);
        }

        private async Task<string> GetGeminiResponse(string message)
        {
            try
            {
                string prompt = $"Bạn là trợ lý AI siêu chill của ứng dụng 'Quản lý đăng ký và thi chứng chỉ kỹ năng lái xe an toàn'. Ứng dụng này giúp học sinh THPT, giảng viên, và cảnh sát giao thông quản lý khóa học, đăng ký, thi, và cấp chứng chỉ lái xe an toàn. Trả lời câu hỏi sau một cách ngắn gọn, thân thiện và hơi vui vui nha: {message}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new { parts = new[] { new { text = prompt } } }
                    }
                };

                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                string urlWithKey = $"{API_URL}?key={API_KEY}";
                HttpResponseMessage response = await client.PostAsync(urlWithKey, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(jsonResponse);
                    return data.candidates[0].content.parts[0].text.ToString();
                }
                else
                {
                    return $"Oops, có lỗi gì rồi: {response.StatusCode} - {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return $"Úi, lỗi tè le luôn: {ex.Message}";
            }
        }

        // Phương thức lấy tất cả khóa học
        public static List<Course> GetAllCourses()
        {
            var list = new List<Course>();
            try
            {
                using var db = new LicenseDriverDbContext();
                list = db.Courses.Include(c => c.Registrations).ToList(); // Include Registrations để đếm số học sinh
            }
            catch (Exception ex)
            {
                // Xử lý lỗi nếu cần, hiện tại để trống theo code của bạn
            }
            return list;
        }
    }
}