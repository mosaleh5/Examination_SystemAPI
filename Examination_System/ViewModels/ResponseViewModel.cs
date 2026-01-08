using Examination_System.Models.Enums;

namespace Examination_System.ViewModels
{
    public class ResponseViewModel<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public ErrorCode? ErrorCode { get; set; }
        public object? Errors { get; set; }

        public static ResponseViewModel<T> Success(T? data, string? message = null)
        {
            return new ResponseViewModel<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message ?? "Operation completed successfully"
            };
        }

        public static ResponseViewModel<T> Failure(ErrorCode errorCode, string message, object? errors = null)
        {
            return new ResponseViewModel<T>
            {
                IsSuccess = false,
                ErrorCode = errorCode,
                Message = message,
                Errors = errors
            };
        }
      
    }
}