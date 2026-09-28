namespace SchoolApiService.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }

        public ApiResponse() { }

        public ApiResponse(bool success, int statusCode, string message, T? data = default, List<string>? errors = null)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
            Errors = errors;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Request processed successfully.", int statusCode = 200)
        {
            return new ApiResponse<T>
            {
                Success = true,
                StatusCode = statusCode,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
        {
            var errList = errors ?? new List<string>();
            if (errList.Count == 0 && !string.IsNullOrWhiteSpace(message))
            {
                errList.Add(message);
            }

            return new ApiResponse<T>
            {
                Success = false,
                StatusCode = statusCode,
                Message = message,
                Data = default,
                Errors = errList.Count > 0 ? errList : null
            };
        }
    }
}
