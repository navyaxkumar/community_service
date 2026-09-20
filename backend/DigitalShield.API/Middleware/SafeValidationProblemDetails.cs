using Microsoft.AspNetCore.Mvc;

namespace DigitalShield.API.Middleware;

public static class SafeValidationProblemDetails
{
    public static IActionResult Create(ActionContext context)
    {
        var problem = new ValidationProblemDetails(context.ModelState)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed",
            Detail = "One or more request values are invalid.",
            Type = "https://httpstatuses.com/400"
        };
        problem.Extensions["traceId"] = context.HttpContext.TraceIdentifier;
        return new BadRequestObjectResult(problem)
        {
            ContentTypes = { "application/problem+json" }
        };
    }
}
