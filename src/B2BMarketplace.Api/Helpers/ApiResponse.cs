using Microsoft.AspNetCore.Mvc;

namespace B2BMarketplace.Api.Helpers
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }

        public ApiResponse(bool success, string message, T? data, int statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = statusCode;
            Timestamp = DateTime.UtcNow;
        }

        public static ApiResponse<T> CreateSuccess(T data, string message = "Request successful", int statusCode = 200)
        {
            return new ApiResponse<T>(true, message, data, statusCode);
        }

        public static ApiResponse<T> CreateFailure(string message = "Request failed", int statusCode = 400)
        {
            return new ApiResponse<T>(false, message, default(T), statusCode);
        }

        public static ApiResponse<T> CreateCreated(T data, string message = "Created successfully")
        {
            return new ApiResponse<T>(true, message, data, 201);
        }

        public static ApiResponse<T> CreateNotFound(string message = "Not found")
        {
            return new ApiResponse<T>(false, message, default(T), 404);
        }
    }
}