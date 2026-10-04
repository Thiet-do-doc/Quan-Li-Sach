using System.Globalization;
using BookManagement.Data;
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
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Books}/{action=Index}/{id?}");
app.Run();
