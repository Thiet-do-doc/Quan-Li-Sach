-- BOOK MANAGEMENT - DATABASE FIRST
-- Trong SSMS: Server = (localdb)\MSSQLLocalDB; Windows Authentication.
-- Mo file nay -> Execute (F5). Script khong xoa database/bang hien co.
USE [master];
GO
IF DB_ID(N'BookManagementDB') IS NULL
    EXEC(N'CREATE DATABASE [BookManagementDB]');
GO
USE [BookManagementDB];
GO
SET XACT_ABORT ON;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID(N'dbo.Books', N'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Books
        (
            Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Books PRIMARY KEY,
            Title NVARCHAR(200) NOT NULL,
            Author NVARCHAR(120) NOT NULL,
            Genre NVARCHAR(80) NOT NULL,
            Publisher NVARCHAR(150) NULL,
            PublishedYear INT NOT NULL,
            Price DECIMAL(18,2) NOT NULL,
            Quantity INT NOT NULL,
            Description NVARCHAR(2000) NULL,
            CONSTRAINT CK_Books_Title CHECK (LEN(LTRIM(RTRIM(Title))) > 0),
            CONSTRAINT CK_Books_Author CHECK (LEN(LTRIM(RTRIM(Author))) > 0),
            CONSTRAINT CK_Books_Genre CHECK (LEN(LTRIM(RTRIM(Genre))) > 0),
            CONSTRAINT CK_Books_Year CHECK (PublishedYear BETWEEN 1000 AND 2100),
            CONSTRAINT CK_Books_Price CHECK (Price BETWEEN 0 AND 999999999),
            CONSTRAINT CK_Books_Quantity CHECK (Quantity BETWEEN 0 AND 1000000)
        );
        -- Du lieu minh hoa; chi them khi tao bang lan dau.
        INSERT INTO dbo.Books
            (Title, Author, Genre, Publisher, PublishedYear, Price, Quantity, Description)
        VALUES
            (N'Dế Mèn phiêu lưu ký', N'Tô Hoài', N'Văn học', N'Kim Đồng', 2020, 75000, 15, N'Những cuộc phiêu lưu của Dế Mèn và bài học về tình bạn.'),
            (N'Tôi thấy hoa vàng trên cỏ xanh', N'Nguyễn Nhật Ánh', N'Văn học', N'Trẻ', 2015, 125000, 12, N'Câu chuyện về tuổi thơ ở một làng quê Việt Nam.'),
            (N'Cho tôi xin một vé đi tuổi thơ', N'Nguyễn Nhật Ánh', N'Văn học', N'Trẻ', 2018, 90000, 20, N'Hành trình trở về với những ký ức tuổi thơ.'),
            (N'Nhà giả kim', N'Paulo Coelho', N'Tiểu thuyết', N'Văn học', 2020, 99000, 18, N'Chàng trai Santiago đi tìm kho báu và khám phá chính mình.'),
            (N'Lập trình C# căn bản', N'Nhóm biên soạn CNTT', N'Công nghệ', N'Tài liệu học tập', 2025, 150000, 10, N'Dữ liệu mẫu cho bài thực hành: cú pháp C#, OOP và ứng dụng.'),
            (N'ASP.NET Core MVC thực hành', N'Nhóm biên soạn CNTT', N'Công nghệ', N'Tài liệu học tập', 2026, 180000, 8, N'Dữ liệu mẫu: MVC, Entity Framework Core và CRUD.'),
            (N'Cơ sở dữ liệu SQL Server', N'Nhóm biên soạn CNTT', N'Công nghệ', N'Tài liệu học tập', 2025, 160000, 14, N'Dữ liệu mẫu: thiết kế bảng, truy vấn và kết nối ứng dụng.'),
            (N'Kỹ năng quản lý thời gian', N'Nhóm biên soạn', N'Kỹ năng', N'Tài liệu học tập', 2024, 85000, 16, N'Dữ liệu mẫu: lập kế hoạch học tập và công việc.'),
            (N'Nhập môn Internet of Things', N'Nhóm biên soạn CNTT', N'Công nghệ', N'Tài liệu học tập', 2026, 175000, 6, N'Dữ liệu mẫu: cảm biến, vi điều khiển và kết nối IoT.'),
            (N'Tư duy giải thuật', N'Nhóm biên soạn CNTT', N'Công nghệ', N'Tài liệu học tập', 2025, 135000, 9, N'Dữ liệu mẫu: cấu trúc dữ liệu, tìm kiếm và sắp xếp.'),
            (N'Giao tiếp hiệu quả', N'Nhóm biên soạn', N'Kỹ năng', N'Tài liệu học tập', 2024, 95000, 11, N'Dữ liệu mẫu: lắng nghe và làm việc nhóm.'),
            (N'Khám phá khoa học', N'Nhóm biên soạn', N'Khoa học', N'Tài liệu học tập', 2025, 110000, 7, N'Dữ liệu mẫu: các kiến thức khoa học cơ bản.');
    END;
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
SELECT DB_NAME() AS DatabaseName, COUNT(*) AS TotalBooks FROM dbo.Books;
SELECT * FROM dbo.Books ORDER BY Id;
GO
