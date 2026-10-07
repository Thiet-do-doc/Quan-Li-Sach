using BookManagement.Data;
using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Controllers;

public class BooksController(
    BookDbContext context,
    ILogger<BooksController> logger,
    IWebHostEnvironment environment) : Controller
{
    private const int PageSize = 8;

    // READ: lọc tại SQL Server, sau đó lấy dữ liệu của trang hiện tại.
    public async Task<IActionResult> Index(string? search, string? genre, int page = 1)
    {
        search = search?.Trim();
        genre = genre?.Trim();

        if (search?.Length > 100)
            search = search[..100];

        var query = context.Books.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(b =>
                b.Title.Contains(search) ||
                b.Author.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            query = query.Where(b => b.Genre == genre);
        }

        var count = await query.CountAsync();

        var totalPages = Math.Max(
            1,
            (int)Math.Ceiling(count / (double)PageSize)
        );

        page = Math.Clamp(page, 1, totalPages);

        return View(new BookIndexViewModel
        {
            Books = await query
                .OrderByDescending(b => b.Id)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync(),

            Genres = await context.Books
                .AsNoTracking()
                .Select(b => b.Genre)
                .Distinct()
                .OrderBy(g => g)
                .ToListAsync(),

            Search = search,
            Genre = genre,
            Page = page,
            TotalPages = totalPages,
            FilteredCount = count,
            TotalBooks = await context.Books.CountAsync()
        });
    }

    // DETAILS
    public async Task<IActionResult> Details([FromRoute] int id)
    {
        var book = await context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return book is null
            ? NotFound()
            : View(book);
    }

    // CREATE - GET
    public IActionResult Create()
    {
        return View(new BookFormViewModel());
    }

    // CREATE - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        BookFormViewModel model,
        IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var book = new Book();

        model.ApplyTo(book);

        // Xử lý upload ảnh
        if (imageFile != null && imageFile.Length > 0)
        {
            var allowedExtensions = new[]
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".gif",
                ".webp"
            };

            var extension = Path
                .GetExtension(imageFile.FileName)
                .ToLowerInvariant();

            // Kiểm tra định dạng ảnh
            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError(
                    "imageFile",
                    "Chỉ được upload ảnh JPG, JPEG, PNG, GIF hoặc WEBP."
                );

                return View(model);
            }

            // Giới hạn ảnh tối đa 5 MB
            if (imageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(
                    "imageFile",
                    "Ảnh không được lớn hơn 5 MB."
                );

                return View(model);
            }

            // Đường dẫn: wwwroot/uploads/books
            var uploadFolder = Path.Combine(
                environment.WebRootPath,
                "uploads",
                "books"
            );

            // Nếu thư mục chưa có thì tự tạo
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Tạo tên file mới để tránh trùng
            var fileName = $"{Guid.NewGuid()}{extension}";

            var filePath = Path.Combine(
                uploadFolder,
                fileName
            );

            // Lưu ảnh thật vào wwwroot
            using (var stream = new FileStream(
                filePath,
                FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Lưu đường dẫn ảnh vào SQL Server
            book.ImagePath = $"/uploads/books/{fileName}";
        }

        context.Books.Add(book);

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(
                ex,
                "Không thể thêm sách."
            );

            ModelState.AddModelError(
                string.Empty,
                "Không lưu được sách. Hãy kiểm tra kết nối và cấu trúc bảng Books."
            );

            return View(model);
        }

        TempData["Success"] = "Đã thêm sách thành công.";

        return RedirectToAction(nameof(Index));
    }

    // EDIT - GET
    public async Task<IActionResult> Edit([FromRoute] int id)
    {
        var book = await context.Books.FindAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        ViewData["BookId"] = id;
        ViewData["ImagePath"] = book.ImagePath;

        return View(
            BookFormViewModel.FromBook(book)
        );
    }

    // EDIT - POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [FromRoute] int id,
        BookFormViewModel model,
        IFormFile? imageFile)
    {
        ViewData["BookId"] = id;

        var book = await context.Books.FindAsync(id);

        if (book is null)
        {
            return NotFound();
        }

        ViewData["ImagePath"] = book.ImagePath;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (imageFile is { Length: > 0 })
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = Path.GetExtension(imageFile.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("imageFile", "Chỉ được upload ảnh JPG, JPEG, PNG, GIF hoặc WEBP.");
                return View(model);
            }

            if (imageFile.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError("imageFile", "Ảnh không được lớn hơn 5 MB.");
                return View(model);
            }

            var uploadFolder = Path.Combine(environment.WebRootPath, "uploads", "books");
            Directory.CreateDirectory(uploadFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            book.ImagePath = $"/uploads/books/{fileName}";
        }

        // Chỉ sửa các trường trong form
        model.ApplyTo(book);

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
            logger.LogError(
                ex,
                "Không thể sửa sách {BookId}.",
                id
            );

            ModelState.AddModelError(
                string.Empty,
                "Không lưu được thay đổi. Hãy kiểm tra kết nối và dữ liệu."
            );

            return View(model);
        }

        TempData["Success"] = "Đã cập nhật sách thành công.";

        return RedirectToAction(nameof(Index));
    }

    // DELETE - GET
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var book = await context.Books
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id);

        return book is null
            ? NotFound()
            : View(book);
    }

    // DELETE - POST
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed([FromRoute] int id)
    {
        var book = await context.Books.FindAsync(id);

        if (book is null)
        {
            return NotFound();
        }

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
            logger.LogError(
                ex,
                "Không thể xóa sách {BookId}.",
                id
            );

            ModelState.AddModelError(
                string.Empty,
                "Không xóa được sách. Hãy kiểm tra kết nối và thử lại."
            );

            return View("Delete", book);
        }

        TempData["Success"] = "Đã xóa sách thành công.";

        return RedirectToAction(nameof(Index));
    }
}