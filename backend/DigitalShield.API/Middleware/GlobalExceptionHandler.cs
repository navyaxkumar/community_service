using DigitalShield.API.Authentication;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Middleware;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;
        var (status, title, detail) = exception switch
        {
            AuthenticationException => (StatusCodes.Status401Unauthorized, "Authentication failed", "Invalid email or password."),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request", "One or more request values are invalid."),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found", "The requested resource was not found."),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Request conflict", "The request conflicts with the current resource state."),
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Forbidden", "You are not allowed to perform this operation."),
            _ => (StatusCodes.Status500InternalServerError, "Internal server error", "An unexpected error occurred.")
        };

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);
        }
        else
        {
            _logger.LogWarning("Handled {ExceptionType} as HTTP {StatusCode}. TraceId: {TraceId}",
                exception.GetType().Name, status, traceId);
        }

        if (httpContext.Response.HasStarted)
        {
            return false;
        }

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application/problem+json";
        var problem = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{status}"
        };
        problem.Extensions["traceId"] = traceId;
        await JsonSerializer.SerializeAsync(httpContext.Response.Body, problem, cancellationToken: cancellationToken);
        return true;
    }
}
