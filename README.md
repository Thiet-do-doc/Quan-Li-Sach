# BookManagement — Quản lý sách MVC

Bài thực hành ASP.NET Core MVC + Entity Framework Core + SQL Server LocalDB, theo hướng **Database First**: tạo database bằng SQL trước, sau đó ánh xạ bảng vào ứng dụng. Có đủ thêm, xem danh sách/chi tiết, sửa và xóa sách; thêm tìm kiếm, lọc thể loại, phân trang và kiểm tra kết nối.

## 1. Tạo database trước

1. Mở SQL Server Management Studio (SSMS).
2. Server type: Database Engine. Server name: `(localdb)\MSSQLLocalDB`. Authentication: Windows Authentication.
3. Chọn File → Open → File, mở `Database/01_Create_BookManagementDB.sql`.
4. Execute hoặc F5. Kết quả phải có `DatabaseName = BookManagementDB` và 12 sách mẫu ở lần chạy đầu.
5. Refresh mục Databases → BookManagementDB → Tables → dbo.Books.

Script không xóa dữ liệu cũ; nếu database chưa có thì tạo database, nếu bảng chưa có thì tạo bảng và 12 sách mẫu. Nếu bảng đã có, script không sửa cấu trúc và không thêm lại dữ liệu mẫu. Vì thế, nếu từng tạo một bảng Books có cấu trúc khác, hãy so sánh schema hoặc dùng một database mới rồi đổi tên tương ứng trong script và chuỗi kết nối.

Dữ liệu mẫu dùng để demo, không phải danh mục xuất bản đã được xác minh. Các sách công nghệ/kỹ năng mang tác giả “Nhóm biên soạn” là ví dụ giả lập.

## 2. Chạy bằng Visual Studio (bản màu tím)

Cần **.NET 10 SDK**, phiên bản Visual Studio có hỗ trợ .NET 10 và workload **ASP.NET and web development**. Nếu IDE báo không hỗ trợ `net10.0`, cập nhật Visual Studio hoặc chạy bằng terminal với SDK 10. Kiểm tra SDK bằng `dotnet --list-sdks`.

1. Giải nén toàn bộ thư mục BookManagement vào ví dụ `E:\BookManagement`.
2. Mở `BookManagement.sln` (hoặc `BookManagement.csproj`).
3. Chờ NuGet restore. Lần đầu cần Internet để tải các package.
4. Chọn profile BookManagement → Ctrl+F5.
5. Mở `http://localhost:5188` nếu trình duyệt chưa tự mở.
6. Chọn **Kiểm tra CSDL**. Trang phải thông báo đã đọc được bảng Books.

Không cần tạo một project MVC trống rồi chép từng file vào. Đây là project có sẵn để mở và chạy.

## 3. Chạy bằng terminal / VS Code

Mở terminal tại thư mục chứa `BookManagement.csproj`:

```powershell
dotnet restore
dotnet build
dotnet run
```

Mở `http://localhost:5188`. Dừng bằng Ctrl+C. Nếu cổng 5188 bị chiếm, dùng:

```powershell
dotnet run --no-launch-profile --urls http://localhost:5190
```

Trong trường hợp này vào `http://localhost:5190`.

## 4. Kết nối đã cấu hình

`appsettings.json`:

