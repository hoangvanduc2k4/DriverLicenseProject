Ứng Dụng Quản Lý Đăng Ký Học & Thi Chứng Chỉ Lái Xe Máy
Ứng dụng này được thiết kế dành cho học sinh THPT nhằm tạo điều kiện thuận lợi cho quá trình đăng ký học, tham gia thi và nhận chứng chỉ lái xe máy. Với giao diện trực quan và quy trình tự động, hệ thống giúp các em trải nghiệm quá trình học tập và thi cử nhanh chóng, tiện lợi và hiệu quả.

Công Nghệ Sử Dụng

Nền tảng: WPF (Windows Presentation Foundation)
Cơ sở dữ liệu: SQL Server
ORM: Entity Framework Core
Kiến trúc: Three Layers


Vai Trò & Quyền Hạn Người Dùng
🟢 Common Screens

Login: Đăng nhập vào hệ thống.
Register: Đăng ký tài khoản mới.
Profile: Xem và chỉnh sửa thông tin cá nhân.
Change Password: Thay đổi mật khẩu tài khoản.
Main Menu: Giao diện chính của ứng dụng.

🔴 Admin

Quản lý tài khoản người dùng.
Xem số liệu thống kê để giám sát hiệu suất tổng thể của hệ thống.

🟠 Teacher

Tạo và quản lý khóa học.
Xác nhận đăng ký của học sinh.
Ghi nhận kết quả thi và phối hợp với cảnh sát giao thông để tổ chức và giám sát kỳ thi.

🔵 Student

Đăng ký khóa học.
Cập nhật thông tin cá nhân.
Tham gia kỳ thi và nhận chứng chỉ lái xe máy khi đạt yêu cầu.

🟡 Traffic Police

Giám sát quá trình thi.
Tạo lịch thi và phân công giám sát.
Kiểm tra tính tuân thủ của kỳ thi theo các quy định an toàn.
Phê duyệt cấp chứng chỉ cho học sinh đủ điều kiện.


Hướng Dẫn Cài Đặt & Sử Dụng
1. Clone Repository
Clone mã nguồn của dự án về máy của bạn:
git clone https://github.com/hoangvanduc2k4/DriverLicenseProject.git

2. Cài Đặt Các Gói Phụ Thuộc

Mở dự án trong Visual Studio.
Sử dụng NuGet Package Manager để cài đặt các package cần thiết hoặc chạy lệnh:

dotnet restore

3. Cấu Hình Cơ Sở Dữ Liệu

Mở file cấu hình appsettings.json (hoặc file tương đương) và cập nhật chuỗi kết nối đến SQL Server của bạn:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=<your_server>;Database=<your_database>;User Id=<username>;Password=<password>;"
  }
}


Mở Package Manager Console và chạy các lệnh sau để tạo migration và cập nhật cơ sở dữ liệu:

dotnet ef migrations add InitialCreate
dotnet ef database update

4. Chạy Ứng Dụng

Thiết lập project chính (Startup Project) trong Visual Studio.
Nhấn F5 hoặc chọn Debug > Start Debugging để khởi chạy ứng dụng.
Giao diện chính sẽ hiển thị, cho phép bạn đăng ký học, tham gia thi và theo dõi kết quả để nhận chứng chỉ lái xe máy.


Đóng Góp & Báo Cáo Lỗi
Chúng tôi luôn chào đón sự đóng góp từ cộng đồng! Nếu bạn có ý kiến cải thiện hoặc phát hiện lỗi:

Mở Pull Request với các thay đổi đề xuất.
Hoặc tạo một Issue mới trên GitHub để báo cáo lỗi và thảo luận về các cải tiến.


Cảm ơn bạn đã sử dụng và đóng góp cho dự án!
