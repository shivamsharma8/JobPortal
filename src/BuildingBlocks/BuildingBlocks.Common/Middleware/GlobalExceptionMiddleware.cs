using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ArgumentNullException => CreateProblemDetails(HttpStatusCode.BadRequest, "Null argument.", exception.Message),
            UnauthorizedAccessException => CreateProblemDetails(HttpStatusCode.Unauthorized, "Unauthorized.", exception.Message),
            KeyNotFoundException => CreateProblemDetails(HttpStatusCode.NotFound, "Not Found.", exception.Message),
            InvalidOperationException => CreateProblemDetails(HttpStatusCode.Conflict, "Conflict.", exception.Message),
            _ => CreateProblemDetails(HttpStatusCode.InternalServerError, "Internal Server Error.", "An unexpected error occurred.")
        };

        context.Response.StatusCode = response.StatusCode;
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }

    private static ApiProblemDetails CreateProblemDetails(HttpStatusCode statusCode, string title, string detail)
        => new()
        {
            StatusCode = (int)statusCode,
            Title = title,
            Detail = detail,
            TraceId = Guid.NewGuid().ToString()
        };
}

public class ApiProblemDetails
{
    public int StatusCode { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
    public string TraceId { get; set; } = string.Empty;
}
