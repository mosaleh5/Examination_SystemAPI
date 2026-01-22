using Examination_System.DTOs.Question;
using Examination_System.Models.Enums;
using FluentValidation.Results;

namespace Examination_System.Common
{
    /// <summary>
    /// Represents the result of an operation without data
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; protected set; }
        public ErrorCode Error { get; protected set; }
        public string? ErrorMessage { get; protected set; }

        protected Result(bool isSuccess, ErrorCode error, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Error = error;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new Result(true, ErrorCode.None, null);

        public static Result Failure(ErrorCode error, string errorMessage)
            => new Result(false, error, errorMessage);
        
        public static Result ValidaitonFailure(ValidationResult validationResult)
        {
            var errorMessage = string.Join("; ",
                 validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            return Result.Failure(
                ErrorCode.ValidationError, "Validation Failed: " +
                "\n" +
                errorMessage);
        }
    }

    /// <summary>
    /// Represents the result of an operation with data
    /// </summary>
    public class Result<T> : Result where T : class
    {
        public T? Data { get; private set; }

        private Result(bool isSuccess, T? data, ErrorCode error, string? errorMessage)
            : base(isSuccess, error, errorMessage)
        {
            Data = data;
        }

        public static Result<T> Success(T data, string? message = null)
            => new Result<T>(true, data, ErrorCode.None, message);

        public static new Result<T> Failure(ErrorCode error, string errorMessage)
            => new Result<T>(false, null, error, errorMessage);
        
        public static new Result<T> ValidaitonFailure(ValidationResult validationResult)
        {
            var errorMessage = string.Join("; ",
                 validationResult.Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"));

            return Result<T>.Failure(
                ErrorCode.ValidationError, "Validation Failed: " +
                "\n" +
                errorMessage);
        }

    }
}