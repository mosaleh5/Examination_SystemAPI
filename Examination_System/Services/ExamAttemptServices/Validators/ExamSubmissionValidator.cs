using Examination_System.Common;
using Examination_System.Common.Constants;
using Examination_System.DTOs.ExamAttempt;
using Examination_System.Models;
using Examination_System.Models.Enums;

namespace Examination_System.Services.ExamAttemptServices.Validators
{
    public interface IExamSubmissionValidator
    {
        Result<object> Validate(ExamAttempt attempt, List<SubmitAnswerDto> answers);
    }

    public class ExamSubmissionValidator : IExamSubmissionValidator
    {
        public Result<object> Validate(ExamAttempt attempt, List<SubmitAnswerDto> answers)
        {
            if (attempt.IsCompleted)
            {
                return Result<object>.Failure(
                    ErrorCode.Conflict,
                    string.Format(ErrorMessages.ExamAttemptAlreadySubmitted, attempt.Id));
            }

            var durationInMinutes = (DateTime.UtcNow - attempt.StartedAt).TotalMinutes;
            if (durationInMinutes > attempt.Exam.DurationInMinutes)
            {
                return Result<object>.Failure(
                    ErrorCode.BadRequest,
                    string.Format(ErrorMessages.ExamTimeExceeded, attempt.Exam.DurationInMinutes, durationInMinutes));
            }

            if (answers.Count != attempt.Exam.QuestionsCount)
            {
                return Result<object>.Failure(
                    ErrorCode.ValidationError,
                    string.Format(ErrorMessages.InvalidAnswerCount, attempt.Exam.QuestionsCount, answers.Count));
            }

            return Result<object>.Success(null);
        }
    }
}