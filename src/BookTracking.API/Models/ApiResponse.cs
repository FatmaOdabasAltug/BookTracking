namespace BookTracking.API.Models;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public int ResponseCode { get; set; }
    public T? Data { get; set; }
    public string? HelperMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public List<string>? ValidationErrors { get; set; }

    public static ApiResponse<T> Success(T data, int code = 200, string message = "Operation successful")
    {
        return new ApiResponse<T>
        {
            IsSuccess = true,
            ResponseCode = code,
            Data = data,
            HelperMessage = message
        };
    }

    public static ApiResponse<T> Failure(string errorMessage, int code = 400, string message = "Operation failed")
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ResponseCode = code,
            Data = default,
            HelperMessage = message,
            ErrorMessage = errorMessage
        };
    }

    public static ApiResponse<T> ValidationError(List<string> errors, string message = "Validation errors occurred")
    {
        return new ApiResponse<T>
        {
            IsSuccess = false,
            ResponseCode = 400,
            Data = default,
            HelperMessage = message,
            ValidationErrors = errors
        };
    }
}
