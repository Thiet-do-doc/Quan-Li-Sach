using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Controllers;

public class BooksController(BookDbContext context, ILogger<BooksController> logger) : Controller
{
    private const int PageSize = 8;

    // READ: lọc tại SQL Server, sau đó lấy dữ liệu của trang hiện tại.
    public async Task<IActionResult> Index(string? search, string? genre, int page = 1)
    {
        search = search?.Trim();
        genre = genre?.Trim();
        if (search?.Length > 100) search = search[..100];
        var query = context.Books.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Title.Contains(search) || b.Author.Contains(search));
        if (!string.IsNullOrWhiteSpace(genre))
            query = query.Where(b => b.Genre == genre);

        var count = await query.CountAsync();
        var totalPages = Math.Max(1, (int)Math.Ceiling(count / (double)PageSize));
        page = Math.Clamp(page, 1, totalPages);
        return View(new BookIndexViewModel
        {
            Books = await query.OrderByDescending(b => b.Id).Skip((page - 1) * PageSize)
                .Take(PageSize).ToListAsync(),
            Genres = await context.Books.AsNoTracking().Select(b => b.Genre).Distinct()
                .OrderBy(g => g).ToListAsync(),
            Search = search, Genre = genre, Page = page, TotalPages = totalPages,
            FilteredCount = count, TotalBooks = await context.Books.CountAsync()
        });
    }

    public async Task<IActionResult> Details([FromRoute] int id)
    {
        var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        return book is null ? NotFound() : View(book);
    }

    // GET hiển thị form; POST kiểm tra dữ liệu và ghi vào CSDL.
    public IActionResult Create() => View(new BookFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookFormViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var book = new Book();
        model.ApplyTo(book);
        context.Books.Add(book);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Không thể thêm sách.");
            ModelState.AddModelError(string.Empty, "Không lưu được sách. Hãy kiểm tra kết nối và cấu trúc bảng Books.");
            return View(model);
        }
        TempData["Success"] = "Đã thêm sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit([FromRoute] int id)
    {
        var book = await context.Books.FindAsync(id);
        if (book is null) return NotFound();
        ViewData["BookId"] = id;
        return View(BookFormViewModel.FromBook(book));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit([FromRoute] int id, BookFormViewModel model)
    {
        ViewData["BookId"] = id;
        if (!ModelState.IsValid) return View(model);
        var book = await context.Books.FindAsync(id);
        if (book is null) return NotFound();
        // Chỉ sửa các trường trong form. EF theo dõi thay đổi của sách đã tải.
        model.ApplyTo(book);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound(); // Bản ghi đã bị xóa trước khi lưu.
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Không thể sửa sách {BookId}.", id);
            ModelState.AddModelError(string.Empty, "Không lưu được thay đổi. Hãy kiểm tra kết nối và dữ liệu.");
            return View(model);
        }
        TempData["Success"] = "Đã cập nhật sách thành công.";
        return RedirectToAction(nameof(Index));
    }

    // GET chỉ hỏi xác nhận; thao tác xóa thật phải đi qua POST.
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var book = await context.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id);
        return book is null ? NotFound() : View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
    {
        var book = await context.Books.FindAsync(id);
        if (book is null) return NotFound();
        context.Books.Remove(book);
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Không thể xóa sách {BookId}.", id);
            ModelState.AddModelError(string.Empty, "Không xóa được sách. Hãy kiểm tra kết nối và thử lại.");
            return View("Delete", book);
        }
        TempData["Success"] = "Đã xóa sách thành công.";
        return RedirectToAction(nameof(Index));
    }
}
