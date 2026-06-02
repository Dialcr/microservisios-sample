using System.Net;
using System.Text.Json;

namespace NotificationService.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try { await _next(context); }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var problem = new { type = "https://tools.ietf.org/html/rfc7231#section-6.6.1", title = "Internal Server Error", status = 500, detail = ex.Message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}
