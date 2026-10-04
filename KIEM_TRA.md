# Kết quả kiểm tra project

- Biên dịch project và solution với .NET SDK 10: thành công.
- Biên dịch Release: 0 lỗi, 0 cảnh báo.
- Kiểm tra tích hợp qua HTTP với Razor Views thực tế: 27 kiểm tra đạt.

Các luồng được kiểm tra: danh sách và phân trang, tìm kiếm tên, lọc thể loại, thêm sách, lưu tiếng Việt và giá thập phân, mở chi tiết, sửa sách, từ chối dữ liệu sai, trả 404 khi sách không tồn tại, GET xóa chỉ hiển thị xác nhận, POST xóa mới ghi dữ liệu, anti-forgery token, mã hóa nội dung HTML, lấy Id từ route để tránh form đổi bản ghi khác, đọc bảng tại trang kiểm tra CSDL, tên bảng/schema và precision của cột giá.

Kiểm tra tích hợp sử dụng SQLite trong bộ nhớ làm database thử nghiệm, thay provider trong bộ kiểm tra. **Project gửi cho bạn vẫn cấu hình SQL Server LocalDB**, không có SQLite hoặc dữ liệu giả trong ứng dụng.

Chưa chạy script T-SQL trên SQL Server và chưa kiểm tra kết nối tới LocalDB trên máy Windows của bạn. Để xác nhận đầy đủ phần SQL Server, hãy chạy script, mở `/Home/Database`, rồi làm các bước thêm/sửa/xóa và SELECT trong phần 8 của README. Tên server LocalDB chỉ trỏ tới máy của người chạy ứng dụng.

Không kèm bin/obj, cache NuGet hoặc bộ kiểm tra tạm trong file ZIP. Lần đầu mở project cần restore các package được khai báo trong csproj.