```json
"BookConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BookManagementDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

Hai dấu `\\` là cách biểu diễn một dấu `\` trong JSON. Server thực tế vẫn là `(localdb)\MSSQLLocalDB`. Dùng tài khoản Windows hiện tại, không cần nhập tên đăng nhập SQL hoặc mật khẩu.

LocalDB nằm trên máy Windows của bạn và chạy theo tài khoản Windows; cấu hình này dành cho bài thực hành cục bộ. Chạy SSMS và project bằng cùng tài khoản. Nếu chuyển sang SQL Server khác, sửa BookConnection cho đúng server/database.

## 5. Các địa chỉ cần dùng

| URL | Chức năng |
| --- | --- |
| `/` hoặc `/Books` | Danh sách, tìm kiếm, lọc, phân trang |
| `/Books/Create` | Thêm sách |
| `/Books/Details/1` | Chi tiết sách có Id = 1 |
| `/Books/Edit/1` | Sửa sách có Id = 1 |
| `/Books/Delete/1` | Xác nhận xóa sách có Id = 1 |
| `/Home/Database` | Kiểm tra kết nối và đọc bảng Books |

Thay 1 bằng Id hiện có. Nút Xóa mở trang xác nhận; chỉ khi bấm **Xác nhận xóa** ứng dụng mới gửi POST để xóa.

## 6. Cấu trúc và trọng điểm code

| File/thư mục | Vai trò |
| --- | --- |
| `Database/01_Create_BookManagementDB.sql` | Nguồn schema: tạo database, bảng, ràng buộc và dữ liệu mẫu |
| `Models/Book.cs` | Entity biểu diễn một bản ghi Books |
| `Data/BookDbContext.cs` | Ánh xạ entity với bảng, theo dõi thay đổi và truy cập CSDL |
| `Models/BookFormViewModel.cs` | Dữ liệu form, quy tắc kiểm tra và sao chép các trường được phép sửa |
| `Controllers/BooksController.cs` | Luồng xử lý CRUD |
| `Views/Books/` | Razor Views hiển thị danh sách, form, chi tiết, xác nhận xóa |
| `Views/Shared/_Layout.cshtml` | Khung giao diện chung |
| `Program.cs` | Đăng ký MVC, DbContext và định tuyến mặc định |
| `appsettings.json` | Cấu hình kết nối SQL Server |
| `wwwroot/css/site.css` | CSS; không cần CDN để hiển thị giao diện |

Luồng thêm sách: mở Create bằng GET → nhập form → POST Create → model binding và validation → ModelState hợp lệ → Add + SaveChangesAsync → INSERT trong SQL Server → chuyển về Index.

Luồng sửa: GET tải sách theo Id → form sửa → POST tải lại entity theo Id → gán trường từ form → SaveChangesAsync → UPDATE. Luồng xóa: GET hiển thị xác nhận → POST Remove + SaveChangesAsync → DELETE.

`SaveChangesAsync()` là bước ghi thay đổi xuống CSDL; chỉ gọi Add/Remove hoặc gán thuộc tính chưa có nghĩa dữ liệu đã được ghi. Danh sách được đọc lại từ SQL Server mỗi lần yêu cầu trang, không lưu bằng một List trong bộ nhớ.

Form có ràng buộc HTML và validation phía server. ViewModel không chứa Id, nên form chỉ gửi các trường được phép chỉnh sửa; Id lấy từ route. Form Tag Helper sinh anti-forgery token và `[ValidateAntiForgeryToken]` kiểm tra token ở POST. Razor mã hóa nội dung sách khi hiển thị.

## 7. Database First và scaffold

Model và DbContext trong bộ này được **viết sẵn theo schema của script SQL**, chưa được tự động reverse-engineer từ LocalDB trên máy bạn. Ứng dụng dùng database có sẵn, không gọi `EnsureCreated`, `Migrate` và không có migrations tạo schema.

Nếu thầy yêu cầu minh chứng thao tác scaffold Database First, sau khi tạo DB bạn có thể sinh thêm một bản ánh xạ độc lập từ database thật. Trong terminal ở thư mục project:

```powershell
dotnet tool restore
dotnet ef dbcontext scaffold "Name=ConnectionStrings:BookConnection" Microsoft.EntityFrameworkCore.SqlServer --table dbo.Books --context ScaffoldedBookDbContext --output-dir ReverseEngineered --context-dir ReverseEngineered --namespace BookManagement.ReverseEngineered --context-namespace BookManagement.ReverseEngineered --no-onconfiguring
```

Lệnh sinh file vào `ReverseEngineered`, không ghi đè Model/DbContext đang dùng. Có thể mở chúng để so sánh ánh xạ bảng. Chỉ chạy một lần; nếu thư mục đã tồn tại, giữ bản đó hoặc chọn tên output/context khác. Project vẫn chạy với BookDbContext hiện tại. Tham khảo chính thức: https://learn.microsoft.com/en-us/ef/core/managing-schemas/scaffolding/

## 8. Kiểm tra CRUD và dữ liệu thật

1. Thêm sách tên **Demo MVC**, tác giả **Thạch Thiết**, thể loại **Công nghệ**, năm **2026**, giá **120000**, số lượng **5**.
2. Tìm “Demo MVC”, mở chi tiết.
3. Sửa giá thành **135000**, số lượng thành **9**.
4. Trong SSMS chạy:

```sql
USE BookManagementDB;
SELECT Id, Title, Price, Quantity FROM dbo.Books WHERE Title = N'Demo MVC';
```

5. Xác nhận giá và số lượng đúng, rồi xóa sách demo trên web.
6. Chạy lại SELECT: không còn bản ghi demo.
7. Thử giá âm, số lượng âm, tên trống: trình duyệt hoặc server phải từ chối, không ghi dữ liệu sai.

## 9. Xử lý lỗi thường gặp

| Dấu hiệu | Cách xử lý |
| --- | --- |
| Không thấy SDK 10 / NETSDK1045 | Cài .NET 10 SDK, cập nhật IDE hoặc dùng terminal |
| NuGet restore thất bại | Kiểm tra Internet, nguồn `https://api.nuget.org/v3/index.json`, chạy lại `dotnet restore` |
| Cannot open database BookManagementDB | Chạy script SQL đúng LocalDB, kiểm tra tên database |
| Invalid object name dbo.Books | Chưa tạo bảng hoặc đang kết nối nhầm database |
| LocalDB instance không tồn tại | Kiểm tra cài SQL Server Express LocalDB; chạy `sqllocaldb info`, `sqllocaldb start MSSQLLocalDB` |
| SSMS có dữ liệu nhưng app không thấy | So sánh server, tên DB, tài khoản Windows trong SSMS và app |
| Giá nhập không hợp lệ | Nhập số thuần như 120000 hoặc 120000.50; không gõ dấu phân cách hàng nghìn |
| HTTP 400 khi POST | Mở lại form từ ứng dụng để có anti-forgery token hợp lệ |

