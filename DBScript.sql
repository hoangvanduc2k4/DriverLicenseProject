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
-- Teachers (5 người, Role = 2)
('Nguyen Van H', 'teacher@gmail.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 2, NULL, NULL, '0911112222'),
('Tran Thi A', 'tranthia@teacher.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 2, NULL, NULL, '0912223333'),
('Le Van B', 'levanb@teacher.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 2, NULL, NULL, '0913334444'),
('Pham Thi C', 'phamthic@teacher.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 2, NULL, NULL, '0914445555'),
('Hoang Van D', 'hoangvand@teacher.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 2, NULL, NULL, '0915556666'),

-- Traffic Police (1 người, Role = 3)
('Pham Minh J', 'police@gmail.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 3, NULL, NULL, '0933334444'),

-- Admin (1 người, Role = 4)
('Le Quoc K', 'admin@gmail.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 4, NULL, NULL, '0944445555'),

-- Students (Nhiều nhất có thể, Role = 1, bắt đầu từ student@gmail.com)
('Student 01', 'student@gmail.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12A1', 'Le Hong Phong High School', '0955556661'),
('Student 02', 'student02@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11A2', 'Nguyen Du High School', '0955556662'),
('Student 03', 'student03@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10B1', 'Tran Phu High School', '0955556663'),
('Student 04', 'student04@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12A3', 'Le Hong Phong High School', '0955556664'),
('Student 05', 'student05@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11C1', 'Nguyen Du High School', '0955556665'),
('Student 06', 'student06@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10B3', 'Tran Phu High School', '0955556666'),
('Student 07', 'student07@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12B2', 'Le Hong Phong High School', '0955556667'),
('Student 08', 'student08@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11A3', 'Nguyen Du High School', '0955556668'),
('Student 09', 'student09@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10C2', 'Tran Phu High School', '0955556669'),
('Student 10', 'student10@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12C3', 'Le Hong Phong High School', '0955556670'),
('Student 11', 'student11@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11B3', 'Nguyen Du High School', '0955556671'),
('Student 12', 'student12@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10A1', 'Tran Phu High School', '0955556672'),
('Student 13', 'student13@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12A2', 'Le Hong Phong High School', '0955556673'),
('Student 14', 'student14@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11C2', 'Nguyen Du High School', '0955556674'),
('Student 15', 'student15@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10B2', 'Tran Phu High School', '0955556675'),
('Student 16', 'student16@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12B1', 'Le Hong Phong High School', '0955556676'),
('Student 17', 'student17@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11A1', 'Nguyen Du High School', '0955556677'),
('Student 18', 'student18@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10C3', 'Tran Phu High School', '0955556678'),
('Student 19', 'student19@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12C1', 'Le Hong Phong High School', '0955556679'),
('Student 20', 'student20@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11B1', 'Nguyen Du High School', '0955556680'),
-- Thêm 30 học sinh nữa (tổng cộng 50 học sinh)
('Student 21', 'student21@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12A4', 'Le Hong Phong High School', '0955556681'),
('Student 22', 'student22@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11B2', 'Nguyen Du High School', '0955556682'),
('Student 23', 'student23@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10C4', 'Tran Phu High School', '0955556683'),
('Student 24', 'student24@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12B3', 'Le Hong Phong High School', '0955556684'),
('Student 25', 'student25@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11A4', 'Nguyen Du High School', '0955556685'),
('Student 26', 'student26@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10B4', 'Tran Phu High School', '0955556686'),
('Student 27', 'student27@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12C4', 'Le Hong Phong High School', '0955556687'),
('Student 28', 'student28@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11C3', 'Nguyen Du High School', '0955556688'),
('Student 29', 'student29@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10A2', 'Tran Phu High School', '0955556689'),
('Student 30', 'student30@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12A5', 'Le Hong Phong High School', '0955556690'),
('Student 31', 'student31@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11B5', 'Nguyen Du High School', '0955556691'),
('Student 32', 'student32@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10C5', 'Tran Phu High School', '0955556692'),
('Student 33', 'student33@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12B5', 'Le Hong Phong High School', '0955556693'),
('Student 34', 'student34@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11A5', 'Nguyen Du High School', '0955556694'),
('Student 35', 'student35@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10B5', 'Tran Phu High School', '0955556695'),
('Student 36', 'student36@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12C5', 'Le Hong Phong High School', '0955556696'),
('Student 37', 'student37@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11C4', 'Nguyen Du High School', '0955556697'),
('Student 38', 'student38@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10A3', 'Tran Phu High School', '0955556698'),
('Student 39', 'student39@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12A6', 'Le Hong Phong High School', '0955556699'),
('Student 40', 'student40@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11B6', 'Nguyen Du High School', '0955556700'),
('Student 41', 'student41@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10C6', 'Tran Phu High School', '0955556701'),
('Student 42', 'student42@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12B6', 'Le Hong Phong High School', '0955556702'),
('Student 43', 'student43@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11A6', 'Nguyen Du High School', '0955556703'),
('Student 44', 'student44@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10B6', 'Tran Phu High School', '0955556704'),
('Student 45', 'student45@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12C6', 'Le Hong Phong High School', '0955556705'),
('Student 46', 'student46@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11C5', 'Nguyen Du High School', '0955556706'),
('Student 47', 'student47@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10A4', 'Tran Phu High School', '0955556707'),
('Student 48', 'student48@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '12A7', 'Le Hong Phong High School', '0955556708'),
('Student 49', 'student49@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '11B7', 'Nguyen Du High School', '0955556709'),
('Student 50', 'student50@student.com', 'pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=', 1, '10C7', 'Tran Phu High School', '0955556710');

INSERT INTO Courses (CourseName, TeacherID, StartDate, EndDate, Status) VALUES
-- Courses của teacher@gmail.com (UserID = 1)
('Theory and Practice', 1, '2025-03-10', '2025-04-10', 'Active'),
('Advanced Theory', 1, '2025-04-15', '2025-05-15', 'Active'),
('A1 Driving Practice', 1, '2025-05-20', '2025-06-20', 'Active'),
('Safe Driving Skills', 1, '2025-06-25', '2025-07-25', 'Closed'),
-- Courses của các giáo viên khác
('Beginner Motorcycle Driving', 2, '2025-06-01', '2025-06-30', 'Active'),
('Motorcycle Safety', 3, '2025-07-01', '2025-07-31', 'Cancelled'),
('Defensive Driving', 4, '2025-08-01', '2025-08-31', 'Active'),
('Road Skills', 5, '2025-09-01', '2025-09-30', 'Active');

INSERT INTO Registrations (UserID, CourseID, Status, Comments) VALUES
-- Đăng ký cho CourseID = 1 (Theory and Practice, TeacherID = 1)
(8, 1, 'Approved', 'Ready to join'),
(9, 1, 'Approved', 'Confirmed'),
(10, 1, 'Approved', 'Good to go'),
(11, 1, 'Approved', 'Excited to start'),
(12, 1, 'Approved', 'No issues'),
(13, 1, 'Approved', 'Prepared'),
(14, 1, 'Approved', NULL),
(15, 1, 'Approved', 'Looking forward'),
(16, 1, 'Approved', 'All set'),
(17, 1, 'Approved', 'Confirmed attendance'),
(18, 1, 'Pending', 'Waiting for approval'),
(19, 1, 'Pending', 'Need more info'),
(20, 1, 'Rejected', 'Missing documents'),
(21, 1, 'Rejected', 'Not eligible'),
-- Đăng ký cho CourseID = 5 (Beginner Motorcycle Driving, TeacherID = 2, nhưng UserID = 1 sẽ duyệt điểm sau)
(27, 5, 'Approved', 'Ready'),
(28, 5, 'Approved', 'Confirmed'),
(29, 5, 'Approved', 'No problems'),
(30, 5, 'Approved', 'Enrolled'),
(31, 5, 'Approved', 'All good'),
(32, 5, 'Approved', 'Joined'),
(33, 5, 'Approved', 'Confirmed'),
(34, 5, 'Approved', 'Good to go'),
(35, 5, 'Approved', 'Prepared'),
(36, 5, 'Pending', 'Processing'),
(37, 5, 'Pending', 'Under review'),
(38, 5, 'Rejected', 'Not qualified'),
(39, 5, 'Rejected', 'Late registration'),
-- Đăng ký linh tinh cho các khóa học khác
(40, 2, 'Approved', 'Joined'),
(41, 3, 'Pending', 'Waiting'),
(42, 4, 'Rejected', 'Not eligible');

INSERT INTO Exams (CourseID, ExamDate, ExamTime, DurationMinutes, Room, UserID) VALUES
-- Exam cho CourseID = 1 (TeacherID = 1, UserID không được là 1)
(1, '2025-04-15', '08:00:00', 60, 'Room 101', 2), -- Tran Thi A giám sát
(1, '2025-04-16', '09:00:00', 60, 'Room 102', 3), -- Le Van B giám sát
-- Exam cho CourseID = 2 (TeacherID = 1, UserID không được là 1)
(2, '2025-05-20', '10:00:00', 60, 'Room 103', 4), -- Pham Thi C giám sát
-- Exam cho CourseID = 5 (TeacherID = 2, UserID = 1 giám sát)
(5, '2025-07-05', '08:00:00', 60, 'Room 106', 1), -- Nguyen Van H (teacher@gmail.com) giám sát
-- Exam cho CourseID = 6 (TeacherID = 3, UserID = 1 giám sát)
(6, '2025-08-05', '09:00:00', 60, 'Room 107', 1), -- Nguyen Van H (teacher@gmail.com) giám sát
-- Exam cho CourseID = 7 (TeacherID = 4, UserID = 1 giám sát)
(7, '2025-09-05', '10:00:00', 60, 'Room 108', 1), -- Nguyen Van H (teacher@gmail.com) giám sát
-- Exam cho CourseID = 3 (TeacherID = 1, UserID không được là 1)
(3, '2025-06-25', '08:30:00', 90, 'Room 104', 5); -- Hoang Van D giám sát




INSERT INTO Results (ExamID, UserID, Score, Status, Notes) VALUES
-- Results cho ExamID = 1 (CourseID = 1)
(1, 8, 120.0, 'Pass', 'Excellent performance'),
(1, 9, 110.0, 'Pass', 'Good effort'),
(1, 10, 105.0, 'Not Pass', 'Needs improvement'),
(1, 11, 108.0, 'Pass', 'Meets requirements'),
(1, 12, 95.0, 'Not Pass', 'Retake required'),
(1, 13, 115.0, 'Pass', 'Very good'),
(1, 14, 100.0, 'Not Pass', 'Close but not enough'),
(1, 15, 125.0, 'Pass', 'Outstanding'),
(1, 16, 85.0, 'Not Pass', 'Poor result'),
(1, 17, 112.0, 'Pass', 'Well done'),
-- Results cho ExamID = 2 (CourseID = 1)
(2, 8, 130.0, 'Pass', 'Perfect score'),
(2, 9, 107.0, 'Pass', 'Just passed'),
(2, 10, 106.0, 'Not Pass', 'Missed by 1 point'),
(2, 11, 90.0, 'Not Pass', 'Needs more effort'),
(2, 12, 118.0, 'Pass', 'Improved'),
(2, 13, 109.0, 'Pass', 'Stable performance'),
(2, 14, 102.0, 'Not Pass', 'Not enough'),
(2, 15, 123.0, 'Pass', 'Great job'),
(2, 16, 111.0, 'Pass', 'Satisfactory'),
(2, 17, 80.0, 'Not Pass', 'Poor performance'),
-- Results cho ExamID = 3 (CourseID = 2)
(3, 40, 115.0, 'Pass', 'Good result'),
-- Results cho ExamID = 4 (CourseID = 5)
(4, 27, 115.0, 'Pass', 'Good result'),
(4, 28, 100.0, 'Not Pass', 'Failed'),
(4, 29, 108.0, 'Pass', 'Meets standard'),
(4, 30, 120.0, 'Pass', 'Excellent'),
(4, 31, 95.0, 'Not Pass', 'Retake required'),
(4, 32, 112.0, 'Pass', 'Well done'),
(4, 33, 107.0, 'Pass', 'Just passed'),
(4, 34, 106.0, 'Not Pass', 'Missed by 1 point'),
(4, 35, 125.0, 'Pass', 'Outstanding'),
-- Results cho ExamID = 5 (CourseID = 6), không có học sinh Approved trong Registrations hiện tại
-- Thêm vài học sinh Approved vào CourseID = 6 trước
(5, 41, 110.0, 'Pass', 'Good effort'), -- Giả sử UserID = 41 được Approved sau này
(5, 42, 100.0, 'Not Pass', 'Needs improvement'), -- Giả sử UserID = 42 được Approved sau này
-- Results cho ExamID = 6 (CourseID = 7), không có học sinh Approved trong Registrations hiện tại
(6, 43, 118.0, 'Pass', 'Great job'), -- Giả sử UserID = 43 được Approved sau này
(6, 44, 90.0, 'Not Pass', 'Failed'), -- Giả sử UserID = 44 được Approved sau này
-- Results cho ExamID = 7 (CourseID = 3)
(7, 41, 108.0, 'Pass', 'Meets standard'); -- Giả sử UserID = 41 đăng ký CourseID = 3


INSERT INTO Certificates (UserID, IssuedDate, ExpirationDate, CertificateCode, Status) VALUES
-- Certificates cho ExamID = 1 (CourseID = 1)
(8, '2025-04-20', '2035-04-20', 'CERT008', 'Inactive'),
(9, '2025-04-20', '2035-04-20', 'CERT009', 'Inactive'),
(11, '2025-04-20', '2035-04-20', 'CERT011', 'Inactive'),
(13, '2025-04-20', '2035-04-20', 'CERT013', 'Inactive'),
(15, '2025-04-20', '2035-04-20', 'CERT015', 'Inactive'),
(17, '2025-04-20', '2035-04-20', 'CERT017', 'Inactive'),
-- Certificates cho ExamID = 2 (CourseID = 1)
(8, '2025-04-25', '2035-04-25', 'CERT008-2', 'Active'),
(9, '2025-04-25', '2035-04-25', 'CERT009-2', 'Active'),
(12, '2025-04-25', '2035-04-25', 'CERT012', 'Active'),
(13, '2025-04-25', '2035-04-25', 'CERT013-2', 'Active'),
(15, '2025-04-25', '2035-04-25', 'CERT015-2', 'Active'),
(16, '2025-04-25', '2035-04-25', 'CERT016', 'Active'),
-- Certificates cho ExamID = 3 (CourseID = 2)
(40, '2025-05-25', '2035-05-25', 'CERT040', 'Active'),
-- Certificates cho ExamID = 4 (CourseID = 5)
(27, '2025-07-10', '2035-07-10', 'CERT027', 'Active'),
(29, '2025-07-10', '2035-07-10', 'CERT029', 'Inactive'),
(30, '2025-07-10', '2035-07-10', 'CERT030', 'Inactive'),
(32, '2025-07-10', '2035-07-10', 'CERT032', 'Active'),
(33, '2025-07-10', '2035-07-10', 'CERT033', 'Active'),
(35, '2025-07-10', '2035-07-10', 'CERT035', 'Active'),
-- Certificates cho ExamID = 5 (CourseID = 6)
(41, '2025-08-10', '2035-08-10', 'CERT041', 'Active'),
-- Certificates cho ExamID = 6 (CourseID = 7)
(43, '2025-09-10', '2035-09-10', 'CERT043', 'Active'),
-- Certificates cho ExamID = 7 (CourseID = 3)
(41, '2025-06-30', '2035-06-30', 'CERT041-2', 'Active');

INSERT INTO Notifications (UserID, Message, SentDate, IsRead) VALUES
-- Thông báo cho UserID = 5 (student05@student.com)
(5, 'Your registration for a course is pending approval.', '2025-03-24', 0),
(5, 'Reminder: Please complete your profile information.', '2025-03-24', 1),
(5, 'Your exam schedule has been updated.', '2025-03-24', 0),
(5, 'Congratulations! You have a new certificate available.', '2025-03-24', 0),
-- Thông báo cho CourseID = 1
(8, 'Your registration for Theory and Practice has been approved!', '2025-03-24', 1),
(9, 'Your exam results for Theory and Practice have been published!', '2025-04-15', 0),
(11, 'Congratulations! Your driving certificate has been issued.', '2025-04-20', 1),
(15, 'Your exam results for Theory and Practice have been published!', '2025-04-16', 0),
(17, 'Congratulations! Your driving certificate has been issued.', '2025-04-20', 1),
(18, 'Your registration for Theory and Practice is pending.', '2025-03-24', 0),
-- Thông báo cho CourseID = 5
(27, 'Your registration for Beginner Motorcycle Driving has been approved!', '2025-06-01', 1),
(29, 'Your exam results for Beginner Motorcycle Driving have been published!', '2025-07-05', 0),
(30, 'Congratulations! Your driving certificate has been issued.', '2025-07-10', 1),
(35, 'Your exam results for Beginner Motorcycle Driving have been published!', '2025-07-05', 0),
(36, 'Your registration for Beginner Motorcycle Driving is pending.', '2025-06-01', 0),
-- Thông báo cho CourseID = 2
(40, 'Your registration for Advanced Theory has been approved!', '2025-04-15', 1),
(40, 'Congratulations! Your driving certificate has been issued.', '2025-05-25', 0),
-- Thông báo cho CourseID = 6
(41, 'Your exam results for Motorcycle Safety have been published!', '2025-08-05', 0),
(41, 'Congratulations! Your driving certificate has been issued.', '2025-08-10', 1),
-- Thông báo cho CourseID = 7
(43, 'Your exam results for Defensive Driving have been published!', '2025-09-05', 0),
(43, 'Congratulations! Your driving certificate has been issued.', '2025-09-10', 1),
-- Thông báo cho CourseID = 3
(41, 'Your exam results for A1 Driving Practice have been published!', '2025-06-25', 0);