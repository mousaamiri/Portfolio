using System.Text.Json;
using Portfolio.API.Common;

namespace Portfolio.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Fail("An unexpected error occurred. Please try again later.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonDefaults.CamelCase));
        }
    }
}
