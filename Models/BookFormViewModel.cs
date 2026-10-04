using System.ComponentModel.DataAnnotations;

namespace BookManagement.Models;

// Chỉ nhận các trường được phép sửa; không nhận Id từ form.
public class BookFormViewModel
{
    [Display(Name = "Tên sách")]
    [Required(ErrorMessage = "Vui lòng nhập tên sách.")]
    [StringLength(200, ErrorMessage = "Tên sách tối đa 200 ký tự.")]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Tác giả")]
    [Required(ErrorMessage = "Vui lòng nhập tác giả.")]
    [StringLength(120, ErrorMessage = "Tác giả tối đa 120 ký tự.")]
    public string Author { get; set; } = string.Empty;

    [Display(Name = "Thể loại")]
    [Required(ErrorMessage = "Vui lòng nhập thể loại.")]
    [StringLength(80, ErrorMessage = "Thể loại tối đa 80 ký tự.")]
    public string Genre { get; set; } = string.Empty;

    [Display(Name = "Nhà xuất bản")]
    [StringLength(150, ErrorMessage = "Nhà xuất bản tối đa 150 ký tự.")]
    public string? Publisher { get; set; }

    [Display(Name = "Năm xuất bản")]
    [Required(ErrorMessage = "Vui lòng nhập năm xuất bản.")]
    [Range(1000, 2100, ErrorMessage = "Năm xuất bản phải từ 1000 đến 2100.")]
    public int? PublishedYear { get; set; } = DateTime.Now.Year;

    [Display(Name = "Giá bán (VNĐ)")]
    [Required(ErrorMessage = "Vui lòng nhập giá bán.")]
    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Giá bán phải từ 0 đến 999.999.999 VNĐ.")]
    [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Giá bán có tối đa 2 chữ số thập phân.")]
    public decimal? Price { get; set; }

    [Display(Name = "Số lượng")]
    [Required(ErrorMessage = "Vui lòng nhập số lượng.")]
    [Range(0, 1000000, ErrorMessage = "Số lượng phải từ 0 đến 1.000.000.")]
    public int? Quantity { get; set; } = 0;

    [Display(Name = "Mô tả")]
    [StringLength(2000, ErrorMessage = "Mô tả tối đa 2000 ký tự.")]
    public string? Description { get; set; }

    public static BookFormViewModel FromBook(Book book) => new()
    {
        Title = book.Title, Author = book.Author, Genre = book.Genre,
        Publisher = book.Publisher, PublishedYear = book.PublishedYear,
        Price = book.Price, Quantity = book.Quantity, Description = book.Description
    };

    public void ApplyTo(Book book)
    {
        book.Title = Title.Trim();
        book.Author = Author.Trim();
        book.Genre = Genre.Trim();
        book.Publisher = string.IsNullOrWhiteSpace(Publisher) ? null : Publisher.Trim();
        book.PublishedYear = PublishedYear!.Value;
        book.Price = Price!.Value;
        book.Quantity = Quantity!.Value;
        book.Description = string.IsNullOrWhiteSpace(Description) ? null : Description.Trim();
    }
}
