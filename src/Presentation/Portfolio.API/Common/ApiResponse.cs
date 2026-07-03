namespace Portfolio.API.Common;

public class ApiResponse
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public List<string>? Errors { get; init; }

    public static ApiResponse Ok(string? message = null) =>
        new() { Success = true, Message = message };

    public static ApiResponse Fail(string message) =>
        new() { Success = false, Message = message };

    public static ApiResponse Fail(List<string> errors) =>
        new() { Success = false, Message = "One or more validation errors occurred.", Errors = errors };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }

    public static ApiResponse<T> Ok(T data, string? message = null) =>
        new() { Success = true, Data = data, Message = message };

    public new static ApiResponse<T> Fail(string message) =>
        new() { Success = false, Message = message };
}
