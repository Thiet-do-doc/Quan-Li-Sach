using System.Globalization;
using BookManagement.Data;
using BookManagement.Middlewares;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BookDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BookConnection")
        ?? throw new InvalidOperationException("Thiếu cấu hình BookConnection.")));

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Dùng en-US để dấu chấm trong ô giá HTML type=number khớp với model binding.
// Giao diện và hiển thị tiền vẫn dùng tiếng Việt trong Razor Views.
var inputCulture = new CultureInfo("en-US");
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(inputCulture),
    SupportedCultures = new[] { inputCulture },
    SupportedUICultures = new[] { inputCulture },
    RequestCultureProviders = new List<IRequestCultureProvider>()
});
// Đặt trước static files để ghi log cả request CSS và các request bị chặn.
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseStaticFiles();
app.UseRouting();

// URL theo đề bài, dùng lại BooksController và các View hiện có.
app.MapControllerRoute(
    name: "book-detail",
    pattern: "Book/Detail/{id?}",
    defaults: new { controller = "Books", action = "Details" });

app.MapControllerRoute(
    name: "book-alias",
    pattern: "Book/{action=Index}/{id?}",
    defaults: new { controller = "Books" });

app.MapControllerRoute(name: "default", pattern: "{controller=Books}/{action=Index}/{id?}");
app.Run();
