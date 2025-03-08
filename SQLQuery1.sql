USE [master]
GO

/*******************************************************************************
   Drop database if it exists
********************************************************************************/
IF EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'LicenseDriverDB')
BEGIN
	ALTER DATABASE LicenseDriverDB SET OFFLINE WITH ROLLBACK IMMEDIATE;
	ALTER DATABASE LicenseDriverDB SET ONLINE;
	DROP DATABASE LicenseDriverDB;
END

GO

CREATE DATABASE LicenseDriverDB
GO

USE LicenseDriverDB
GO

/*******************************************************************************
	Drop tables if exists
*******************************************************************************/
-- Tạo bảng Users (Quản lý tài khoản)
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(255) NOT NULL,
    Role INT CHECK (Role IN (1, 2, 3, 4)) NOT NULL, -- 1: Student, 2: Teacher, 3: TrafficPolice, 4: Admin
    Class VARCHAR(50) NULL, -- chỉ dành cho học sinh) 
    School VARCHAR(100) NULL, -- chỉ dành cho học sinh)
    Phone NVARCHAR(15) NULL
);

-- Tạo bảng Courses (Khóa học)
CREATE TABLE Courses (
    CourseID INT IDENTITY(1,1) PRIMARY KEY,
    CourseName NVARCHAR(100) NOT NULL,
    TeacherID INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active', 'Closed', 'Cancelled')), -- Ràng buộc giá trị cho Status
    FOREIGN KEY (TeacherID) REFERENCES Users(UserID)
);


-- Tạo bảng Registrations (Đăng ký khóa học/kỳ thi)
CREATE TABLE Registrations (
    RegistrationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    CourseID INT NOT NULL,
    Status NVARCHAR(20) CHECK (Status IN ('Pending', 'Approved', 'Rejected')) DEFAULT 'Pending',
    Comments NVARCHAR(MAX) NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID),
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID)
);

