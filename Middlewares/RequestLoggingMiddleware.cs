using System.Diagnostics;

namespace BookManagement.Middlewares;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var timer = Stopwatch.StartNew();
        var time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var method = context.Request.Method;
        var path = context.Request.Path.ToString();

        Console.WriteLine($"[{time}] Method: {method} - Path: {path}");

        if (IsInvalidBookId(path))
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync("Book id không hợp lệ");

            LogStatus(context, timer);
            return; // Không gọi _next: request không vào Controller.
        }

        await _next(context);

        // Chạy khi middleware phía sau và Controller đã xử lý xong.
        LogStatus(context, timer);
    }

    private static bool IsInvalidBookId(string path)
    {
        var parts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3) return false;

        // Hỗ trợ URL trong đề và URL của project hôm qua.
        var isBook = parts[0].Equals("Book", StringComparison.OrdinalIgnoreCase)
            || parts[0].Equals("Books", StringComparison.OrdinalIgnoreCase);
        var isDetail = parts[1].Equals("Detail", StringComparison.OrdinalIgnoreCase)
            || parts[1].Equals("Details", StringComparison.OrdinalIgnoreCase);

        return isBook && isDetail
            && int.TryParse(parts[2], out var id) && id <= 0;
    }

    private static void LogStatus(HttpContext context, Stopwatch timer)
    {
        timer.Stop();
        Console.WriteLine($"Status Code: {context.Response.StatusCode}"
            + $" - Method: {context.Request.Method} - Path: {context.Request.Path}"
            + $" - Duration: {timer.Elapsed.TotalMilliseconds:F2} ms");
    }
}
