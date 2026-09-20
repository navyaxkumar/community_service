using System.Text.Json;
using DigitalShield.API.Authentication;
using DigitalShield.API.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace DigitalShield.Tests;

public class GlobalErrorHandlingTests
{
    [Theory]
    [InlineData(typeof(ArgumentException), 400, "Invalid request")]
    [InlineData(typeof(InvalidOperationException), 409, "Request conflict")]
    [InlineData(typeof(AuthenticationException), 401, "Authentication failed")]
    [InlineData(typeof(UnauthorizedAccessException), 403, "Forbidden")]
    [InlineData(typeof(KeyNotFoundException), 404, "Resource not found")]
    public async Task KnownExceptionsBecomeSafeProblemDetails(Type exceptionType, int status, string title)
    {
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-known";
        context.Response.Body = new MemoryStream();
        var exception = (Exception)Activator.CreateInstance(exceptionType, "sensitive internal detail")!;

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);
        var problem = await ReadProblemDetails(context);

        Assert.True(handled);
        Assert.Equal(status, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Equal(title, problem.RootElement.GetProperty("title").GetString());
        Assert.Equal("trace-known", problem.RootElement.GetProperty("traceId").GetString());
        Assert.DoesNotContain("sensitive internal detail", problem.RootElement.GetRawText(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UnexpectedExceptionReturnsGeneric500WithoutInternalDetails()
    {
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-unexpected";
        context.Response.Body = new MemoryStream();
        var exception = new InvalidOperationException("SQL connection string and C:\\private\\source.cs:42");

        var handled = await handler.TryHandleAsync(context, exception, CancellationToken.None);
        var problem = await ReadProblemDetails(context);

        Assert.True(handled);
        Assert.Equal(409, context.Response.StatusCode);
        Assert.DoesNotContain("C:\\private", problem.RootElement.GetRawText(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task TrulyUnexpectedExceptionReturnsGeneric500()
    {
        var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance);
        var context = new DefaultHttpContext();
        context.TraceIdentifier = "trace-500";
        context.Response.Body = new MemoryStream();
        var exception = new FormatException("database password=secret; source path C:\\private\\source.cs");

        await handler.TryHandleAsync(context, exception, CancellationToken.None);
        var problem = await ReadProblemDetails(context);

        Assert.Equal(500, context.Response.StatusCode);
        Assert.Equal("An unexpected error occurred.", problem.RootElement.GetProperty("detail").GetString());
        Assert.DoesNotContain("database password", problem.RootElement.GetRawText(), StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("source.cs", problem.RootElement.GetRawText(), StringComparison.OrdinalIgnoreCase);
        Assert.Equal("trace-500", problem.RootElement.GetProperty("traceId").GetString());
    }

    private static async Task<JsonDocument> ReadProblemDetails(HttpContext context)
    {
        context.Response.Body.Position = 0;
        return await JsonDocument.ParseAsync(context.Response.Body);
    }
}
