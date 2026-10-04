# Kịch bản giải thích BookManagement — khoảng 6 phút

Mục tiêu: chứng minh đủ CRUD, giải thích rõ MVC và cho thấy thao tác trên web thực sự ghi vào SQL Server. Mở sẵn project, trình duyệt và SSMS. Tập chạy trước một lần để video liền mạch.

## Thứ tự quay

| Thời gian dự kiến | Màn hình / file mở | Nội dung trọng điểm |
| --- | --- | --- |
| 0:00–0:25 | Danh sách sách | Giới thiệu bài, công nghệ và đủ CRUD |
| 0:25–1:05 | SQL script → appsettings.json → Program.cs | Database First, kết nối LocalDB và đăng ký DbContext |
| 1:05–1:40 | Book.cs → BookDbContext.cs → sơ đồ luồng ở dưới | Vai trò Model, Controller, View, Entity Framework |
| 1:40–2:45 | Form thêm → POST Create → kiểm tra dữ liệu | GET/POST, validation, Add, SaveChangesAsync |
| 2:45–3:30 | Danh sách → Index → tìm kiếm / chi tiết | READ, truy vấn SQL, trả dữ liệu cho View |
| 3:30–4:25 | Form sửa → POST Edit → SSMS | Tải sách theo Id, tracking, UPDATE, kiểm tra DB thật |
| 4:25–5:15 | Trang xác nhận → POST Delete → SSMS | DELETE chỉ qua POST, token, xác nhận đã xóa |
| 5:15–6:00 | _BookForm.cshtml → tổng kết luồng | View dùng Razor, đồng bộ dữ liệu qua EF, điểm kỹ thuật chính |

## Lời nói gợi ý và thao tác khớp với code

### 1. Giới thiệu

**Thao tác:** mở trang `/Books`, chỉ các nút Thêm, Xem, Sửa, Xóa.

“Em xây dựng ứng dụng quản lý sách bằng ASP.NET Core MVC, sử dụng Entity Framework Core để làm việc với SQL Server. Ứng dụng có đủ CRUD: thêm sách, đọc danh sách và chi tiết, cập nhật và xóa. Ngoài ra có tìm kiếm, lọc thể loại và phân trang.”

### 2. Database First và kết nối

**Thao tác:** mở script SQL, chỉ `CREATE DATABASE`, `CREATE TABLE dbo.Books`, `Id IDENTITY`, `Price DECIMAL(18,2)`. Sau đó mở BookConnection và dòng AddDbContext/UseSqlServer trong Program.cs.

“Bài này đi theo hướng Database First. Em tạo database BookManagementDB và bảng Books bằng script SQL trước. Id là khóa chính tự tăng, Price dùng decimal để lưu giá. Model và DbContext được ánh xạ theo cấu trúc bảng. Chuỗi kết nối dùng `(localdb)\MSSQLLocalDB` với tài khoản Windows; Program.cs đăng ký BookDbContext để Controller nhận được kết nối qua dependency injection.”

**Nếu được hỏi có scaffold tự động chưa:** “Bản chạy này dùng ánh xạ viết sẵn theo schema SQL. Em có lệnh scaffold ở README để sinh Model và DbContext trực tiếp từ database trên máy.” Không nói đã scaffold nếu chưa thực hiện.

### 3. Giải thích MVC đúng trọng tâm

**Thao tác:** mở Book.cs, BookDbContext.cs, BooksController.cs và chỉ thư mục Views/Books.

“Book là entity biểu diễn một bản ghi sách. BookDbContext ánh xạ entity vào bảng Books và làm việc với CSDL. BooksController nhận yêu cầu, kiểm tra dữ liệu rồi gọi DbContext. Các Razor View chịu trách nhiệm hiển thị. BookFormViewModel chứa dữ liệu nhập và các quy tắc kiểm tra, tách khỏi entity lưu trong database.”

Luồng cần nhớ: **Trình duyệt → Controller → DbContext → SQL Server → Controller → View → Trình duyệt**.

### 4. Thêm sách — phần giải thích chính

**Thao tác:** mở Create bằng GET, nhập **Demo MVC**, tác giả **Thạch Thiết**, thể loại **Công nghệ**, năm **2026**, giá **120000**, số lượng **5**. Bấm Lưu. Mở POST Create trong BooksController.

“GET Create chỉ hiển thị form. Khi bấm lưu, form gửi POST. MVC đưa các trường nhập vào BookFormViewModel và kiểm tra các thuộc tính Required, StringLength, Range. Nếu ModelState không hợp lệ thì trả lại form kèm lỗi. Nếu hợp lệ, em tạo Book, gọi Add để đánh dấu thêm mới, rồi SaveChangesAsync để EF thực hiện INSERT trong SQL Server. Cuối cùng chuyển về Index để tránh gửi lại form khi tải lại trang.”

**Minh họa validation:** thử bỏ tên hoặc nhập số lượng âm trước khi lưu. Nếu trình duyệt chặn, nói đó là kiểm tra HTML; server vẫn kiểm tra ModelState, không chỉ dựa vào trình duyệt.

**Điểm cần chỉ đúng:** `SaveChangesAsync` mới là bước ghi dữ liệu. `Add` chỉ đánh dấu entity cần được thêm.

