using BookManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookManagement.Controllers;

public class HomeController(BookDbContext context, ILogger<HomeController> logger) : Controller
{
    public async Task<IActionResult> Database()
    {
        try
        {
            // Kiểm tra cả kết nối và bảng, không chỉ mở được SQL Server.
            var count = await context.Books.AsNoTracking().CountAsync();
            ViewData["Connected"] = true;
            ViewData["Count"] = count;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Kiểm tra kết nối database thất bại.");
            ViewData["Connected"] = false;
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}
