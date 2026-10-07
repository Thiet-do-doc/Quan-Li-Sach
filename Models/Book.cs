namespace BookManagement.Models;

// Ánh xạ theo bảng dbo.Books đã được tạo bằng script SQL.
// Không dùng migrations/EnsureCreated để tạo hoặc sửa schema khi chạy app.
public partial class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public int PublishedYear { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }

    public string? ImagePath { get; set; }
}
