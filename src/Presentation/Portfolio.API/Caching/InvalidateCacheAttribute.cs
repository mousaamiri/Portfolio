using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Portfolio.API.Caching;

// Purges cached public API responses after an admin write, so a saved edit is
// visible on the next page load instead of up to the output-cache TTL later.
// The purge runs only on a successful (2xx) result and completes before the
// response is returned — the admin's redirect lands after the cache is clean.
//
//     [HttpPost]
//     [InvalidateCache("projects")]
//     public async Task<IActionResult> Create(...)
//
// For data embedded on every page (the profile) use <see cref="InvalidateAll"/>:
//
//     [HttpPut]
//     [InvalidateCache(InvalidateAll = true)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class InvalidateCacheAttribute : Attribute, IAsyncActionFilter
{
    // Parameterless form for <see cref="InvalidateAll"/> (no single resource to evict).
    public InvalidateCacheAttribute()
    {
    }

    public InvalidateCacheAttribute(string resource)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(resource);
        Resource = resource;
    }

    // Tag whose cached entries are evicted. It must match a tag on the public
    // endpoint ([OutputCache(Tags = ["projects"])]), which is the public route
    // segment (e.g. "impact-metrics"). Null when <see cref="InvalidateAll"/> is set.
    public string? Resource { get; }

    // Evict every cached public response (the profile is embedded on all pages).
    public bool InvalidateAll { get; set; }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executed = await next();

        // A failed write (400/404, an exception) cannot have changed the data.
        var status = executed.HttpContext.Response.StatusCode;
        if (status is < StatusCodes.Status200OK or >= StatusCodes.Status300MultipleChoices)
            return;

        var invalidator = executed.HttpContext.RequestServices.GetRequiredService<ICacheInvalidator>();
        var cancellationToken = executed.HttpContext.RequestAborted;

        try
        {
            if (InvalidateAll)
                await invalidator.InvalidateAllAsync(cancellationToken);
            else if (!string.IsNullOrEmpty(Resource))
                await invalidator.InvalidateAsync(Resource, cancellationToken);
        }
        catch (Exception ex)
        {
            // A purge failure must not break an otherwise-successful admin edit;
            // the TTL still bounds staleness.
            var logger = executed.HttpContext.RequestServices.GetService<ILogger<InvalidateCacheAttribute>>();
            logger?.LogWarning(ex, "Could not evict cached responses tagged '{Tag}'.", Resource);
        }
    }
}