### 5. Đọc dữ liệu và tìm kiếm

**Thao tác:** tìm Demo MVC, mở chi tiết. Mở Index, chỉ query, Where và ToListAsync.

“Index tạo truy vấn từ context.Books. Nếu có từ khóa thì lọc theo tên sách hoặc tác giả bằng Where. Nếu có thể loại thì thêm điều kiện lọc. Count tính số kết quả; Skip và Take lấy dữ liệu theo trang. ToListAsync thực thi truy vấn để lấy dữ liệu từ SQL Server, rồi Controller truyền ViewModel sang View. AsNoTracking dùng cho thao tác chỉ đọc, vì không cần theo dõi thay đổi.”

Không cần giải thích chi tiết từng dòng phân trang; giữ trọng tâm là **query → SQL → dữ liệu → View**.

### 6. Sửa và chứng minh database đã cập nhật

**Thao tác:** sửa giá **135000**, số lượng **9**. Mở POST Edit rồi chạy SELECT trong SSMS.

“GET Edit tải sách theo Id để điền vào form. POST Edit kiểm tra dữ liệu, tìm lại sách rồi gán các trường được phép sửa. Entity đã được DbContext theo dõi, nên SaveChangesAsync phát hiện thay đổi và gửi UPDATE. Id lấy từ route, còn ViewModel chỉ chứa các trường nội dung. Em kiểm tra lại trong SQL Server để chứng minh giá và số lượng đã được lưu thật.”

```sql
USE BookManagementDB;
SELECT Id, Title, Price, Quantity
FROM dbo.Books
WHERE Title = N'Demo MVC';
```

Nếu thầy hỏi “Sao không gọi Update?”: “Vì entity được tải bằng FindAsync đang được theo dõi; chỉ cần sửa thuộc tính và SaveChangesAsync.”

### 7. Xóa sách

**Thao tác:** bấm Xóa, chỉ trang xác nhận, bấm Xác nhận xóa. Mở DeleteConfirmed rồi chạy lại SELECT.

“GET Delete chỉ hiển thị xác nhận. POST DeleteConfirmed mới tìm sách, gọi Remove và SaveChangesAsync để thực hiện DELETE. ValidateAntiForgeryToken kiểm tra token do form sinh ra. Chạy lại SELECT không còn sách demo, chứng minh thao tác xóa đã tác động vào SQL Server.”

`ActionName("Delete")` giúp hàm DeleteConfirmed nhận request POST của action Delete. Không cần đi sâu hơn nếu không được hỏi.

### 8. View và chốt luồng

**Thao tác:** mở _BookForm.cshtml, chỉ `asp-for`, `asp-validation-for`; quay lại danh sách.

“View dùng Razor để hiển thị dữ liệu. asp-for gắn ô nhập với thuộc tính của ViewModel; asp-validation-for hiển thị lỗi từng trường. Partial _BookForm dùng chung cho thêm và sửa. Các thay đổi đều đi qua Controller và Entity Framework để ghi xuống SQL Server; trang danh sách đọc lại dữ liệu từ database sau mỗi thao tác.”

## Các câu hỏi nên chuẩn bị

| Câu hỏi | Trả lời ngắn |
| --- | --- |
| CRUD là gì? | Create, Read, Update, Delete: thêm, đọc, sửa, xóa. |
| Vì sao là Database First? | Schema tạo bằng SQL trước; ứng dụng ánh xạ và dùng database có sẵn, không tạo schema bằng migrations. |
| DbContext làm gì? | Quản lý truy vấn, ánh xạ, tracking và lưu thay đổi vào CSDL. |
| GET và POST khác nhau trong bài? | GET đọc/hiển thị form; POST xử lý thao tác ghi sau khi kiểm tra dữ liệu. |
| SaveChangesAsync làm gì? | Gửi các thay đổi đang được theo dõi xuống database. |
| EF có thay thế SQL Server không? | Không. EF là lớp truy cập dữ liệu; SQL Server vẫn lưu dữ liệu. |
| Làm sao chứng minh đồng bộ? | Sửa trên web rồi SELECT trong SSMS; tải lại trang thấy dữ liệu đã lưu. |
| Có tự cập nhật mọi trình duyệt realtime không? | Không. Trang đọc lại DB khi có request hoặc tải lại; đây là CRUD thông thường. |
| Vì sao dùng ViewModel? | Chỉ nhận trường cho phép sửa và kiểm tra dữ liệu nhập, tách khỏi entity. |
| Form sai thì sao? | Trả lại View với lỗi, không gọi SaveChangesAsync. |
| Sách không tồn tại thì sao? | Controller trả NotFound, HTTP 404. |
| Vì sao dùng decimal cho giá? | Phù hợp dữ liệu tiền và khớp cột decimal(18,2). |

## Những phần có thể lướt nhanh

Không đọc từng dòng CSS; không giải thích lặp lại mỗi ô input; không kể toàn bộ thao tác tạo project. Tập trung vào **SQL schema, kết nối, vai trò MVC, GET/POST, validation, SaveChangesAsync và bằng chứng CRUD trong database**.
