using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Common;

namespace Portfolio.API.Common;

public static class ResultExtensions
{
    public static ActionResult<ApiResponse<T>> ToOkResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(ApiResponse<T>.Ok(result.Value!));

        return new BadRequestObjectResult(ApiResponse.Fail(result.Error!));
    }

    public static ActionResult<ApiResponse<T>> ToOkOrNotFoundResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
            return new OkObjectResult(ApiResponse<T>.Ok(result.Value!));

        return new NotFoundObjectResult(ApiResponse.Fail(result.Error!));
    }

    public static ActionResult<ApiResponse<Guid>> ToCreatedResult(
        this Result<Guid> result, string routeName, Func<Guid, object> routeValues)
    {
        if (result.IsSuccess)
        {
            var response = ApiResponse<Guid>.Ok(result.Value);
            return new CreatedAtRouteResult(routeName, routeValues(result.Value), response);
        }

        return new BadRequestObjectResult(ApiResponse.Fail(result.Error!));
    }
}
