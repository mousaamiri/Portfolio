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
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client (browser / SPA-router AbortController) closed the
            // connection mid-request — normal, not a server fault. TaskCanceledException
            // derives from OperationCanceledException, so this covers both. Don't log
            // it as an error and don't try to write a body to a dead connection.
            logger.LogDebug("Request {Method} {Path} was canceled by the client.",
                context.Request.Method, context.Request.Path);

            if (!context.Response.HasStarted)
            {
                // 499 = "Client Closed Request" (nginx convention) — never reaches the
                // browser (it already left), but keeps access logs honest.
                context.Response.StatusCode = 499;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            // If the response already began streaming we can't rewrite the status
            // or body — attempting to would throw and mask the original error.
            if (context.Response.HasStarted)
                return;

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";

            var response = ApiResponse.Fail("An unexpected error occurred. Please try again later.");
            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonDefaults.CamelCase));
        }
    }
}
