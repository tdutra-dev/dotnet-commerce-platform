using System.Text.Json;
using Commerce.Api.Services;
using Microsoft.AspNetCore.Diagnostics;

namespace Commerce.Api.Middleware;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            ValidationException e => (400, e.Message),
            NotFoundException e => (404, e.Message),
            ConflictException e => (409, e.Message),
            InvalidOperationException e => (409, e.Message),
            _ => (500, "An unexpected error occurred.")
        };
        if (status == 500) logger.LogError(exception, "Unhandled request exception");
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsJsonAsync(new { status, title, detail = exception.Message, instance = context.Request.Path }, ct);
        return true;
    }
}
