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
-- Teachers
('Nguyen Van H', 'h@teacher.com', '123456', 2, NULL, NULL, '0911112222'),
('Tran Thi I', 'i@teacher.com', '123456', 2, NULL, NULL, '0922223333'),

-- Traffic Police
('Pham Minh J', 'j@police.com', '123456', 3, NULL, NULL, '0933334444'),

-- Admin
('Le Quoc K', 'k@admin.com', '123456', 4, NULL, NULL, '0944445555'),

-- Students
('Student 01', 'student01@student.com', '123456', 1, '12A1', 'Le Hong Phong High School', '0955556661'),
('Student 02', 'student02@student.com', '123456', 1, '11A2', 'Nguyen Du High School', '0955556662'),
('Student 03', 'student03@student.com', '123456', 1, '10B1', 'Tran Phu High School', '0955556663'),
('Student 04', 'student04@student.com', '123456', 1, '12A3', 'Le Hong Phong High School', '0955556664'),
('Student 05', 'student05@student.com', '123456', 1, '11C1', 'Nguyen Du High School', '0955556665'),
('Student 06', 'student06@student.com', '123456', 1, '10B3', 'Tran Phu High School', '0955556666'),
('Student 07', 'student07@student.com', '123456', 1, '12B2', 'Le Hong Phong High School', '0955556667'),
('Student 08', 'student08@student.com', '123456', 1, '11A3', 'Nguyen Du High School', '0955556668'),
('Student 09', 'student09@student.com', '123456', 1, '10C2', 'Tran Phu High School', '0955556669'),
('Student 10', 'student10@student.com', '123456', 1, '12C3', 'Le Hong Phong High School', '0955556670'),
('Student 11', 'student11@student.com', '123456', 1, '11B3', 'Nguyen Du High School', '0955556671'),
('Student 12', 'student12@student.com', '123456', 1, '10A1', 'Tran Phu High School', '0955556672'),
('Student 13', 'student13@student.com', '123456', 1, '12A2', 'Le Hong Phong High School', '0955556673'),
('Student 14', 'student14@student.com', '123456', 1, '11C2', 'Nguyen Du High School', '0955556674'),
('Student 15', 'student15@student.com', '123456', 1, '10B2', 'Tran Phu High School', '0955556675'),
('Student 16', 'student16@student.com', '123456', 1, '12B1', 'Le Hong Phong High School', '0955556676'),
('Student 17', 'student17@student.com', '123456', 1, '11A1', 'Nguyen Du High School', '0955556677'),
('Student 18', 'student18@student.com', '123456', 1, '10C3', 'Tran Phu High School', '0955556678'),
('Student 19', 'student19@student.com', '123456', 1, '12C1', 'Le Hong Phong High School', '0955556679'),
('Student 20', 'student20@student.com', '123456', 1, '11B1', 'Nguyen Du High School', '0955556680');

INSERT INTO Courses (CourseName, TeacherID, StartDate, EndDate, Status) VALUES
('Theory and Practice for A1 License', 1, '2025-03-10', '2025-04-10', 'Active'),
('Advanced Theory for A1 & A2 License', 2, '2025-03-15', '2025-04-20', 'Active'),
('A1 Driving Practice', 1, '2025-03-12', '2025-04-15', 'Active'),
('Comprehensive Theory for A1, A2, B1 Licenses', 2, '2025-03-20', '2025-05-01', 'Active'),
('Safe Driving Skills for A1 License', 1, '2025-03-25', '2025-04-30', 'Active');

INSERT INTO Registrations (UserID, CourseID, Status) VALUES
(5, 1, 'Approved'), (6, 1, 'Approved'), (7, 1, 'Approved'), (8, 1, 'Approved'), 
(9, 2, 'Approved'), (10, 2, 'Approved'), (11, 2, 'Approved'), (12, 2, 'Approved'),
(13, 3, 'Approved'), (14, 3, 'Approved'), (15, 3, 'Approved'), (16, 3, 'Approved'),
(17, 4, 'Approved'), (18, 4, 'Approved'), (19, 4, 'Approved'), (20, 4, 'Approved'),
(21, 5, 'Approved'), (22, 5, 'Approved'), (23, 5, 'Approved'), (24, 5, 'Approved');

INSERT INTO Exams (CourseID, ExamDate, ExamTime, DurationMinutes, Room, UserID) VALUES
(1, '2025-04-15', '08:00:00', 60, 'Room 101', 3), 
(2, '2025-04-25', '09:00:00', 60, 'Room 102', 3), 
(3, '2025-04-20', '10:00:00', 60, 'Room 103', 3), 
(4, '2025-05-05', '08:30:00', 90, 'Room 104', 3), 
(5, '2025-04-30', '09:30:00', 60, 'Room 105', 3);

INSERT INTO Results (ExamID, UserID, Score, Status, Notes) VALUES
(1, 5, 8.5, 'Pass', 'Good performance'), (1, 6, 7.0, 'Pass', 'Meets requirements'), 
(1, 7, 6.5, 'Pass', 'Needs improvement'), (1, 8, 5.0, 'Not Pass', 'Retake required'),
(2, 9, 9.0, 'Pass', 'Excellent'), (2, 10, 7.5, 'Pass', 'Meets requirements'), 
(2, 11, 6.0, 'Pass', 'Needs more effort'), (2, 12, 4.5, 'Not Pass', 'Retake required'),
(3, 13, 8.0, 'Pass', 'Good performance'), (3, 14, 7.2, 'Pass', 'Stable performance'), 
(3, 15, 6.8, 'Pass', 'Could improve'), (3, 16, 4.0, 'Not Pass', 'Retake required'),
(4, 17, 9.5, 'Pass', 'Excellent'), (4, 18, 7.8, 'Pass', 'Meets requirements'), 
(4, 19, 6.2, 'Pass', 'Needs improvement'), (4, 20, 3.5, 'Not Pass', 'Retake required'),
(5, 21, 8.7, 'Pass', 'Very good'), (5, 22, 7.3, 'Pass', 'Meets requirements'), 
(5, 23, 6.7, 'Pass', 'Acceptable'), (5, 24, 5.2, 'Not Pass', 'Retake required');


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