-- Tạo bảng Exams (Thông tin kỳ thi)
CREATE TABLE Exams (
    ExamID INT IDENTITY(1,1) PRIMARY KEY,
    CourseID INT NOT NULL,
    ExamDate DATE NOT NULL,
    ExamTime TIME NOT NULL, -- Giờ bắt đầu thi
    DurationMinutes INT NOT NULL, -- Thời lượng thi (phút)
    Room NVARCHAR(50) NOT NULL,
    UserID INT NOT NULL, -- Người giám sát kỳ thi
    FOREIGN KEY (CourseID) REFERENCES Courses(CourseID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);



-- Tạo bảng Results (Kết quả thi)
CREATE TABLE Results (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    ExamID INT NOT NULL,
    UserID INT NOT NULL,
    Score DECIMAL(5,2) NOT NULL,
    Status NVARCHAR(10) NOT NULL CHECK (Status IN ('Pass', 'Not Pass')), 
    Notes NVARCHAR(MAX) NULL, -- Ghi chú về kết quả thi
    FOREIGN KEY (ExamID) REFERENCES Exams(ExamID),
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);




-- Tạo bảng Certificates (Chứng chỉ)
CREATE TABLE Certificates (
    CertificateID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    IssuedDate DATE NOT NULL,
    ExpirationDate DATE NOT NULL,
    CertificateCode NVARCHAR(50) UNIQUE NOT NULL,
Status Nvarchar(20) check (Status in ('Inactive', 'Active')) Default 'Active'
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- Tạo bảng Notifications (Thông báo)
CREATE TABLE Notifications (
    NotificationID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL,
    Message NVARCHAR(MAX) NOT NULL,
    SentDate DATETIME DEFAULT GETDATE(),
    IsRead BIT DEFAULT 0,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);


INSERT INTO Users (FullName, Email, Password, Role, Class, School, Phone) VALUES
-- Giáo viên (Teacher)
('Nguyễn Văn H', 'h@teacher.com', '123456', 2, NULL, NULL, '0911112222'),
('Trần Thị I', 'i@teacher.com', '123456', 2, NULL, NULL, '0922223333'),

-- Cảnh sát giao thông (Traffic Police)
('Phạm Minh J', 'j@police.com', '123456', 3, NULL, NULL, '0933334444'),

-- Quản trị viên (Admin)
('Lê Quốc K', 'k@admin.com', '123456', 4, NULL, NULL, '0944445555'),

-- Học sinh (Student)
('Học Sinh 01', 'student01@student.com', '123456', 1, '12A1', 'THPT Lê Hồng Phong', '0955556661'),
('Học Sinh 02', 'student02@student.com', '123456', 1, '11A2', 'THPT Nguyễn Du', '0955556662'),
('Học Sinh 03', 'student03@student.com', '123456', 1, '10B1', 'THPT Trần Phú', '0955556663'),
('Học Sinh 04', 'student04@student.com', '123456', 1, '12A3', 'THPT Lê Hồng Phong', '0955556664'),
('Học Sinh 05', 'student05@student.com', '123456', 1, '11C1', 'THPT Nguyễn Du', '0955556665'),
('Học Sinh 06', 'student06@student.com', '123456', 1, '10B3', 'THPT Trần Phú', '0955556666'),
('Học Sinh 07', 'student07@student.com', '123456', 1, '12B2', 'THPT Lê Hồng Phong', '0955556667'),
('Học Sinh 08', 'student08@student.com', '123456', 1, '11A3', 'THPT Nguyễn Du', '0955556668'),
('Học Sinh 09', 'student09@student.com', '123456', 1, '10C2', 'THPT Trần Phú', '0955556669'),
('Học Sinh 10', 'student10@student.com', '123456', 1, '12C3', 'THPT Lê Hồng Phong', '0955556670'),
('Học Sinh 11', 'student11@student.com', '123456', 1, '11B3', 'THPT Nguyễn Du', '0955556671'),
('Học Sinh 12', 'student12@student.com', '123456', 1, '10A1', 'THPT Trần Phú', '0955556672'),
('Học Sinh 13', 'student13@student.com', '123456', 1, '12A2', 'THPT Lê Hồng Phong', '0955556673'),
('Học Sinh 14', 'student14@student.com', '123456', 1, '11C2', 'THPT Nguyễn Du', '0955556674'),
('Học Sinh 15', 'student15@student.com', '123456', 1, '10B2', 'THPT Trần Phú', '0955556675'),
('Học Sinh 16', 'student16@student.com', '123456', 1, '12B1', 'THPT Lê Hồng Phong', '0955556676'),
('Học Sinh 17', 'student17@student.com', '123456', 1, '11A1', 'THPT Nguyễn Du', '0955556677'),
('Học Sinh 18', 'student18@student.com', '123456', 1, '10C3', 'THPT Trần Phú', '0955556678'),
('Học Sinh 19', 'student19@student.com', '123456', 1, '12C1', 'THPT Lê Hồng Phong', '0955556679'),
('Học Sinh 20', 'student20@student.com', '123456', 1, '11B1', 'THPT Nguyễn Du', '0955556680');


INSERT INTO Courses (CourseName, TeacherID, StartDate, EndDate, Status) VALUES
('Lý thuyết và thực hành bằng A1', 1, '2025-03-10', '2025-04-10', 'Active'),
('Lý thuyết nâng cao A1 & A2', 2, '2025-03-15', '2025-04-20', 'Active'),
('Thực hành lái xe A1', 1, '2025-03-12', '2025-04-15', 'Active'),
('Tổng hợp lý thuyết A1, A2, B1', 2, '2025-03-20', '2025-05-01', 'Active'),
('Kỹ năng lái xe an toàn A1', 1, '2025-03-25', '2025-04-30', 'Active');


INSERT INTO Registrations (UserID, CourseID, Status) VALUES
(5, 1, 'Approved'), (6, 1, 'Approved'), (7, 1, 'Approved'), (8, 1, 'Approved'), 
(9, 2, 'Approved'), (10, 2, 'Approved'), (11, 2, 'Approved'), (12, 2, 'Approved'),
(13, 3, 'Approved'), (14, 3, 'Approved'), (15, 3, 'Approved'), (16, 3, 'Approved'),
(17, 4, 'Approved'), (18, 4, 'Approved'), (19, 4, 'Approved'), (20, 4, 'Approved'),
(21, 5, 'Approved'), (22, 5, 'Approved'), (23, 5, 'Approved'), (24, 5, 'Approved');

INSERT INTO Exams (CourseID, ExamDate, ExamTime, DurationMinutes, Room, UserID) VALUES
(1, '2025-04-15', '08:00:00', 60, 'Phòng 101', 3), 
(2, '2025-04-25', '09:00:00', 60, 'Phòng 102', 3), 
(3, '2025-04-20', '10:00:00', 60, 'Phòng 103', 3), 
(4, '2025-05-05', '08:30:00', 90, 'Phòng 104', 3), 
(5, '2025-04-30', '09:30:00', 60, 'Phòng 105', 3);

INSERT INTO Exams (CourseID, ExamDate, ExamTime, DurationMinutes, Room, UserID) VALUES
-- Giáo viên T1 giám sát lớp C2, C3 (không phải lớp của họ)
(1, '2025-04-15', '08:00:00', 60, 'Phòng 101', 2), 
(2, '2025-04-25', '09:00:00', 60, 'Phòng 102', 3), -- Police giám sát lớp này
-- Giáo viên T2 giám sát lớp C1, C4 (không phải lớp của họ)
(3, '2025-04-20', '10:00:00', 60, 'Phòng 103', 1), 
(4, '2025-05-05', '08:30:00', 90, 'Phòng 104', 3), -- Police giám sát lớp này
-- Police giám sát lớp C5
(5, '2025-04-30', '09:30:00', 60, 'Phòng 105', 3);



INSERT INTO Results (ExamID, UserID, Score, Status, Notes) VALUES
-- Kết quả cho kỳ thi của khóa 1
(1, 5, 8.5, 'Pass', 'Làm bài tốt'), (1, 6, 7.0, 'Pass', 'Đạt yêu cầu'), 
(1, 7, 6.5, 'Pass', 'Cần cải thiện'), (1, 8, 5.0, 'Not Pass', 'Thi lại lần sau'),
-- Kết quả cho kỳ thi của khóa 2
(2, 9, 9.0, 'Pass', 'Xuất sắc'), (2, 10, 7.5, 'Pass', 'Đạt yêu cầu'), 
(2, 11, 6.0, 'Pass', 'Cố gắng hơn'), (2, 12, 4.5, 'Not Pass', 'Thi lại lần sau'),
-- Kết quả cho kỳ thi của khóa 3
(3, 13, 8.0, 'Pass', 'Làm bài tốt'), (3, 14, 7.2, 'Pass', 'Ổn định'), 
(3, 15, 6.8, 'Pass', 'Cải thiện thêm'), (3, 16, 4.0, 'Not Pass', 'Thi lại lần sau'),
-- Kết quả cho kỳ thi của khóa 4
(4, 17, 9.5, 'Pass', 'Xuất sắc'), (4, 18, 7.8, 'Pass', 'Đạt yêu cầu'), 
(4, 19, 6.2, 'Pass', 'Cần cải thiện'), (4, 20, 3.5, 'Not Pass', 'Thi lại lần sau'),
-- Kết quả cho kỳ thi của khóa 5
(5, 21, 8.7, 'Pass', 'Rất tốt'), (5, 22, 7.3, 'Pass', 'Đạt yêu cầu'), 
(5, 23, 6.7, 'Pass', 'Ổn'), (5, 24, 5.2, 'Not Pass', 'Thi lại lần sau');


INSERT INTO Certificates (UserID, IssuedDate, ExpirationDate, CertificateCode, Status)
VALUES
(5, '2025-05-10', '2035-05-10', 'CERT-5', 'Inactive'),
(6, '2025-05-10', '2035-05-10', 'CERT-6', 'Inactive'),
(7, '2025-05-10', '2035-05-10', 'CERT-7', 'Inactive'),
(9, '2025-05-10', '2035-05-10', 'CERT-9', 'Inactive'),
(10, '2025-05-10', '2035-05-10', 'CERT-10', 'Inactive'),
(11, '2025-05-10', '2035-05-10', 'CERT-11', 'Inactive'),
(13, '2025-05-10', '2035-05-10', 'CERT-13', 'Inactive'),
(14, '2025-05-10', '2035-05-10', 'CERT-14','Inactive'),
(15, '2025-05-10', '2035-05-10', 'CERT-15', 'Inactive'),
(17, '2025-05-10', '2035-05-10', 'CERT-17', 'Inactive'),
(18, '2025-05-10', '2035-05-10', 'CERT-18', 'Active'),
(19, '2025-05-10', '2035-05-10', 'CERT-19', 'Active'),
(21, '2025-05-10', '2035-05-10', 'CERT-21', 'Active'),
(22, '2025-05-10', '2035-05-10', 'CERT-22', 'Active'),
(23, '2025-05-10', '2035-05-10', 'CERT-23', 'Active');

INSERT INTO Notifications (UserID, Message, SentDate, IsRead)
VALUES
-- Thông báo được duyệt đăng ký khóa học
(5, 'Your course registration has been approved!', GETDATE(), 0),
(6, 'Your course registration has been approved!', GETDATE(), 0),
(7, 'Your course registration has been approved!', GETDATE(), 0),
(8, 'Your course registration has been approved!', GETDATE(), 0),
(9, 'Your course registration has been approved!', GETDATE(), 0),
(10, 'Your course registration has been approved!', GETDATE(), 0),
(11, 'Your course registration has been approved!', GETDATE(), 0),
(12, 'Your course registration has been approved!', GETDATE(), 0),
(13, 'Your course registration has been approved!', GETDATE(), 0),
(14, 'Your course registration has been approved!', GETDATE(), 0),
(15, 'Your course registration has been approved!', GETDATE(), 0),
(16, 'Your course registration has been approved!', GETDATE(), 0),
(17, 'Your course registration has been approved!', GETDATE(), 0),
(18, 'Your course registration has been approved!', GETDATE(), 0),
(19, 'Your course registration has been approved!', GETDATE(), 0),
(20, 'Your course registration has been approved!', GETDATE(), 0),
(21, 'Your course registration has been approved!', GETDATE(), 0),
(22, 'Your course registration has been approved!', GETDATE(), 0),
(23, 'Your course registration has been approved!', GETDATE(), 0),
(24, 'Your course registration has been approved!', GETDATE(), 0),

-- Thông báo đã có điểm thi
(5, 'Your exam results have been published!', GETDATE(), 0),
(6, 'Your exam results have been published!', GETDATE(), 0),
(7, 'Your exam results have been published!', GETDATE(), 0),
(8, 'Your exam results have been published!', GETDATE(), 0),
(9, 'Your exam results have been published!', GETDATE(), 0),
(10, 'Your exam results have been published!', GETDATE(), 0),
(11, 'Your exam results have been published!', GETDATE(), 0),
(12, 'Your exam results have been published!', GETDATE(), 0),
(13, 'Your exam results have been published!', GETDATE(), 0),
(14, 'Your exam results have been published!', GETDATE(), 0),
(15, 'Your exam results have been published!', GETDATE(), 0),
(16, 'Your exam results have been published!', GETDATE(), 0),
(17, 'Your exam results have been published!', GETDATE(), 0),
(18, 'Your exam results have been published!', GETDATE(), 0),
(19, 'Your exam results have been published!', GETDATE(), 0),
(20, 'Your exam results have been published!', GETDATE(), 0),
(21, 'Your exam results have been published!', GETDATE(), 0),
(22, 'Your exam results have been published!', GETDATE(), 0),
(23, 'Your exam results have been published!', GETDATE(), 0),
(24, 'Your exam results have been published!', GETDATE(), 0),

-- Thông báo cấp chứng chỉ
(5, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(6, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(7, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(9, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(10, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(11, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(13, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(14, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(15, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(17, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(18, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(19, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(21, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(22, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0),
(23, 'Congratulations! Your driving certificate has been issued.', GETDATE(), 0);

